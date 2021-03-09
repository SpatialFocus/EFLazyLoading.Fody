// <copyright file="CustomerWithInjectedLazyLoader.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace SpatialFocus.EFLazyLoading.Tests.Assembly
{
	using System;
	using System.Collections.Generic;

	public class CustomerWithInjectedLazyLoader
	{
		private readonly Action<object, string>? lazyLoader;
		private readonly List<Order> orders = new();

		public CustomerWithInjectedLazyLoader(string name, Action<object, string> lazyLoader)
		{
			Name = name;
			this.lazyLoader = lazyLoader;
		}

		public int NumberOfOrders => this.orders.Count;

		public virtual IReadOnlyCollection<Order> Orders => this.orders.AsReadOnly();

		public int Id { get; protected set; }

		public string Name { get; protected set; }

		public void AddOrder(Order order) => this.orders.Add(order);
	}
}