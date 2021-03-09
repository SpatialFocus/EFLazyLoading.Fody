// <copyright file="WeavingTest.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace SpatialFocus.EFLazyLoading.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using global::Fody;
	using SpatialFocus.EFLazyLoading.Fody;
	using SpatialFocus.EFLazyLoading.Tests.Assembly;
	using Xunit;

	[Collection("Assembly")]
	public class WeavingTest
	{
		private static readonly TestResult TestResult;

		static WeavingTest()
		{
			WeavingTest.TestResult =
				new ModuleWeaver().ExecuteTestRun($"{typeof(Customer).Namespace}.dll", ignoreCodes: new[] { "0x80131869" });
		}

		[Fact]
		public void CanCreateInstance()
		{
			ICollection<Tuple<object, string>> lazyLoaderCalls = new List<Tuple<object, string>>();

			dynamic instance =
				TestHelpers.CreateInstance<Customer>(WeavingTest.TestResult.Assembly, "Customer1", new Action<object, string>((entity, property) => lazyLoaderCalls.Add(new Tuple<object, string>(entity, property))));
			dynamic count = instance.BranchesCount;

			Assert.True(lazyLoaderCalls.Count == 1);
			Assert.Equal("Branches", lazyLoaderCalls.Single().Item2);
		}
	}
}