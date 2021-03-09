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
		public void T01_CanCreateInstance()
		{
			ICollection<Tuple<object, string>> lazyLoaderCalls = new List<Tuple<object, string>>();

			dynamic instance = TestHelpers.CreateInstance<Customer>(WeavingTest.TestResult.Assembly, "Customer1",
				new Action<object, string>((entity, property) => lazyLoaderCalls.Add(new Tuple<object, string>(entity, property))));

			Assert.Equal(0, lazyLoaderCalls.Count);
		}

		[Fact]
		public void T02_AccessNavigationProperty()
		{
			ICollection<Tuple<object, string>> lazyLoaderCalls = new List<Tuple<object, string>>();

			dynamic instance = TestHelpers.CreateInstance<Customer>(WeavingTest.TestResult.Assembly, "Customer1",
				new Action<object, string>((entity, property) => lazyLoaderCalls.Add(new Tuple<object, string>(entity, property))));
			_ = instance.Orders.Count;

			Assert.Equal(0, lazyLoaderCalls.Count);
		}

		[Fact]
		public void T03_AddItemViaBackingFieldTriggersLazyLoading()
		{
			ICollection<Tuple<object, string>> lazyLoaderCalls = new List<Tuple<object, string>>();

			dynamic instance = TestHelpers.CreateInstance<Customer>(WeavingTest.TestResult.Assembly, "Customer1",
				new Action<object, string>((entity, property) => lazyLoaderCalls.Add(new Tuple<object, string>(entity, property))));
			dynamic order1 = TestHelpers.CreateInstance<Order>(WeavingTest.TestResult.Assembly, "Order 1", 10.99M);

			instance.AddOrder(order1);

			Assert.Equal(1, lazyLoaderCalls.Count);
			Assert.Equal("Orders", lazyLoaderCalls.Single().Item2);
		}

		[Fact]
		public void T04_AddItemAndClearCollection()
		{
			ICollection<Tuple<object, string>> lazyLoaderCalls = new List<Tuple<object, string>>();

			dynamic instance = TestHelpers.CreateInstance<Customer>(WeavingTest.TestResult.Assembly, "Customer1",
				new Action<object, string>((entity, property) => lazyLoaderCalls.Add(new Tuple<object, string>(entity, property))));
			dynamic order1 = TestHelpers.CreateInstance<Order>(WeavingTest.TestResult.Assembly, "Order 1", 10.99M);

			instance.AddOrder(order1);
			instance.ClearOrders();

			Assert.Equal(2, lazyLoaderCalls.Count);
			Assert.Equal("Orders", lazyLoaderCalls.First().Item2);
			Assert.Equal("Orders", lazyLoaderCalls.Last().Item2);
		}

		[Fact]
		public void T05_AccessExpressionBodyProperty()
		{
			ICollection<Tuple<object, string>> lazyLoaderCalls = new List<Tuple<object, string>>();

			dynamic instance = TestHelpers.CreateInstance<Customer>(WeavingTest.TestResult.Assembly, "Customer1",
				new Action<object, string>((entity, property) => lazyLoaderCalls.Add(new Tuple<object, string>(entity, property))));

			_ = instance.NumberOfOrders;

			Assert.Equal(1, lazyLoaderCalls.Count);
			Assert.Equal("Orders", lazyLoaderCalls.Single().Item2);
		}
	}
}