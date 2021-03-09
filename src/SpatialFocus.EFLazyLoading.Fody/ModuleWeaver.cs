// <copyright file="ModuleWeaver.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace SpatialFocus.EFLazyLoading.Fody
{
	using System.Collections.Generic;
	using System.Linq;
	using global::Fody;

	public partial class ModuleWeaver : BaseModuleWeaver
	{
		public override void Execute()
		{
			References references = Fody.References.Init(this);

			foreach (ClassWeavingContext classWeavingContext in this.GetWeavingCandidates(references, new Namespaces(this)))
			{
				ICollection<NavigationPropertyWeavingContext> navigationPropertyWeavingContexts = classWeavingContext.GetNavigationPropertyCandidates();

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
	}
}