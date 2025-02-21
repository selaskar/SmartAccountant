# Smart Accountant API

## Before run

* Fill in the blank secrets in `appsettings.json`.

## Development guidelines

* All `ValidationExceptions` are handled by an exception filter.
No need to handle them in controllers.


### Test projects
* Add the following build property to all test projects.

```
<TestProject>true</TestProject>
```

* SDK version of test projects (MSTest.Sdk) needs to be manually updated rather than through individual NuGet package updates.

* Name test methods as that have 'should'/'must' in the beginning of the name.
E.g., ThrowValidationExceptionForInvalidRequest().

## Build status
[![Build Status](https://dev.azure.com/selaskar/StandardTeamProject/_apis/build/status%2FSmartAccountant.API?branchName=master)](https://dev.azure.com/selaskar/StandardTeamProject/_build/latest?definitionId=41&branchName=master)
