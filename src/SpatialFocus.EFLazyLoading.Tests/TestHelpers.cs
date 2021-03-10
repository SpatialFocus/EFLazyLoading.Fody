// <copyright file="TestHelpers.cs" company="Spatial Focus GmbH">
// Copyright (c) Spatial Focus GmbH. All rights reserved.
// </copyright>

namespace SpatialFocus.EFLazyLoading.Tests
{
	using System;
	using System.Globalization;
	using System.Reflection;

	public static class TestHelpers
	{
		public static BindingFlags ConstructorBindingFlags =>
			BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

		public static dynamic CreateInstance<T>(System.Reflection.Assembly assembly, params object[] parameters)
		{
			return Activator.CreateInstance(TestHelpers.CreateType<T>(assembly), TestHelpers.ConstructorBindingFlags, null, parameters,
				CultureInfo.InvariantCulture) ?? throw new InvalidOperationException();
		}

		public static Type CreateType<T>(System.Reflection.Assembly assembly)
		{
			return assembly.GetType(typeof(T).FullName ?? string.Empty) ?? throw new InvalidOperationException();
		}
	}
}