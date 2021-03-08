// <copyright file="Customer.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace SpatialFocus.EFLazyLoading.Tests.Assembly
{
	using System.Collections.Generic;

	public class Customer
	{
		private readonly List<Branch> branches = new();

		public Customer(string name)
		{
			Name = name;
		}

		public virtual IReadOnlyCollection<Branch> Branches => this.branches.AsReadOnly();

		public int Id { get; protected set; }

		public string Name { get; protected set; }

		public void AddBranch(Branch branch) => this.branches.Add(branch);

		public void DeleteBranches() => this.branches.Clear();
	}
}