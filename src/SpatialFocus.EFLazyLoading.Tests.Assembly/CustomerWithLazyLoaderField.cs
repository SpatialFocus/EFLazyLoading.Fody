// <copyright file="OtherCustomer.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace SpatialFocus.EFLazyLoading.Tests.Assembly
{
	using System;
	using System.Collections.Generic;

	public class CustomerWithLazyLoaderField
	{
#pragma warning disable 169
		private readonly Action<object, string>? lazyLoader;
#pragma warning restore 169

		private readonly List<Order> orders = new();

		public CustomerWithLazyLoaderField(string name)
		{
			Name = name;
		}

		public virtual IReadOnlyCollection<Order> Orders => this.orders.AsReadOnly();

		public int Id { get; protected set; }

		public string Name { get; protected set; }

		public void AddOrder(Order order) => this.orders.Add(order);
	}
}