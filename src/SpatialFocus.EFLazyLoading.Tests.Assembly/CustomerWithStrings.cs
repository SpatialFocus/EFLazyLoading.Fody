// <copyright file="CustomerWithStrings.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace SpatialFocus.EFLazyLoading.Tests.Assembly
{
	using System.Collections.Generic;

	// Lazy loader should only be injected for entities (classes), not for strings
	public class CustomerWithStrings
	{
		private readonly List<string> tags = new();

		public CustomerWithStrings(string name)
		{
			Name = name;
		}

		public virtual IReadOnlyCollection<string> Tags => this.tags.AsReadOnly();

		public int Id { get; protected set; }

		public string Name { get; protected set; }

		public void AddTag(string tag) => this.tags.Add(tag);
	}
}