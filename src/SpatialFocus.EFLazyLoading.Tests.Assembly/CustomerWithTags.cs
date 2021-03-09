// <copyright file="CustomerWithTags.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace SpatialFocus.EFLazyLoading.Tests.Assembly
{
	using System.Collections.Generic;

	// Lazy loader should only be injected for entities (classes), not for enums
	public class CustomerWithTags
	{
		private readonly List<Tag> tags = new();

		public CustomerWithTags(string name)
		{
			Name = name;
		}

		public virtual IReadOnlyCollection<Tag> Tags => this.tags.AsReadOnly();

		public int Id { get; protected set; }

		public string Name { get; protected set; }

		public void AddTag(Tag tag) => this.tags.Add(tag);
	}
}