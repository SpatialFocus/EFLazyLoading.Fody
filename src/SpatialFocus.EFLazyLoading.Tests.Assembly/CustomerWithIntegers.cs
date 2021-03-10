// <copyright file="CustomerWithIntegers.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace SpatialFocus.EFLazyLoading.Tests.Assembly
{
	using System.Collections.Generic;

	// Lazy loader should only be injected for entities (classes), not for integers
	public class CustomerWithIntegers
	{
		private readonly List<int> tags = new();

		public CustomerWithIntegers(string name)
		{
			Name = name;
		}

		public virtual IReadOnlyCollection<int> Tags => this.tags.AsReadOnly();

		public int Id { get; protected set; }

		public string Name { get; protected set; }

		public void AddTag(int tag) => this.tags.Add(tag);
	}
}