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
				////options.LogTo(Console.WriteLine);

				options.UseSqlite("DataSource=local.db").UseLazyLoadingProxies();
			}, ServiceLifetime.Transient);

			var serviceProvider = services.BuildServiceProvider();

			Program.CreateDatabase(serviceProvider.CreateScope().ServiceProvider);
			Program.AddCustomer(serviceProvider.CreateScope().ServiceProvider);
			Program.GetCustomerAndAddBranches(serviceProvider.CreateScope().ServiceProvider);
			Program.GetCustomerCount(serviceProvider.CreateScope().ServiceProvider);
			Program.GetCustomerAndDeleteBranches(serviceProvider.CreateScope().ServiceProvider);
			Program.GetCustomerCount(serviceProvider.CreateScope().ServiceProvider);
		}

		private static void AddCustomer(IServiceProvider serviceProvider)
		{
			using SampleContext context = serviceProvider.GetRequiredService<SampleContext>();

			Console.WriteLine("Add new customer and branch 1");

			Customer customer = new Customer("Customer1");
			customer.AddBranch(new Branch("Branch1"));

			context.Customers.Add(customer);
			context.SaveChanges();
		}

		private static void CreateDatabase(IServiceProvider serviceProvider)
		{
			using SampleContext context = serviceProvider.GetRequiredService<SampleContext>();

			context.Database.EnsureDeleted();
			context.Database.EnsureCreated();
		}

		private static void GetCustomerAndAddBranches(IServiceProvider serviceProvider)
		{
			using SampleContext context = serviceProvider.GetRequiredService<SampleContext>();

			Console.WriteLine("Add branch 2 for existing customer");

			Customer customer = context.Customers.Single();
			customer.AddBranch(new Branch("Branch2"));

			context.SaveChanges();
		}

		private static void GetCustomerAndDeleteBranches(IServiceProvider serviceProvider)
		{
			using SampleContext context = serviceProvider.GetRequiredService<SampleContext>();

			Customer customer = context.Customers.Single();
			customer.DeleteBranches();

			context.SaveChanges();
		}

		private static void GetCustomerCount(IServiceProvider serviceProvider)
		{
			using SampleContext context = serviceProvider.GetRequiredService<SampleContext>();

			Customer customer = context.Customers.Single();

			Console.WriteLine($"Customer has {customer.BranchesCount} branches");

			context.SaveChanges();
		}
	}
}