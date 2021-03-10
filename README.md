# SpatialFocus.EFLazyLoading.Fody

An EF Lazy Loading Fody plugin.

[![Nuget](https://img.shields.io/nuget/v/SpatialFocus.EFLazyLoading.Fody)](https://www.nuget.org/packages/SpatialFocus.EFLazyLoading.Fody/)
[![Build & Publish](https://github.com/SpatialFocus/EFLazyLoading.Fody/workflows/Build%20&%20Publish/badge.svg)](https://github.com/SpatialFocus/EFLazyLoading.Fody/actions)
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FSpatialFocus%2FEFLazyLoading.Fody.svg?type=shield)](https://app.fossa.com/projects/git%2Bgithub.com%2FSpatialFocus%2FEFLazyLoading.Fody?ref=badge_shield)

A Fody plugin to inject EF lazy-loader and load statements for readonly collections.

If you are going for a clean DDD implementation, you might want to encapsulate access to navigation collection properties and expose those as readonly collections only. If you now reference your backing field within your entity, there is no chance for lazy loading to kick in. For this to work you would have to pollute your entity with EF infrastructure knowledge, as described in this [issue](https://github.com/dotnet/efcore/issues/22752#issuecomment-700282063). This Fody plugin deals exactly with this problem and does the heavy lifting described in the issue mentioned.

## Usage

See also [Fody usage](https://github.com/Fody/Home/blob/master/pages/usage.md).

### NuGet installation

Install the [SpatialFocus.EFLazyLoading.Fody NuGet package](https://nuget.org/packages/SpatialFocus.EFLazyLoading.Fody/) and update the [Fody NuGet package](https://nuget.org/packages/Fody/):

```powershell
PM> Install-Package Fody
PM> Install-Package SpatialFocus.EFLazyLoading.Fody
```

The `Install-Package Fody` is required since NuGet always defaults to the oldest, and most buggy, version of any dependency.

### Add to FodyWeavers.xml

Add `<SpatialFocus.EFLazyLoading/>` to [FodyWeavers.xml](https://github.com/Fody/Home/blob/master/pages/usage.md#add-fodyweaversxml)

```xml
<Weavers>
    <SpatialFocus.EFLazyLoading/>
</Weavers>
```

## Overview

This fody plugin performs the following actions:

* Adds a field `private readonly Action<object, string>? lazyLoader` to your entity
* Overloads existing constructors with an additional `Action<object, string>? lazyLoader` parameter
* For every `ReadOnlyCollection` or `IReadOnlyCollection` it finds the corresponding backing field (`propertyname`, `_propertyname`, `m_propertyname`)
* For every access (including methods & properties) to this backing field (except the navigation property itself), it adds the corresponding `this.lazyLoader?.Invoke` statement

Before code:

```csharp
public class Customer
{
	private readonly List<Order> orders = new();

	public Customer(string name)
	{
		Name = name;
	}

	public int NumberOfOrders => this.orders.Count;

	public virtual IReadOnlyCollection<Order> Orders => this.orders.AsReadOnly();

	public int Id { get; protected set; }

	public string Name { get; protected set; }

	public void AddOrder(Order order) => this.orders.Add(order);

	public void ClearOrders() => this.orders.Clear();

	public void RemoveOrder(Order order) => this.orders.Remove(order);
}
```

What gets compiled

```csharp
public class Customer
{
	private readonly Action<object, string>? lazyLoader;
	private readonly List<Order> orders = new();

	public CustomerWeaved(string name)
	{
		Name = name;
	}

	// For every constructor a constructor overload with Action<object, string> lazyLoader will be added,
	// and the original constructor will be called
	// See https://docs.microsoft.com/en-us/ef/core/querying/related-data/lazy#lazy-loading-without-proxies
	protected CustomerWeaved(string name, Action<object, string> lazyLoader) : this(name)
	{
		this.lazyLoader = lazyLoader;
	}

	public int NumberOfOrders
	{
		get
		{
			this.lazyLoader?.Invoke(this, "Orders");
			return this.orders.Count;
		}
	}

	// Access via navigation property will trigger default lazy loading behaviour
	public virtual IReadOnlyCollection<Order> Orders => this.orders.AsReadOnly();

	public int Id { get; protected set; }

	public string Name { get; protected set; }

	public void AddOrder(Order order)
	{
		this.lazyLoader?.Invoke(this, "Orders");
		this.orders.Add(order);
	}

	public void ClearOrders()
	{
		this.lazyLoader?.Invoke(this, "Orders");
		this.orders.Clear();
	}

	public void RemoveOrder(Order order)
	{
		this.lazyLoader?.Invoke(this, "Orders");
		this.orders.Remove(order);
	}
}
```

## Important remarks

Classes with an already exsting `lazyloader` field or any constructor with a `lazyloader` named parameter will be skipped. Only classes with `ReadOnlyCollection<T>` or `IReadOnlyCollection<T>` navigation properties will be weaved. The generic type of these collections must be class (excluding strings, enums and value types) to be considered for weaving.

## Include or exclude namespaces

These config options are configured by modifying the `SpatialFocus.EFLazyLoading` node in FodyWeavers.xml

### ExcludeNamespaces

A list of namespaces to exclude.

Can take two forms.

As an element with items delimited by a newline.

```xml
<SpatialFocus.EFLazyLoading>
    <ExcludeNamespaces>
        Foo
        Bar
    </ExcludeNamespaces>
</SpatialFocus.EFLazyLoading>
```

Or as a attribute with items delimited by a pipe `|`.

```xml
<SpatialFocus.EFLazyLoading ExcludeNamespaces='Foo|Bar'/>
```

### IncludeNamespaces

A list of namespaces to include.

Can take two forms.

As an element with items delimited by a newline.

```xml
<SpatialFocus.EFLazyLoading>
    <IncludeNamespaces>
        Foo
        Bar
    </IncludeNamespaces>
</SpatialFocus.EFLazyLoading>
```

Or as a attribute with items delimited by a pipe `|`.

```xml
<SpatialFocus.EFLazyLoading IncludeNamespaces='Foo|Bar'/>
```

### Wildcard support

Use `*` at the beginning or at the end of an in- or exclude for wildcard matching.

To include the namespace and all sub-namespaces, simply define it like this:

```xml
<SpatialFocus.EFLazyLoading>
    <IncludeNamespaces>
        Foo
        Foo.*
    </IncludeNamespaces>
</SpatialFocus.EFLazyLoading>
```

### Combination of exclude and include

You can combine excludes and includes, excludes overrule the includes if both match.

## License
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FSpatialFocus%2FEFLazyLoading.Fody.svg?type=large)](https://app.fossa.com/projects/git%2Bgithub.com%2FSpatialFocus%2FEFLazyLoading.Fody?ref=badge_large)

----

Made with :heart: by [Spatial Focus](https://spatial-focus.net/)
