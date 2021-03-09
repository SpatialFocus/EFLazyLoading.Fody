// <copyright file="TestHelpers.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace SpatialFocus.EFLazyLoading.Tests
{
	using System;

	public static class TestHelpers
	{
		public static dynamic CreateInstance<T>(System.Reflection.Assembly assembly, params object[] parameters)
		{
			return Activator.CreateInstance(TestHelpers.CreateType<T>(assembly), parameters) ?? throw new InvalidOperationException();
		}

		private static Type CreateType<T>(System.Reflection.Assembly assembly)
		{
			return assembly.GetType(typeof(T).FullName ?? string.Empty) ?? throw new InvalidOperationException();
		}
	}
}