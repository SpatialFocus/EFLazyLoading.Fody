// <copyright file="Customer.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace SpatialFocus.EFLazyLoading.Sample.Data
{
	using System;
	using System.Collections.Generic;

	public class Customer
	{
		public List<Branch> branches = new();
		private readonly Action<object, string> lazyLoader;

		public Customer(string name, Action<object, string> lazyLoader) : this(name)
		{
			this.lazyLoader = lazyLoader;
		}

		public Customer(string name)
		{
			Name = name;
		}

		public virtual IReadOnlyCollection<Branch> Branches => this.branches.AsReadOnly();

		public int Id { get; protected set; }

		public string Name { get; protected set; }

		public void AddBranch(Branch branch)
		{
			Console.WriteLine($"Executing {nameof(Customer.AddBranch)}: {nameof(this.branches)} count: {this.branches.Count}");

			this.branches.Add(branch);
		}

		public void DeleteBranches()
		{
			Console.WriteLine($"Executing {nameof(Customer.DeleteBranches)}: {nameof(this.branches)} count: {this.branches.Count}");

			this.branches.Clear();
		}
	}
}