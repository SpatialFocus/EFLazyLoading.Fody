// <copyright file="ClassWeavingContextExtension.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace SpatialFocus.EFLazyLoading.Fody
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Mono.Cecil;

	public static class ClassWeavingContextExtension
	{
		public static ICollection<NavigationPropertyWeavingContext> GetNavigationPropertyCandidates(this ClassWeavingContext context)
		{
			return context.TypeDefinition.Properties.Select(propertyDefinition =>
				{
					if (propertyDefinition.SetMethod != null)
					{
						return null;
					}

					bool isReadOnlyCollectionType = propertyDefinition.PropertyType.GetElementType().Resolve() ==
						context.References.ReadOnlyCollectionType.Resolve();
					bool isReadOnlyCollectionInterface = propertyDefinition.PropertyType.GetElementType().Resolve() ==
						context.References.ReadOnlyCollectionInterface.Resolve();

					if (!isReadOnlyCollectionType && !isReadOnlyCollectionInterface)
					{
						return null;
					}

					GenericInstanceType instance = (GenericInstanceType)propertyDefinition.PropertyType;
					TypeDefinition? genericArgument = instance.GenericArguments.FirstOrDefault()?.Resolve();

					if (genericArgument == null || !genericArgument.IsClass || genericArgument.IsEnum)
					{
						return null;
					}

					string propertyName = propertyDefinition.Name;

					FieldDefinition? fieldDefinition = context.TypeDefinition.Fields.SingleOrDefault(fieldDefinition =>
						string.Equals(fieldDefinition.Name, propertyName, StringComparison.OrdinalIgnoreCase) ||
						string.Equals(fieldDefinition.Name, $"_{propertyName}", StringComparison.OrdinalIgnoreCase) ||
						string.Equals(fieldDefinition.Name, $"m_{propertyName}", StringComparison.OrdinalIgnoreCase));

					if (fieldDefinition == null)
					{
						return null;
					}

					return new NavigationPropertyWeavingContext(context, propertyDefinition, fieldDefinition);
				})
				.Where(x => x != null)
				.ToList()!;
		}
	}
}