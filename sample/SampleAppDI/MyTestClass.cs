using Microsoft.Extensions.Logging;

namespace SampleAppDI
{
    /// <summary>
    /// A class representing Business Logic Layer (BLL) or Data Access Layer (DAL) methods, properties and functions.
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
