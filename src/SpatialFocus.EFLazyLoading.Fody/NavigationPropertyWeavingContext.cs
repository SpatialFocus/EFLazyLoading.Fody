// <copyright file="NavigationPropertyWeavingContext.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace SpatialFocus.EFLazyLoading.Fody
{
	using Mono.Cecil;

	public class NavigationPropertyWeavingContext
	{
		public NavigationPropertyWeavingContext(ClassWeavingContext classWeavingContext, PropertyDefinition propertyDefinition,
			FieldDefinition fieldDefinition)
		{
			ClassWeavingContext = classWeavingContext;
			PropertyDefinition = propertyDefinition;
			FieldDefinition = fieldDefinition;
		}

		public ClassWeavingContext ClassWeavingContext { get; }

		public FieldDefinition FieldDefinition { get; }

		public PropertyDefinition PropertyDefinition { get; }

		public virtual void WriteDebug(string message) => ClassWeavingContext.WriteDebug(message);

		public virtual void WriteError(string message) => ClassWeavingContext.WriteError(message);

		public virtual void WriteInfo(string message) => ClassWeavingContext.WriteInfo(message);

		public virtual void WriteWarning(string message) => ClassWeavingContext.WriteWarning(message);
	}
}