// <copyright file="ModuleWeaverExtension.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace SpatialFocus.EFLazyLoading.Fody
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Mono.Cecil;
	using Mono.Cecil.Rocks;

	public static class ModuleWeaverExtension
	{
		public static ICollection<ClassWeavingContext> GetWeavingCandidates(this ModuleWeaver moduleWeaver, References references,
			Namespaces namespaces)
		{
			if (moduleWeaver == null)
			{
				throw new ArgumentNullException(nameof(moduleWeaver));
			}

			if (references == null)
			{
				throw new ArgumentNullException(nameof(references));
			}

			TypeDefinition compilerGeneratedAttribute = references.CompilerGeneratedAttributeType.Resolve();

			return moduleWeaver.ModuleDefinition.Types.Where(typeDefinition =>
				{
					bool hasCompilerGeneratedAttribute =
						typeDefinition.CustomAttributes.Any(attribute => attribute.AttributeType.Resolve().Equals(compilerGeneratedAttribute));

					bool hasLazyLoaderConstructor = typeDefinition.GetConstructors()
						.Any(constructors => constructors.Parameters.Any(parameter =>
							string.Equals(parameter.Name, "lazyLoader", StringComparison.OrdinalIgnoreCase)));
					bool hasLazyLoadingField =
						typeDefinition.Fields.Any(field => string.Equals(field.Name, "lazyLoader", StringComparison.OrdinalIgnoreCase));

					if (hasCompilerGeneratedAttribute || typeDefinition.IsSpecialName || hasLazyLoaderConstructor || hasLazyLoadingField)
					{
						return false;
					}

					if (!namespaces.ShouldIncludeType(typeDefinition))
					{
						return false;
					}

					return true;
				})
				.Select(typeDefinition => new ClassWeavingContext(moduleWeaver, typeDefinition, references))
				.ToList();
		}
	}
}