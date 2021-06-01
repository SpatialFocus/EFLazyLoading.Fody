// <copyright file="Customer.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace SpatialFocus.EFLazyLoading.Tests.Assembly
{
	using System.Collections.Generic;

	public class Customer
	{
		private readonly List<Order> orders = new();

		public Customer(string name)
		{
			Name = name;
		}

		public int NumberOfOrders => this.orders.Count;

		public virtual IReadOnlyCollection<Order> Orders => this.orders.AsReadOnly();

		public int Id { get; protected set; }

		public string Name { get; protected set; }

		public void AddOrder(Order order) => this.orders.Add(order);

		public void ClearOrders() => this.orders.Clear();

		public void RemoveOrder(Order order) => this.orders.Remove(order);

		public class Nested
		{
			private readonly Customer customer;

			public Nested(Customer customer)
			{
				this.customer = customer;
			}

			public virtual IReadOnlyCollection<Order> Orders => this.customer.orders.AsReadOnly();
		}
	}
}