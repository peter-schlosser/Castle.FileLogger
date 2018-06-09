# Castle File Logger

A simple and lightweight log-to-file provider writing log entries by `Microsoft.Extensions.Logging.ILogger` to disk file for offline analysis.

* Adds the provider using an `ILoggerFactory` extension making integration and configuration quick and easy.
* Uses existing `Microsoft.Extensions.Logging` references minimizing dependencies and coding footprint.
* Available as a [NuGet Package](https://www.nuget.org/packages/Castle.FileLogger/) or [C# Class Library](https://github.com/peter-schlosser/Castle.FileLogger/tree/master/src/Castle.Extensions.Logging.FileLogger) for fast and easy integration.
* ASP.NET Core MVC Framework friendly.
* Design and implementation deeply influenced by [`Microsoft.Extensions.Logging.AzureAppServices.BatchingLogger`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.azureappservices?view=aspnetcore-2.1)

*This repository is provided for those wishing to learn and expand their knowledge.  There are several feature-rich loggers available providing these features and so much more.  For those seeking more robust features and options, we recommend using [Serilog](https://www.nuget.org/packages/serilog/) and [log4net](https://www.nuget.org/packages/log4net/).*

## Setup

Begin by installing the [NuGet Package](https://github.com/peter-schlosser/nuget) or add the [C# Class Library](https://github.com/peter-schlosser/nuget) to your project.

Add one line of code to your `Program.cs` file.
```csharp
public static IWebHost BuildWebHost(string[] args) =>
	WebHost.CreateDefaultBuilder(args)
		.ConfigureLogging(logging => logging.AddFile())	// <-- add this line
		.UseStartup<Startup>()
		.Build();
```

Build and run.  Log entries written to your existing `ILogger` will appear in your project folder as: `~/Logs/log-YYYYMMDD.txt`.

## Sample Projects

The sample projects demonstrate the use of logging and `ILogger` within your ASP.NET Core MVC Web Application projects.  These projects indirectly demonstrate File Logging by helping investigators produce results quickly and easily using Core services provided by `Microsoft.Extensions.Logging.ILogger`.

### SampleApp

The [SampleApp](https://github.com/peter-schlosser/Castle.FileLogger/tree/master/sample/SampleApp) ASP.NET Core Web Application Project demonstrates how simple use of the File Logger extension can be.  After adding the library and the the above-mentioned one line of code to `Program.cs`, we add a few more lines to the `HomeController` to demonstrate output of basic log entries.

```csharp
namespace SampleApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            _logger.LogInformation("Method: HomeController.Index() called.");
            return View();
        }
```
Build and run.  Review log entries written to `ILogger` in: `~/Logs/log-YYYYMMDD.txt`.


### SampleAppDI

The [SampleAppDI](https://github.com/peter-schlosser/Castle.FileLogger/tree/master/sample/SampleAppDI) ASP.NET Core Web Application Project demonstrates the use of [Dependency Injection](https://en.wikipedia.org/wiki/Dependency_injection) (DI) of the `ILogger` object into third-party classes not derived from `Conroller`.  This is an **advanced** topic for those wishing to use the `ILogger` from [Business Logic](https://en.wikipedia.org/wiki/Multitier_architecture) and [Data Access](https://en.wikipedia.org/wiki/Multitier_architecture) layer classes.

Begin by modeling your class constructor with the desired class object, in this case `ILogger`:
```csharp
namespace SampleAppDI
{
    /// <summary>
    /// A class representing Business Logic Layer (BLL) or Data Access Layer (DAL)
    /// methods, properties and functions.
    /// </summary>
	public class MyTestClass
    {
        private readonly ILogger<MyTestClass> _logger;

        public MyTestClass(ILogger<MyTestClass> logger)
        {
            _logger = logger;
            _logger.LogInformation("MyTestClass() instantiated.");
        }

        public string Foo()
        {
            _logger.LogInformation("Method: MyTestClass.Foo() called.");
            return "Hello, world.";
        }
    }
}
```

Next, instantiate your class using DI from the `Controller`-derived class: 
```csharp
namespace SampleAppDI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MyTestClass _test;

        public HomeController(ILogger<HomeController> logger, MyTestClass test)
        {
            _logger = logger;
            _test = test;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Method: HomeController.Index() called.");
            _logger.LogInformation(@"MyTestInstance.Foo() = {0}", _test.Foo());
            return View();
        }

```

Finally, register the class to be instantiated using DI in the `ConfigureServices()` method of `Startup.cs`:
```csharp
// This method gets called by the runtime. Use this method to add services to the container.
public void ConfigureServices(IServiceCollection services)
{
    services.AddTransient<MyTestClass>();	// <-- add this line
    services.AddMvc();
}
```

Build and run.  Review log entries written to `ILogger` in: `~/Logs/log-YYYYMMDD.txt`.

