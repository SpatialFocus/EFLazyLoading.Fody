// <copyright file="ConstructorTests.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace SpatialFocus.EFLazyLoading.Tests
{
	using System.Reflection;
	using global::Fody;
	using SpatialFocus.EFLazyLoading.Fody;
	using SpatialFocus.EFLazyLoading.Tests.Assembly;
	using Xunit;

	[Collection("Assembly")]
	public class ConstructorTests
	{
		private static readonly TestResult TestResult;

		static ConstructorTests()
		{
			ConstructorTests.TestResult =
				new ModuleWeaver().ExecuteTestRun($"{typeof(Customer).Namespace}.dll", ignoreCodes: new[] { "0x80131869" });
		}

		[Fact]
		public void C01_ConstructorsAreWeaved()
		{
			ConstructorInfo[] constructors = TestHelpers.CreateType<Customer>(ConstructorTests.TestResult.Assembly)
				.GetConstructors(TestHelpers.ConstructorBindingFlags);

			Assert.Equal(2, constructors.Length);
		}

		[Fact]
		public void C02_ConstructorsAreNotWeavedWhenLazyLoaderField()
		{
			ConstructorInfo[] constructors = TestHelpers.CreateType<CustomerWithLazyLoaderField>(ConstructorTests.TestResult.Assembly)
				.GetConstructors(TestHelpers.ConstructorBindingFlags);

			Assert.Single(constructors);
		}

		[Fact]
		public void C03_ConstructorsAreNotWeavedWhenLazyLoaderIsInjected()
		{
			ConstructorInfo[] constructors = TestHelpers.CreateType<CustomerWithInjectedLazyLoader>(ConstructorTests.TestResult.Assembly)
				.GetConstructors(TestHelpers.ConstructorBindingFlags);

			Assert.Single(constructors);
		}

		[Fact]
		public void C04_ConstructorsAreNotWeavedWhenNoReadOnlyClassCollections()
		{
			ConstructorInfo[] constructors = TestHelpers.CreateType<CustomerWithTags>(ConstructorTests.TestResult.Assembly)
				.GetConstructors(TestHelpers.ConstructorBindingFlags);

			Assert.Single(constructors);
		}

		[Fact]
		public void C05_ConstructorsAreWeavedForReadOnlyCollection()
		{
			ConstructorInfo[] constructors = TestHelpers.CreateType<CustomerWithReadOnlyCollection>(ConstructorTests.TestResult.Assembly)
				.GetConstructors(TestHelpers.ConstructorBindingFlags);

			Assert.Equal(2, constructors.Length);
		}
	}
}