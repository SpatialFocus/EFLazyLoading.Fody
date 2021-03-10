// <copyright file="CustomerWithReadOnlyCollection.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace SpatialFocus.EFLazyLoading.Tests.Assembly
{
	using System.Collections.Generic;
	using System.Collections.ObjectModel;

	public class CustomerWithReadOnlyCollection
	{
		private readonly List<Order> orders = new();

		public CustomerWithReadOnlyCollection(string name)
		{
			Name = name;
		}

		public int NumberOfOrders => this.orders.Count;

		// Using class instead of interface IReadOnlyCollection
		public virtual ReadOnlyCollection<Order> Orders => this.orders.AsReadOnly();

		public int Id { get; protected set; }

		public string Name { get; protected set; }

		public void AddOrder(Order order) => this.orders.Add(order);
	}
}