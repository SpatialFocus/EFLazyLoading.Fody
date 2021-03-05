// <copyright file="Branch.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace SpatialFocus.EFLazyLoading.Tests.Assembly
{
	public class Branch
	{
		public Branch(string name)
		{
			Name = name;
		}

		public int Id { get; protected set; }

		public string Name { get; protected set; }
	}
}