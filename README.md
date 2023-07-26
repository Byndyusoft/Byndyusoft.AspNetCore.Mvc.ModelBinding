# Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData [![Nuget](https://img.shields.io/nuget/v/ExampleProject.svg)](https://www.nuget.org/packages/Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData/)[![Downloads](https://img.shields.io/nuget/dt/Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData.svg)](https://www.nuget.org/packages/Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData/)

This package allows you to read form data values and files. Files are not read from requests during model binding and files' streams are provided for developers. These files' streams must be read consequentially.

## Installing

```shell
dotnet add package Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData
```

## Usage

Register model binder for [FormStreamedFileCollection](src/FormStreamedData/Values/FormStreamedFileCollection.cs) type.

```csharp
  services.AddControllers(o => o.AddFormStreamedFileCollectionBinder());
```

Replace default value providers with [FormStreamedDataValueProvider](src/FormStreamedData/Binders/FormStreamedDataValueProvider.cs) for form-data actions by using [SetFormStreamedDataValueProviderAttribute](src/FormStreamedData/Attributes/SetFormStreamedDataValueProviderAttribute.cs).
Set 

# Contributing

To contribute, you will need to setup your local environment, see [prerequisites](#prerequisites). For the contribution and workflow guide, see [package development lifecycle](#package-development-lifecycle).

## Prerequisites

Make sure you have installed all of the following prerequisites on your development machine:

- Git - [Download & Install Git](https://git-scm.com/downloads). OSX and Linux machines typically have this already installed.
- .NET (.net version) - [Download & Install .NET](https://dotnet.microsoft.com/en-us/download/dotnet/).

## Package development lifecycle

- Implement package logic in `src`
- Add or adapt integration tests (prefer before and simultaneously with coding) in `tests`
- Add or change the documentation as needed
- Open pull request in the correct branch. Target the project's `master` branch

# Maintainers
[github.maintain@byndyusoft.com](mailto:github.maintain@byndyusoft.com)
