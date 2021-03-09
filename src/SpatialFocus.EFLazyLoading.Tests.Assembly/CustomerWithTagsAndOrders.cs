// <copyright file="CustomerWithTagsAndOrders.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace SpatialFocus.EFLazyLoading.Tests.Assembly
{
	using System.Collections.Generic;

	public class CustomerWithTagsAndOrders
	{
		private readonly List<Order> orders = new();
		private readonly List<Tag> tags = new();

		public CustomerWithTagsAndOrders(string name)
		{
			Name = name;
		}

		public int NumberOfOrders => this.orders.Count;

		public virtual IReadOnlyCollection<Order> Orders => this.orders.AsReadOnly();

		public virtual IReadOnlyCollection<Tag> Tags => this.tags.AsReadOnly();

		public int Id { get; protected set; }

		public string Name { get; protected set; }

		public void AddOrder(Order order) => this.orders.Add(order);

		public void AddTag(Tag tag) => this.tags.Add(tag);
	}
}