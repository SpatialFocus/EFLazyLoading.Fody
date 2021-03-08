// <copyright file="References.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace SpatialFocus.EFLazyLoading.Fody
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.ComponentModel.Design;
	using System.Linq;
	using System.Runtime.CompilerServices;
	using Mono.Cecil;
	using Mono.Cecil.Rocks;


	public class References
	{
		protected References(ModuleWeaver moduleWeaver)
		{
			ModuleWeaver = moduleWeaver;
		}

		public TypeReference CompilerGeneratedAttributeType { get; set; }

		public TypeReference LazyLoaderType { get; set; }

		public MethodReference LazyLoaderInvokeMethod { get; set; }

		public TypeReference ReadOnlyCollectionInterface { get; set; }

		public TypeReference ReadOnlyCollectionType { get; set; }

		protected ModuleWeaver ModuleWeaver { get; }

		public static References Init(ModuleWeaver moduleWeaver)
		{
			if (moduleWeaver == null)
			{
				throw new ArgumentNullException(nameof(moduleWeaver));
			}

			References references = new References(moduleWeaver);

			TypeDefinition compilerGeneratedAttributeType = moduleWeaver.FindTypeDefinition(typeof(CompilerGeneratedAttribute).FullName);
			references.CompilerGeneratedAttributeType = moduleWeaver.ModuleDefinition.ImportReference(compilerGeneratedAttributeType);

			TypeDefinition lazyLoaderType = moduleWeaver.FindTypeDefinition(typeof(Action<,>).Name);
			references.LazyLoaderType = moduleWeaver.ModuleDefinition.ImportReference(lazyLoaderType)
				.MakeGenericInstanceType(moduleWeaver.TypeSystem.ObjectDefinition, moduleWeaver.TypeSystem.StringReference);

			MethodDefinition lazyLoaderInvokeMethod = references.LazyLoaderType.Resolve().Methods.Single(x => x.Name == "Invoke");
			references.LazyLoaderInvokeMethod = moduleWeaver.ModuleDefinition.ImportReference(lazyLoaderInvokeMethod).MakeHostInstanceGeneric(moduleWeaver.TypeSystem.ObjectDefinition, moduleWeaver.TypeSystem.StringReference);

			TypeDefinition readOnlyCollectionType = moduleWeaver.FindTypeDefinition(typeof(ReadOnlyCollection<>).Name);
			references.ReadOnlyCollectionType = moduleWeaver.ModuleDefinition.ImportReference(readOnlyCollectionType);

			TypeDefinition readOnlyCollectionInterface = moduleWeaver.FindTypeDefinition(typeof(IReadOnlyCollection<>).Name);
			references.ReadOnlyCollectionInterface = moduleWeaver.ModuleDefinition.ImportReference(readOnlyCollectionInterface);

			return references;
		}
	}
}