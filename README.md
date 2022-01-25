# Readme.net

## Contents
- [Overview](#overview)
- [ASP.NET Core Integration](#aspnet-core-integration)
- [ASP.NET Core Middleware Reference](#aspnet-core-middleware-reference)

## Overview
With ReadMe's Metrics API your team can get deep insights into your API's usage. If you're a developer, it's super easy to send your API logs to ReadMe, Here's an overview of how the integration works:

- Add the `Readme.Metrics` NuGet package to your API server and integrate the middleware.
- The .NET SDK sends ReadMe the details of your API's incoming requests and outgoing responses, with the option for you to redact any private parameters or headers.
- ReadMe uses these request and response details to create an API Metrics Dashboard which can be used to analyze specific API calls or monitor aggregate usage data. Additionally, if your users log into your API documentation we'll show them logs of the requests they made!

## ASP.NET Core Integration

1. Install the `Readme.Metrics` NuGet package using [Visual Studio, VS Code](https://docs.microsoft.com/en-us/nuget/install-nuget-client-tools) or the following command:

```shell
nuget install Readme.Metrics
```

2. Find the file that creates your `app`. This is often in a `Startup.cs` or `Program.cs` file, located in your root directory. Find the `Configure` method, and add the following line before the routing is enabled. For full details on each option, read more about the [Group Object](#group-object).


```cs
app.Use(async (context, next) =>
{
    HttpRequest req = context.Request;

    context.Items["apiKey"] = <Extract API users API key from the request>
    context.Items["label"] = <Extract API users display name from the request>
    context.Items["email"] = <Extract API users email address from the request>

    await next();
});
```


3. Add the logging middleware to your API server. This will be added immediately after your custom middleware from step 2.

```cs
app.UseMiddleware<Readme.Metrics>();
```


4. Locate `appsettings.json` in the root directory of your Application. Add the following JSON to your configuration and fill in any applicable values. For full details on each option read more about the [ReadMe Object in appsettings.json](readme-object-in-appsettingsjson)

```javascript
"readme": {
    "apiKey": "<Your Readme API Key>",
    "options": {
        "allowList": [ "<Any parameters you want allowed in your log. See docs>" ],
        "denyList": [ "<Any parameters you want removed from your log. See docs>"],
        "development": true, // Where to bucket your data, development or production
        "baseLogUrl": "https://example.readme.com" // Your ReadMe website's base url
    }
}
```

For a full example take a look at our example project: https://github.com/readmeio/metrics-sdks-dotnet/blob/6794e99c86e608f90a8e6c6d1357bd73b4d1de75/RequestResponseLoggingMiddlewareAPI/Startup.cs#L23

## ASP.NET Core Middleware Reference
### Group Object
Before assigning the `Readme.Metrics` middleware you should assign custom middleware to extract certain grouping parameters, as seen in step 2 of the [ASP.NET Core Integration](aspnet-core-integration). The grouping parameters includes three values: `apiKey`, `label` and `email`. While only `apiKey` is required, we recommend providing all three values to get the most out of the metrics dashboard.

Field  | Required? | Type   | Usage
-------|-----------|--------|------------
`apiKey` | yes       | string | API Key used to make the request. Note that this is different from the `readmeAPIKey` described above and should be a value from your API that is unique to each of your users, not part of ReadMe's API.
`label`  | no        | string | This will be the users' display name in the API Metrics Dashboard, as it's much easier to remember a name than an API key.
`email`  | no        | string | Email of the user that is making the call.

Example:

```cs
app.Use(async (context, next) =>
{
    HttpRequest req = context.Request;

    context.Items["apiKey"] = <Extract API users API key from the request>
    context.Items["label"] = <Extract API users display name from the request>
    context.Items["email"] = <Extract API users email address from the request>

    await next();
});
```

### ReadMe Object in appsettings.json
The ASP.NET Core middleware extracts the following parameters from appsettings.json file:

`Parameter`    | Required? | Description
---------------|-----------|------------------
`readmeAPIKey` | yes       | The API key for your ReadMe project. This ensures your requests end up in your dashboard. You can read more about the API key in [our docs](https://docs.readme.com/reference/authentication).
`options`      | no        | Additional options. You can read more under [Additional Express Options](#additional-express-options)


#### Options Object
This is an optional object used to restrict traffic being sent to readme server based on given values in allowList or denyList arrays.

`Option`      | Type             | Description
------------|------------------|---------------
`denyList`         | Array of strings | An array of parameter names that will be redacted from the query parameters, request body (when JSON or form-encoded), response body (when JSON) and headers. For nested request parameters use dot notation (e.g. `a.b.c` to redact the field `c` within `{ a: { b: { c: 'foo' }}}`).
`allowList`        | Array of strings | If included, `denyList` will be ignored and all parameters but those in this list will be redacted.
`development`      | bool             | Defaults to `false`. When `true`, the log will be marked as a development log. This is great for separating staging or test data from data coming from customers.
`baseLogUrl`       | string           | This value is used when building the `x-documentation-url` header (see docs [below](#documentation-url)). It is your ReadMe documentation's base URL (e.g. `https://example.readme.com`). If not provided, we will make one API call a day to determine your base URL (more info in [Documentation URL](#documentation-url). If provided, we will use that value and never look it up automatically.

Example:
```javascript
{
    "apiKey": 'abcd123',
    "options": {
        "denyList": ['password', 'secret'],
        "development": true,
        "baseLogUrl": "https://example.readme.com"
    }
}
```

### Documentation URL
With the middleware loaded, all requests that funneled through it will receive a `x-documentation-url` header applied to the response. The value of this header will be the URL on ReadMe Metrics with which you can view the log for that request.

Make sure to supply a `baseLogUrl` option into your readme settings, which should evaluate to the public-facing URL of your ReadMe project.

