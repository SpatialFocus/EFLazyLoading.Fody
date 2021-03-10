// <copyright file="CustomerWeaved.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace SpatialFocus.EFLazyLoading.Sample.Data
{
	using System;
	using System.Collections.Generic;

	public class CustomerWeaved
	{
		private readonly Action<object, string>? lazyLoader;
		private readonly List<Order> orders = new();

		public CustomerWeaved(string name)
		{
			Name = name;
		}

		// For every constructor a constructor overload with Action<object, string> lazyLoader will be added
		// See https://docs.microsoft.com/en-us/ef/core/querying/related-data/lazy#lazy-loading-without-proxies
		protected CustomerWeaved(string name, Action<object, string> lazyLoader) : this(name)
		{
			this.lazyLoader = lazyLoader;
		}

		public int NumberOfOrders
		{
			get
			{
				this.lazyLoader?.Invoke(this, "Orders");
				return this.orders.Count;
			}
		}

		// Access via navigation property will trigger default lazy loading behaviour
		public virtual IReadOnlyCollection<Order> Orders => this.orders.AsReadOnly();

		public int Id { get; protected set; }

		public string Name { get; protected set; }

		public void AddOrder(Order order)
		{
			this.lazyLoader?.Invoke(this, "Orders");
			this.orders.Add(order);
		}

		public void ClearOrders()
		{
			this.lazyLoader?.Invoke(this, "Orders");
			this.orders.Clear();
		}

		public void RemoveOrder(Order order)
		{
			this.lazyLoader?.Invoke(this, "Orders");
			this.orders.Remove(order);
		}
	}
}