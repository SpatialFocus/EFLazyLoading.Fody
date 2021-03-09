// <copyright file="Order.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace SpatialFocus.EFLazyLoading.Sample.Data
{
	public class Order
	{
		public Order(string name, decimal price)
		{
			Name = name;
			Price = price;
		}

		public int Id { get; protected set; }

		public string Name { get; protected set; }

		public decimal Price { get; protected set; }
	}
}