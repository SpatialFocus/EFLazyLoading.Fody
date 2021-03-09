// <copyright file="ClassWeavingContext.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace SpatialFocus.EFLazyLoading.Fody
{
	using Mono.Cecil;

	public class ClassWeavingContext
	{
		private readonly ModuleWeaver moduleWeaver;

		public ClassWeavingContext(ModuleWeaver moduleWeaver, TypeDefinition typeDefinition, References references)
		{
			this.moduleWeaver = moduleWeaver;
			TypeDefinition = typeDefinition;
			References = references;
		}

		public References References { get; }

		public TypeDefinition TypeDefinition { get; }

		public FieldDefinition? LazyLoaderField { get; set; }

		public virtual void WriteDebug(string message) => this.moduleWeaver.WriteDebug(message);

		public virtual void WriteInfo(string message) => this.moduleWeaver.WriteInfo(message);

		public virtual void WriteError(string message) => this.moduleWeaver.WriteError(message);

		public virtual void WriteWarning(string message) => this.moduleWeaver.WriteWarning(message);
	}
}