# Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData [![Nuget](https://img.shields.io/nuget/v/ExampleProject.svg)](https://www.nuget.org/packages/Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData/)[![Downloads](https://img.shields.io/nuget/dt/Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData.svg)](https://www.nuget.org/packages/Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData/)

This package allows you to read form data values and files. Files streams are not read in binding model process and are provided to be used in user code.

## How it works

Default asp net core behaviour reads all form contents including files during model binding process. File streams are fully drained to the end and their contents are saved in memory and/or disk before any user custom code.

This behaviour is not convenient when large files are sent and we want to treat them as streams. 
Possible cases:
- You create S3 gateway service and want to upload files to S3 as streams.
- You want to read form string values first and validate its data before reading any files' content.

This package allows you to use default binding model except for files that will be available to user code later after model binding process.

## Requirements

- Always provide form string values first.
- Always provide from file values last.
- Read file streams consequentially, that is at the beginning get first IFormStreamedFile object then read its stream to the end. Then get next IFormStreamedFile object and then read its stream to the end. When you get next IFormStreamedFile object the first file stream will be read automatically to the end and you will not be able to read it.
- Use only one property or one parameter of type [FormStreamedFileCollection](src/FormStreamedData/Values/FormStreamedFileCollection.cs) because it will always be one stream.

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
To read files' streams use only one property or one parameter of type [FormStreamedFileCollection](src/FormStreamedData/Values/FormStreamedFileCollection.cs).

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
