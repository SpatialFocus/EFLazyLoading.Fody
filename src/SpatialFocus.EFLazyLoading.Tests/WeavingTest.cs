// <copyright file="WeavingTest.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace SpatialFocus.EFLazyLoading.Tests
{
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

			WeavingTest.TestResult = weavingTask.ExecuteTestRun($"{typeof(MyClass).Namespace}.dll", ignoreCodes: new[] { "0x80131869" });
		}

		[Fact]
		public void CanCreateInstance()
		{
			dynamic instance = TestHelpers.CreateInstance<MyClass>(WeavingTest.TestResult.Assembly);
		}
	}
}