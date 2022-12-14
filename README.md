# Repository Content
Library to implement an abstraction for Cache providers, in order to facilitate the shift between in-memory approach to Distributed one, based on your system needs and requirements

[![Build Status](https://garaproject.visualstudio.com/UmbrellaFramework/_apis/build/status/Umbrella.Infrastructure.Cache?branchName=main)](https://garaproject.visualstudio.com/UmbrellaFramework/_build/latest?definitionId=76&branchName=main)


To install it, use proper command:
```
dotnet add package Umbrella.Infrastructure.Cache 
```

[![Nuget](https://img.shields.io/nuget/v/Umbrella.Infrastructure.Cache.svg?style=plastic)](https://www.nuget.org/packages/Umbrella.Infrastructure.Cache/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Umbrella.Infrastructure.Cache.svg)](https://www.nuget.org/packages/Umbrella.Infrastructure.Cache/)

For more details about download, see [NuGet Web Site](https://www.nuget.org/packages/Umbrella.Infrastructure.Cache/)

#  Configuration
to enable Cache feature in your application you have to follow these simple steps.

## Add Cache Section to appSettings.json file
First of all, add the settings to your appSettings file, as in the example:

```json
{
  "UmbrellaCache": {
    "AdmitNullValues": false,
    "MinutesLifeTimeDuration":  1
  }
}
```

For more information, see UmbrellaCacheSettings.cs

## Enable Cache capabilities at startup (Net6.0)
To enable the feature, use this simple snippet:

_Simple configuration_
In this scenario you use the simplest cache provider based on dictionary.

```c#
builder.Services.AddCache(builder.Configuration);
```

_Microsoft In-memory Cache_
In this scenario you are using the Microsoft implementation of Cache.

```c#
builder.Services.AddMicrosoftCache(builder.Configuration);
```

_Microsoft Distributed Cache_
In this scenario you are using the Microsoft implementation of Distributed Cache provided by Redis.

```c#
builder.Services.AddMicrosoftDistributedRedisCache(builder.Configuration);
```
in such cese, the appSettings has to be modified a bit:

```json
{
  "UmbrellaCache": {
    "AdmitNullValues": false,
    "MinutesLifeTimeDuration": 1,
    "ConnectionString": "my-conn-str",
    "InstanceName": "my-name"
  }
}
```
