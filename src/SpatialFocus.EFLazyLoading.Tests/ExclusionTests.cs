// <copyright file="ExclusionTests.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace SpatialFocus.EFLazyLoading.Tests
{
	using System;
	using System.Collections.Generic;
	using global::Fody;
	using SpatialFocus.EFLazyLoading.Fody;
	using SpatialFocus.EFLazyLoading.Tests.Assembly;
	using Xunit;

	[Collection("Assembly")]
	public class ExclusionTests
	{
		private static readonly TestResult TestResult;

		static ExclusionTests()
		{
			ExclusionTests.TestResult =
				new ModuleWeaver().ExecuteTestRun($"{typeof(Customer).Namespace}.dll", ignoreCodes: new[] { "0x80131869" });
		}

		[Fact]
		public void E01_CanNotCreateInstance()
		{
			ICollection<Tuple<object, string>> lazyLoaderCalls = new List<Tuple<object, string>>();

			Assert.Throws<MissingMethodException>(() =>
				_ = TestHelpers.CreateInstance<CustomerWithLazyLoaderField>(ExclusionTests.TestResult.Assembly, "Customer1",
					new Action<object, string>((entity, property) => lazyLoaderCalls.Add(new Tuple<object, string>(entity, property)))));
		}

		[Fact]
		public void E02_CanCreateInstance()
		{
			ICollection<Tuple<object, string>> lazyLoaderCalls = new List<Tuple<object, string>>();

			_ = TestHelpers.CreateInstance<CustomerWithInjectedLazyLoader>(ExclusionTests.TestResult.Assembly, "Customer1",
				new Action<object, string>((entity, property) => lazyLoaderCalls.Add(new Tuple<object, string>(entity, property))));

			Assert.Equal(0, lazyLoaderCalls.Count);
		}

		[Fact]
		public void E03_LazyLoadingNotUsedInMethod()
		{
			ICollection<Tuple<object, string>> lazyLoaderCalls = new List<Tuple<object, string>>();

			dynamic instance = TestHelpers.CreateInstance<CustomerWithInjectedLazyLoader>(ExclusionTests.TestResult.Assembly, "Customer1",
				new Action<object, string>((entity, property) => lazyLoaderCalls.Add(new Tuple<object, string>(entity, property))));
			dynamic order1 = TestHelpers.CreateInstance<Order>(ExclusionTests.TestResult.Assembly, "Order 1", 10.99M);

			instance.AddOrder(order1);

			Assert.Equal(0, lazyLoaderCalls.Count);
		}

		[Fact]
		public void E04_LazyLoadingNotUsedInExpressionBodyProperty()
		{
			ICollection<Tuple<object, string>> lazyLoaderCalls = new List<Tuple<object, string>>();

			dynamic instance = TestHelpers.CreateInstance<CustomerWithInjectedLazyLoader>(ExclusionTests.TestResult.Assembly, "Customer1",
				new Action<object, string>((entity, property) => lazyLoaderCalls.Add(new Tuple<object, string>(entity, property))));
			_ = instance.NumberOfOrders;

			Assert.Equal(0, lazyLoaderCalls.Count);
		}

		[Fact]
		public void E05_CanNotCreateInstanceOfCustomerWithOnlyEnum()
		{
			ICollection<Tuple<object, string>> lazyLoaderCalls = new List<Tuple<object, string>>();

			Assert.Throws<MissingMethodException>(() => _ = TestHelpers.CreateInstance<CustomerWithTags>(ExclusionTests.TestResult.Assembly, "Customer1",
				new Action<object, string>((entity, property) => lazyLoaderCalls.Add(new Tuple<object, string>(entity, property)))));

			Assert.Equal(0, lazyLoaderCalls.Count);
		}

		[Fact]
		public void E06_CanCreateInstanceOfCustomerWithEnumAndOrders()
		{
			ICollection<Tuple<object, string>> lazyLoaderCalls = new List<Tuple<object, string>>();

			_ = TestHelpers.CreateInstance<CustomerWithTagsAndOrders>(ExclusionTests.TestResult.Assembly, "Customer1",
				new Action<object, string>((entity, property) => lazyLoaderCalls.Add(new Tuple<object, string>(entity, property))));

			Assert.Equal(0, lazyLoaderCalls.Count);
		}

		[Fact]
		public void E07_LazyLoaderNotUsedForEnum()
		{
			ICollection<Tuple<object, string>> lazyLoaderCalls = new List<Tuple<object, string>>();

			dynamic instance = TestHelpers.CreateInstance<CustomerWithTagsAndOrders>(ExclusionTests.TestResult.Assembly, "Customer1",
				new Action<object, string>((entity, property) => lazyLoaderCalls.Add(new Tuple<object, string>(entity, property))));
			dynamic tag = TestHelpers.CreateInstance<Tag>(ExclusionTests.TestResult.Assembly);

			instance.AddTag(tag);

			Assert.Equal(0, lazyLoaderCalls.Count);
		}

		[Fact]
		public void E07_LazyLoaderStillUsedForClass()
		{
			ICollection<Tuple<object, string>> lazyLoaderCalls = new List<Tuple<object, string>>();

			dynamic instance = TestHelpers.CreateInstance<CustomerWithTagsAndOrders>(ExclusionTests.TestResult.Assembly, "Customer1",
				new Action<object, string>((entity, property) => lazyLoaderCalls.Add(new Tuple<object, string>(entity, property))));
			dynamic order1 = TestHelpers.CreateInstance<Order>(ExclusionTests.TestResult.Assembly, "Order 1", 10.99M);

			instance.AddOrder(order1);

			Assert.Equal(1, lazyLoaderCalls.Count);
		}

		[Fact]
		public void E08_CanNotCreateInstanceOfCustomerWithOnlyStrings()
		{
			ICollection<Tuple<object, string>> lazyLoaderCalls = new List<Tuple<object, string>>();

			Assert.Throws<MissingMethodException>(() => _ = TestHelpers.CreateInstance<CustomerWithStrings>(ExclusionTests.TestResult.Assembly, "Customer1",
				new Action<object, string>((entity, property) => lazyLoaderCalls.Add(new Tuple<object, string>(entity, property)))));

			Assert.Equal(0, lazyLoaderCalls.Count);
		}
	}
}