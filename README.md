# Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData [![Nuget](https://img.shields.io/nuget/v/Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData.svg)](https://www.nuget.org/packages/Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData/)[![Downloads](https://img.shields.io/nuget/dt/Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData.svg)](https://www.nuget.org/packages/Byndyusoft.AspNetCore.Mvc.ModelBinding.FormStreamedData/)

This package allows you to read form data values and files. Files streams are not read in binding model process and are provided to be used in user code.

## How it works

Default asp net core behaviour reads all form contents including files during model binding process. File streams are fully drained to the end and their contents are saved in memory and/or disk before any user custom code.

This behaviour is not convenient when we want to treat files as streams. 
Possible cases:
- You process file streams consequentially. For example, you want to calculate file hash only.
- You want to read form string values first and validate its data before reading any files' content.

This package allows you to use default binding model except for files that will be available to user code later after model binding process.

There are some drawbacks:
- If you read IFormStreamedFile object from [FormStreamedFileCollection](src/FormStreamedData/Values/FormStreamedFileCollection.cs) then previous files' streams will become empty either you have read them earlier or not. It is not obvious behaviour.
- Swagger is not supported.
- FormOptions settings are not supported yet.
- RequestFormLimitsAttribute is not supported yet.

It is recommended to use standard behaviour if both conditions are met:
1. You don't need streaming.
2. You have enough resources to store incoming files' content of all simultaneous requests in memory and on disk.

