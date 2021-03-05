// <copyright file="ModuleWeaver.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace SpatialFocus.EFLazyLoading.Fody
{
	using System.Collections.Generic;
	using global::Fody;

	public partial class ModuleWeaver : BaseModuleWeaver
	{
		public override void Execute()
		{
		}

		public override IEnumerable<string> GetAssembliesForScanning()
		{
			yield return "netstandard";
			yield return "mscorlib";
			yield return "SpatialFocus.EFLazyLoading";
		}
	}
}