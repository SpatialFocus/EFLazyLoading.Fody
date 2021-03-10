// <copyright file="LazyLoading.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace SpatialFocus.EFLazyLoading.Fody
{
	using System.Collections.Generic;
	using System.Linq;
	using Mono.Cecil;
	using Mono.Cecil.Cil;
	using Mono.Cecil.Rocks;

	public static class LazyLoading
	{
		public static void AddConstructorOverloads(this ClassWeavingContext context)
		{
			foreach (MethodDefinition constructor in context.TypeDefinition.GetConstructors().ToList())
			{
				context.WriteDebug(
					$"Add constructor overload for {context.TypeDefinition.Name}({string.Join(", ", constructor.Parameters.Select(x => $"{x.ParameterType.Name} {x.Name}"))})");

				MethodDefinition method = new MethodDefinition(constructor.Name, constructor.Attributes,
					context.TypeDefinition.Module.TypeSystem.Void)
				{
					IsFamily = true,
				};

				foreach (ParameterDefinition parameterDefinition in constructor.Parameters)
				{
					method.Parameters.Add(new ParameterDefinition(parameterDefinition.Name, parameterDefinition.Attributes,
						parameterDefinition.ParameterType));
				}

				ParameterDefinition lazyLoaderParameter =
					new ParameterDefinition("lazyLoader", ParameterAttributes.None, context.References.LazyLoaderType);
				method.Parameters.Add(lazyLoaderParameter);

				context.TypeDefinition.Methods.Add(method);

				ILProcessorContext processor = method.Body.GetILProcessor().Start();

				processor = processor.Append(x => x.Create(OpCodes.Ldarg_0));

				foreach (ParameterDefinition parameterDefinition in constructor.Parameters)
				{
					processor = processor.Append(x => x.Create(OpCodes.Ldarg, parameterDefinition.Index + 1));
				}

				processor = processor.Append(x => x.Create(OpCodes.Call, constructor));

				processor = processor.Append(x => x.Create(OpCodes.Ldarg_0))
					.Append(x => x.Create(OpCodes.Ldarg, lazyLoaderParameter))
					.Append(x => x.Create(OpCodes.Stfld, context.LazyLoaderField))
					.Append(x => x.Create(OpCodes.Ret));
			}
		}

		public static void AddFieldDefinition(this ClassWeavingContext context)
		{
			context.WriteDebug($"Add lazy loader field to {context.TypeDefinition.Name}");

			context.LazyLoaderField = new FieldDefinition("lazyLoader", FieldAttributes.Private | FieldAttributes.InitOnly,
				context.References.LazyLoaderType);
			context.TypeDefinition.Fields.Add(context.LazyLoaderField);
		}

		public static void AddLazyLoadingToReferencingMethods(this NavigationPropertyWeavingContext context)
		{
			IEnumerable<MethodDefinition> getterAndSetterMethods = context.ClassWeavingContext.TypeDefinition.Properties
				.Where(propertyDefinition => propertyDefinition != context.PropertyDefinition)
				.SelectMany(propertyDefinition => new[] { propertyDefinition.SetMethod, propertyDefinition.GetMethod })
				.Where(methodDefinition => methodDefinition != null &&
					methodDefinition.Body.Instructions.Any(body => body.Operand == context.FieldDefinition));

			IEnumerable<MethodDefinition> methods = context.ClassWeavingContext.TypeDefinition.Methods.Where(methodDefinition =>
				!methodDefinition.IsSpecialName && !methodDefinition.IsGetter && !methodDefinition.IsSetter &&
					!methodDefinition.IsConstructor && methodDefinition.Body.Instructions.Any(x => x.Operand == context.FieldDefinition));

			foreach (MethodDefinition methodDefinition in getterAndSetterMethods.Union(methods))
			{
				context.WriteDebug($"Add lazy loading to method {context.ClassWeavingContext.TypeDefinition.Name}:{methodDefinition.Name}");

				ILProcessorContext processor = methodDefinition.Body.GetILProcessor().Start();

				var loadInstructionStart = processor.Prepend(x => x.Create(OpCodes.Ldarg_0));

				var loadInstructionEnd = loadInstructionStart.Append(x => x.Create(OpCodes.Ldstr, context.PropertyDefinition.Name))
					.Append(x => x.Create(OpCodes.Callvirt, context.ClassWeavingContext.References.LazyLoaderInvokeMethod));

				loadInstructionStart.Prepend(x => x.Create(OpCodes.Ldarg_0))
					.Append(x => x.Create(OpCodes.Ldfld, context.ClassWeavingContext.LazyLoaderField))
					.Append(x => x.Create(OpCodes.Dup))
					.Append(x => x.Create(OpCodes.Brtrue_S, loadInstructionStart.CurrentInstruction))
					.Append(x => x.Create(OpCodes.Pop))
					.Append(x => x.Create(OpCodes.Br_S, loadInstructionEnd.CurrentInstruction!.Next));
			}
		}
	}
}