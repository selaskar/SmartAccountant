# Smart Accountant API

## Before run

* Fill in the blank secrets in `appsettings.json`.

## Development guidelines

* Add the following build property to all test projects.

```
<TestProject>true</TestProject>
```

* All `ValidationExceptions` are handled by an exception filter.
No need to handle them in controllers.

## Build status
[![Build Status](https://dev.azure.com/selaskar/StandardTeamProject/_apis/build/status%2FSmartAccountant.API?branchName=master)](https://dev.azure.com/selaskar/StandardTeamProject/_build/latest?definitionId=41&branchName=master)
