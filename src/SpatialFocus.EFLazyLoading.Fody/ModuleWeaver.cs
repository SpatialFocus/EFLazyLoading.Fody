// <copyright file="ModuleWeaver.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace SpatialFocus.EFLazyLoading.Fody
{
	using System.Collections.Generic;
	using System.Linq;
	using global::Fody;
	using Mono.Cecil;
	using Mono.Cecil.Cil;

	public partial class ModuleWeaver : BaseModuleWeaver
	{
		public override void Execute()
		{
			References references = Fody.References.Init(this);

			TypeDefinition extension = CreateExtensionClass(references);
			ModuleDefinition.Types.Add(extension);
			references.ExtensionLoadMethod = ModuleDefinition.ImportReference(extension.Methods.Single());

			foreach (ClassWeavingContext classWeavingContext in this.GetWeavingCandidates(references, new Namespaces(this)))
			{
				ICollection<NavigationPropertyWeavingContext> navigationPropertyWeavingContexts =
					classWeavingContext.GetNavigationPropertyCandidates();

				if (!navigationPropertyWeavingContexts.Any())
				{
					WriteDebug($"Skipping type {classWeavingContext.TypeDefinition.Name} because no eligible properties found");
					continue;
				}

				WriteDebug($"Weaving type {classWeavingContext.TypeDefinition.Name}");

				classWeavingContext.AddFieldDefinition();
				classWeavingContext.AddConstructorOverloads();

				foreach (NavigationPropertyWeavingContext navigationPropertyWeavingContext in navigationPropertyWeavingContexts)
				{
					navigationPropertyWeavingContext.AddLazyLoadingToReferencingMethods();
				}
			}
		}

		public override IEnumerable<string> GetAssembliesForScanning()
		{
			yield return "netstandard";
			yield return "mscorlib";
			yield return "SpatialFocus.EFLazyLoading";
		}

		protected TypeDefinition CreateExtensionClass(References references)
		{
			var extension = new TypeDefinition("SpatialFocus.EFLazyLoading", "LazyLoadingExtensions",
				TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.AutoClass |
				TypeAttributes.AnsiClass | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit, TypeSystem.ObjectReference);

			MethodDefinition loadMethodDefinition = new MethodDefinition("Load",
				MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static, TypeSystem.VoidDefinition);

			GenericParameter genericParameter = new GenericParameter(loadMethodDefinition) { Name = "TEntity" };
			loadMethodDefinition.ReturnType = genericParameter;

			loadMethodDefinition.GenericParameters.Add(genericParameter);

			loadMethodDefinition.Parameters.Add(new ParameterDefinition("instance", ParameterAttributes.None, genericParameter));
			loadMethodDefinition.Parameters.Add(new ParameterDefinition("loader", ParameterAttributes.None, references.LazyLoaderType));
			loadMethodDefinition.Parameters.Add(new ParameterDefinition("name", ParameterAttributes.None, TypeSystem.StringDefinition));

			var loadInstructionStart = loadMethodDefinition.Body.GetILProcessor()
				.Start()
				.Append(x => x.Create(OpCodes.Ldarg_1));

			var loadInstructionEnd = loadInstructionStart.Append(x => x.Create(OpCodes.Ldarg_1))
				.Append(x => x.Create(OpCodes.Ldarg_0))
				.Append(x => x.Create(OpCodes.Box, genericParameter))
				.Append(x => x.Create(OpCodes.Ldarg_2))
				.Append(x => x.Create(OpCodes.Callvirt, references.LazyLoaderInvokeMethod));

			loadInstructionEnd.Append(x => x.Create(OpCodes.Ldarg_0))
				.Append(x => x.Create(OpCodes.Ret));

			loadInstructionStart.Append(x => x.Create(OpCodes.Brtrue_S, loadInstructionStart.CurrentInstruction!.Next))
				.Append(x => x.Create(OpCodes.Br_S, loadInstructionEnd.CurrentInstruction!.Next));

			extension.Methods.Add(loadMethodDefinition);

			return extension;
		}
	}
}