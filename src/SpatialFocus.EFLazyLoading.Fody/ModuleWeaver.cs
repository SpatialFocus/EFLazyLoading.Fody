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

				bool hasLazyLoaderConstructor = typeDefinition.GetConstructors().Any(x => x.Parameters.Any(x => x.Name == "lazyLoader"));

				if (!hasLazyLoaderConstructor)
				{
					foreach (MethodDefinition methodDefinition in typeDefinition.GetConstructors())
					{
						MethodDefinition method = new MethodDefinition(methodDefinition.Name, methodDefinition.Attributes, ModuleDefinition.TypeSystem.Void);

						foreach (ParameterDefinition parameterDefinition in methodDefinition.Parameters)
						{
							method.Parameters.Add(new ParameterDefinition(parameterDefinition.Name, parameterDefinition.Attributes, parameterDefinition.ParameterType));
						}

						method.Parameters.Add(new ParameterDefinition("lazyLoader", ParameterAttributes.None, references.LazyLoaderType));

						method.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));

						typeDefinition.Methods.Add(method);
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