# Smart Accountant API

## Before run

* Fill in the blank secrets in `appsettings.json`.

## Development guidelines

* All `ValidationExceptions` are handled by an exception filter.
No need to handle them in controllers.


* In XML documentation of methods, sort exceptions by complexity of the error case. E.g.:

```csharp
/// <exception cref="ImportException"/>
/// <exception cref="ValidationException"/>
/// <exception cref="OperationCanceledException" />
/// <exception cref="ArgumentNullException"/>
```

* Add the following to the beginning of EF migration classes to prevent code style and quality checks:

```csharp
// <auto-generated />
```

Also make those classes _internal_, please.

### Test projects
* Add the following build property to all test projects:

```
<TestProject>true</TestProject>
```

* SDK version of test projects (MSTest.Sdk) needs to be manually updated rather than through individual NuGet package updates.
See https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-mstest-sdk#known-limitations.

* Name test methods as that have 'should'/'must' in the beginning of the name.
E.g., ThrowValidationExceptionForInvalidRequest().

## Build status
[![Build Status](https://dev.azure.com/selaskar/StandardTeamProject/_apis/build/status%2FSmartAccountant.API?branchName=master)](https://dev.azure.com/selaskar/StandardTeamProject/_build/latest?definitionId=41&branchName=master)
