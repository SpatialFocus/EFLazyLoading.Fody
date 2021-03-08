// <copyright file="ModuleWeaver.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace SpatialFocus.EFLazyLoading.Fody
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using global::Fody;
	using Mono.Cecil;
	using Mono.Cecil.Cil;
	using Mono.Cecil.Rocks;

	public partial class ModuleWeaver : BaseModuleWeaver
	{
		public override void Execute()
		{
			Namespaces namespaces = new Namespaces(this);
			References references = Fody.References.Init(this);

			foreach (TypeDefinition typeDefinition in ModuleDefinition.Types)
			{
				if (!namespaces.ShouldIncludeType(typeDefinition))
				{
					continue;
				}

				if(typeDefinition.Name != "Customer") continue;

				WriteDebug($"Weaving type {typeDefinition}");

				bool hasLazyLoaderConstructor = typeDefinition.GetConstructors().Any(x => x.Parameters.Any(x => string.Equals(x.Name, "lazyLoader", StringComparison.OrdinalIgnoreCase)));
				bool hasLazyLoadingField = typeDefinition.Fields.Any(x => string.Equals(x.Name, "lazyLoader", StringComparison.OrdinalIgnoreCase));

				if (!hasLazyLoaderConstructor && !hasLazyLoadingField)
				{
					FieldDefinition lazyLoaderField = new FieldDefinition("lazyLoader", FieldAttributes.Private | FieldAttributes.InitOnly, references.LazyLoaderType);
					typeDefinition.Fields.Add(lazyLoaderField);

					foreach (MethodDefinition methodDefinition in typeDefinition.GetConstructors().ToList())
					{
						MethodDefinition method = new MethodDefinition(methodDefinition.Name, methodDefinition.Attributes, ModuleDefinition.TypeSystem.Void);

						foreach (ParameterDefinition parameterDefinition in methodDefinition.Parameters)
						{
							method.Parameters.Add(new ParameterDefinition(parameterDefinition.Name, parameterDefinition.Attributes, parameterDefinition.ParameterType));
						}

						ParameterDefinition lazyLoaderParameter = new ParameterDefinition("lazyLoader", ParameterAttributes.None, references.LazyLoaderType);
						method.Parameters.Add(lazyLoaderParameter);

						typeDefinition.Methods.Add(method);

						ILProcessorContext context = method.Body.GetILProcessor().Start();

						context = context.Append(x => x.Create(OpCodes.Ldarg_0));

						foreach (ParameterDefinition parameterDefinition in methodDefinition.Parameters)
						{
							context = context.Append(x => x.Create(OpCodes.Ldarg, parameterDefinition.Index + 1));
						}

						context = context.Append(x => x.Create(OpCodes.Call, methodDefinition));

						context = context.Append(x => x.Create(OpCodes.Ldarg_0))
							.Append(x => x.Create(OpCodes.Ldarg, lazyLoaderParameter))
							.Append(x => x.Create(OpCodes.Stfld, lazyLoaderField))
							.Append(x => x.Create(OpCodes.Ret));
					}

					foreach (PropertyDefinition typeDefinitionProperty in typeDefinition.Properties.Where(x => x.SetMethod == null))
					{
						bool isReadOnlyCollectionType = typeDefinitionProperty.PropertyType.GetElementType().Resolve() == references.ReadOnlyCollectionType.Resolve();
						bool isReadOnlyCollectionInterface = typeDefinitionProperty.PropertyType.GetElementType().Resolve() == references.ReadOnlyCollectionInterface.Resolve();

						if (!isReadOnlyCollectionType && !isReadOnlyCollectionInterface)
						{
							continue;
						}

						FieldDefinition fieldDefinition = typeDefinition.Fields.SingleOrDefault(x =>
							x.Name.ToLower() == typeDefinitionProperty.Name.ToLower() ||
							x.Name.ToLower() == $"_{typeDefinitionProperty.Name.ToLower()}" ||
							x.Name.ToLower() == $"m_{typeDefinitionProperty.Name.ToLower()}");

						if (fieldDefinition == null)
						{
							continue;
						}

						fieldDefinition.Attributes &= ~FieldAttributes.InitOnly;

						////ldarg.0
						////IL_0002: ldfld      class [mscorlib] System.Action`2<object,string> SpatialFocus.EFLazyLoading.Tests.Assembly.Customer2::lazyLoader
						////IL_0007:  dup
						////IL_0008:  brtrue.s IL_000d
						////IL_000a:  pop
						////IL_000b:  br.s IL_001e
						////IL_000d:  ldarg.0
						////IL_000e:  ldfld class [mscorlib] System.Collections.Generic.List`1<class SpatialFocus.EFLazyLoading.Tests.Assembly.Product> SpatialFocus.EFLazyLoading.Tests.Assembly.Customer2::products
						////IL_0013:  ldstr      "Product"
						////IL_0018:  callvirt instance void class [mscorlib] System.Action`2<object,string>::Invoke(!0,
						////	!1)

						foreach (MethodDefinition methodDefinition in typeDefinition.Methods.Where(m => !m.IsSpecialName && !m.IsConstructor && m.Body.Instructions.Any(x => x.Operand == fieldDefinition)))
						{
							WriteWarning($"Weaving {typeDefinition.Name}:{methodDefinition.Name}");

							ILProcessorContext context = methodDefinition.Body.GetILProcessor().Start();

							var loadInstructionStart = context
								.Prepend(x => x.Create(OpCodes.Ldarg_0));

							var loadInstructionEnd = loadInstructionStart
								.Append(x => x.Create(OpCodes.Ldstr, typeDefinitionProperty.Name))
								.Append(x => x.Create(OpCodes.Callvirt, references.LazyLoaderInvokeMethod));

							loadInstructionStart.Prepend(x => x.Create(OpCodes.Ldarg_0))
								.Append(x => x.Create(OpCodes.Ldfld, lazyLoaderField))
								.Append(x => x.Create(OpCodes.Dup))
								.Append(x => x.Create(OpCodes.Brtrue_S, loadInstructionStart.CurrentInstruction))
								.Append(x => x.Create(OpCodes.Pop))
								.Append(x => x.Create(OpCodes.Br_S, loadInstructionEnd.CurrentInstruction!.Next));
						}
					}
				}
			}
		}

		public override IEnumerable<string> GetAssembliesForScanning()
		{
			yield return "netstandard";
			yield return "mscorlib";
			yield return "SpatialFocus.EFLazyLoading";
		}
	}
}