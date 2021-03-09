# SpatialFocus.EFLazyLoading.Fody

An EF Lazy Loading Fody plugin.

[![Nuget](https://img.shields.io/nuget/v/SpatialFocus.EFLazyLoading.Fody)](https://www.nuget.org/packages/SpatialFocus.EFLazyLoading.Fody/)
[![Build & Publish](https://github.com/SpatialFocus/EFLazyLoading.Fody/workflows/Build%20&%20Publish/badge.svg)](https://github.com/SpatialFocus/EFLazyLoading.Fody/actions)
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FSpatialFocus%2FEFLazyLoading.Fody.svg?type=shield)](https://app.fossa.com/projects/git%2Bgithub.com%2FSpatialFocus%2FEFLazyLoading.Fody?ref=badge_shield)

A Fody plugin to inject EF lazy-loader and load statements for readonly collections.

## Usage

See also [Fody usage](https://github.com/Fody/Home/blob/master/pages/usage.md).

### NuGet installation

Install the [SpatialFocus.EFLazyLoading.Fody NuGet package](https://nuget.org/packages/SpatialFocus.EFLazyLoading.Fody/) and update the [Fody NuGet package](https://nuget.org/packages/Fody/):

```powershell
PM> Install-Package Fody
PM> Install-Package SpatialFocus.EFLazyLoading.Fody
```

The `Install-Package Fody` is required since NuGet always defaults to the oldest, and most buggy, version of any dependency.

## License
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FSpatialFocus%2FEFLazyLoading.Fody.svg?type=large)](https://app.fossa.com/projects/git%2Bgithub.com%2FSpatialFocus%2FEFLazyLoading.Fody?ref=badge_large)

----

Made with :heart: by [Spatial Focus](https://spatial-focus.net/)
