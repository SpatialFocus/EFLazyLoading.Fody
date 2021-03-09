// <copyright file="Program.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace SpatialFocus.EFLazyLoading.Sample
{
	using System;
	using System.Linq;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.DependencyInjection;
	using SpatialFocus.EFLazyLoading.Sample.Data;

	public class Program
	{
		public static void Main(string[] args)
		{
			var services = new ServiceCollection();
			services.AddDbContext<SampleContext>(options =>
			{
				options.UseSqlite("DataSource=local.db").UseLazyLoadingProxies();
			}, ServiceLifetime.Transient);

			var serviceProvider = services.BuildServiceProvider();

			Program.CreateDatabase(serviceProvider.CreateScope().ServiceProvider);

			Program.AddCustomerAndOrder(serviceProvider.CreateScope().ServiceProvider);
			Program.GetOrderCount(serviceProvider.CreateScope().ServiceProvider);

			Program.GetCustomerAndAddOrder(serviceProvider.CreateScope().ServiceProvider);
			Program.GetOrderCount(serviceProvider.CreateScope().ServiceProvider);

			Program.GetCustomerAndRemoveOrder(serviceProvider.CreateScope().ServiceProvider);
			Program.GetOrderCount(serviceProvider.CreateScope().ServiceProvider);

			Program.GetCustomerAndClearOrders(serviceProvider.CreateScope().ServiceProvider);
			Program.GetOrderCount(serviceProvider.CreateScope().ServiceProvider);
		}

		private static void AddCustomerAndOrder(IServiceProvider serviceProvider)
		{
			using SampleContext context = serviceProvider.GetRequiredService<SampleContext>();

			Console.WriteLine("Add new customer and order 1");

			Customer customer = new Customer("Customer1");
			customer.AddOrder(new Order("Order 1", 10.99M));

			context.Customers.Add(customer);
			context.SaveChanges();
		}

		private static void CreateDatabase(IServiceProvider serviceProvider)
		{
			using SampleContext context = serviceProvider.GetRequiredService<SampleContext>();

			context.Database.EnsureDeleted();
			context.Database.EnsureCreated();
		}

		private static void GetCustomerAndAddOrder(IServiceProvider serviceProvider)
		{
			using SampleContext context = serviceProvider.GetRequiredService<SampleContext>();

			Console.WriteLine("Add order 2 and order 3 to existing customer");

			Customer customer = context.Customers.Single();
			customer.AddOrder(new Order("Order 2", 7.99M));
			customer.AddOrder(new Order("Order 3", 14.99M));

			context.SaveChanges();
		}

		private static void GetCustomerAndClearOrders(IServiceProvider serviceProvider)
		{
			using SampleContext context = serviceProvider.GetRequiredService<SampleContext>();

			Console.WriteLine("Clear orders of existing customer");

			Customer customer = context.Customers.Single();
			customer.ClearOrders();

			context.SaveChanges();
		}

		private static void GetCustomerAndRemoveOrder(IServiceProvider serviceProvider)
		{
			using SampleContext context = serviceProvider.GetRequiredService<SampleContext>();

			Console.WriteLine("Remove order 1 from existing customer");

			Customer customer = context.Customers.Single();
			Order order1 = context.Orders.Single(x => x.Name == "Order 1");
			customer.RemoveOrder(order1);

			context.SaveChanges();
		}

		private static void GetOrderCount(IServiceProvider serviceProvider)
		{
			using SampleContext context = serviceProvider.GetRequiredService<SampleContext>();

			Customer customer = context.Customers.Single();

			Console.WriteLine($" => Customer has {customer.NumberOfOrders} orders.");

			context.SaveChanges();
		}
	}
}