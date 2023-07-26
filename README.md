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

Set [SetFormStreamedDataValueProviderAttribute](src/FormStreamedData/Attributes/SetFormStreamedDataValueProviderAttribute.cs) for controller actions where file streams should be read.
This will replace default value providers for form data with [FormStreamedDataValueProvider](src/FormStreamedData/Binders/FormStreamedDataValueProvider.cs).

Set [FromFormStreamedDataAttribute](src/FormStreamedData/Attributes/FromFormStreamedDataAttribute.cs) for action parameters or request object properties that should be bound to form values.
To read files' streams use only one property or one parameter with type [FormStreamedFileCollection](src/FormStreamedData/Values/FormStreamedFileCollection.cs).

Examples:
```csharp
public class NewRequestDto
{
	public string Name { get; set; }
	public int Age { get; set; }
	public FormStreamedFileCollection StreamedFiles { get; set; }
}

[ApiController]
[Route("[controller]")]
public class FilesController : ControllerBase
{
	private readonly FileService _fileService;

	public FilesController(FileService fileService)
	{		
		_fileService = fileService;
	}

	[HttpPost("SaveNew")]
	[RequestSizeLimit(long.MaxValue)]
	[SetFormStreamedDataValueProvider]
	public async Task<ActionResult> SaveNewWay(
		[FromFormStreamedData] NewRequestDto requestDto,
		CancellationToken cancellationToken)
	{		
		await foreach (var file in requestDto.StreamedFiles.WithCancellation(cancellationToken))
		{
			await using var stream = file.OpenReadStream();
			await _fileService.SaveFileAsync(stream, file.FileName, cancellationToken);
		}

		return Ok();
	}

	[HttpPost("SaveNewByParameter")]
	[RequestSizeLimit(long.MaxValue)]
	[SetFormStreamedDataValueProvider]
	public async Task<ActionResult> SaveNewWayByParameter(
		FormStreamedFileCollection files,
		CancellationToken cancellationToken)
	{
		await foreach (var file in files.WithCancellation(cancellationToken))
		{
			await using var stream = file.OpenReadStream();
			var filePath = await _fileService.SaveFileAsync(stream, file.FileName, cancellationToken);
		}

		return Ok();
	}
}
```

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