In [Benchmarking](README.md#Benchmarking) section you can see that new behaviour is faster in high performance stream processing use cases. It is strongly recommended to measure performance for your use case before using it in production environment.

## Implementation

The implementation is based on Microsoft [suggestion](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-7.0#upload-large-files-with-streaming) with addition:
- DisableFormValueModelBindingAttribute is presented as [SetFormStreamedDataValueProviderAttribute](src/FormStreamedData/Attributes/SetFormStreamedDataValueProviderAttribute.cs) which actually replaces value provider with new [provider](src/FormStreamedData/Binders/FormStreamedDataValueProvider.cs).
- Model binding is enabled. It means that all standard and custom binding to your properties of specific types from string values is supported.
- Model binding of property or parameter of type [FormStreamedFileCollection](src/FormStreamedData/Values/FormStreamedFileCollection.cs) is introduced. This type allows reading file contents consequentially by user code after model binding process.
- You can read form data with [ReadFormStreamedDataAsync](src/FormStreamedData/Extensions/FormStreamedDataExtensions.cs) extensions method of HttpRequest.

## New behaviour usage requirements

- Always provide form string values first.
- Always provide form file values last.
- Read file streams consequentially, that is at the beginning get first IFormStreamedFile object then read its stream to the end. Then get next IFormStreamedFile object and then read its stream to the end. When you get next IFormStreamedFile object the first file stream will be read automatically to the end and you will not be able to read it.
- Use only one property or one parameter of type [FormStreamedFileCollection](src/FormStreamedData/Values/FormStreamedFileCollection.cs) because it will always be one stream.
- Do not combine standard behaviour and this behaviour in one controller action. For example, do not use IFormFile and [FormStreamedFileCollection](src/FormStreamedData/Values/FormStreamedFileCollection.cs) in one action.

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

# Benchmarking

## Use cases and implementation

BenchmarkNet was used to measure new behaviour performance.
3 use cases were tested:
1. File content hashing.
2. Saving to disk.
3. Uploading to localhost Minio S3 storage.

All three cases were implemented in [TestApi](tests/TestApi) project. Performance tests were implemented in (PerformanceTests)[tests/PerformanceTests] project.

There were used three values for TestFileSize parameter:
1. Small - API receives 5 1Mb-sized generated files.
2. Big - API receives 2 100Mb-sized generated files.
3. Large - API receives 1 1Gb-sized generated files.

All these values can be changed in [FileGeneratorSetting](tests/PerformanceTests/Files/FileGeneratorSetting.cs) class.

## Results

### File content hashing use case

|  Method | TestFileSize |         Mean |      Error |     StdDev |    StdErr |
|-------- |------------- |-------------:|-----------:|-----------:|----------:|
| **HashOld** |        **Small** |     **41.65 ms** |   **0.822 ms** |   **1.698 ms** |  **0.236 ms** |
| HashNew |        Small |     15.39 ms |   0.058 ms |   0.045 ms |  0.013 ms |
| **HashOld** |          **Big** |  **1,259.90 ms** |  **24.976 ms** |  **26.724 ms** |  **6.299 ms** |
| HashNew |          Big |    485.09 ms |  10.955 ms |  30.899 ms |  3.221 ms |
| **HashOld** |        **Large** | **10,082.17 ms** | **176.127 ms** | **164.749 ms** | **42.538 ms** |
| HashNew |        Large |  2,469.44 ms |  44.506 ms | 110.007 ms | 12.964 ms |

### Saving to disk use case

|  Method | TestFileSize |        Mean |      Error |     StdDev |    StdErr |
|-------- |------------- |------------:|-----------:|-----------:|----------:|
| **SaveOld** |        **Small** |    **24.87 ms** |   **0.493 ms** |   **1.102 ms** |  **0.142 ms** |
| SaveNew |        Small |    14.91 ms |   0.107 ms |   0.083 ms |  0.024 ms |
| **SaveOld** |          **Big** |   **836.72 ms** |  **39.259 ms** | **113.272 ms** | **11.561 ms** |
| SaveNew |          Big |   501.68 ms |  11.450 ms |  32.669 ms |  3.370 ms |
| **SaveOld** |        **Large** | **8,952.73 ms** | **178.932 ms** | **513.388 ms** | **52.673 ms** |
| SaveNew |        Large | 7,913.74 ms | 139.790 ms | 149.574 ms | 35.255 ms |

### Uploading to localhost Minio S3 storage use case

|    Method | TestFileSize |         Mean |       Error |      StdDev |      StdErr |
|---------- |------------- |-------------:|------------:|------------:|------------:|
| **UploadOld** |        **Small** |     **184.0 ms** |    **62.66 ms** |    **41.45 ms** |    **13.11 ms** |
| UploadNew |        Small |     446.3 ms |   140.33 ms |    92.82 ms |    29.35 ms |
| **UploadOld** |          **Big** |   **5,068.9 ms** |   **653.25 ms** |   **432.08 ms** |   **136.64 ms** |
| UploadNew |          Big |  51,659.3 ms | 1,413.90 ms |   935.21 ms |   295.74 ms |
| **UploadOld** |        **Large** |  **26,848.8 ms** | **1,585.32 ms** | **1,048.59 ms** |   **331.59 ms** |
| UploadNew |        Large | 259,196.0 ms | 7,761.91 ms | 5,134.02 ms | 1,623.52 ms |

## Summary

1. New behaviour is faster if file contents are being read rapidly. It can be observed in first two use cases.
2. Standard (old) behaviour is faster if files streams are sent to S3. It is much faster to read all content before sending it to next service as another stream. [BufferedStream](https://learn.microsoft.com/en-us/dotnet/api/system.io.bufferedstream) can boost new behaviour speed but it will not be faster anyway.
3. Use new behaviour only if you need to process files` content in high performance method or if you do not have enough resources to store incoming files' content of all simultaneous requests in memory and on disk.

Api site, performance test console application and Mino S3 server storage were launched on one machine. So these results may not reflect production environment performance.
You are welcome to update them if performance is measured in production (or at least preproduction) environment.

# Contributing

To contribute, you will need to setup your local environment, see [prerequisites](#prerequisites). For the contribution and workflow guide, see [package development lifecycle](#package-development-lifecycle).

## Prerequisites

Make sure you have installed all of the following prerequisites on your development machine:

- Git - [Download & Install Git](https://git-scm.com/downloads). OSX and Linux machines typically have this already installed.
- .NET Core 3.1 or higher - [Download & Install .NET Core 3.1](https://dotnet.microsoft.com/en-us/download/dotnet/3.1).

## Package development lifecycle

- Implement package logic in `src`
- Add or adapt integration tests (prefer before and simultaneously with coding) in `tests`
- Add or change the documentation as needed
- Open pull request in the correct branch. Target the project's `master` branch

# Maintainers
[github.maintain@byndyusoft.com](mailto:github.maintain@byndyusoft.com)
