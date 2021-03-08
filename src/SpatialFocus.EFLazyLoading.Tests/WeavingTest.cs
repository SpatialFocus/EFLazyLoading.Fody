// <copyright file="WeavingTest.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace SpatialFocus.EFLazyLoading.Tests
{
	using System;
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
			ModuleWeaver weavingTask = new ModuleWeaver();

			try
			{
				WeavingTest.TestResult =
					weavingTask.ExecuteTestRun($"{typeof(Customer).Namespace}.dll", ignoreCodes: new[] { "0x80131869" });
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		[Fact]
		public void CanCreateInstance()
		{
			dynamic instance = TestHelpers.CreateInstance<Customer>(WeavingTest.TestResult.Assembly, "Test");
		}
	}
}