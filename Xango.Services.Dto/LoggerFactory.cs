using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xango.Services.Dto
{
    public class MyLogger : ILogger
    {
        private readonly string _categoryName;

        public MyLogger(string categoryName)
        {
            _categoryName = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = formatter(state, exception);
            Console.WriteLine($"[{logLevel}] {_categoryName}: {message}");
        }
    }

    public class LoggerFactory : ILoggerFactory
    {
        public void AddProvider(ILoggerProvider provider)
        {
            provider.CreateLogger("AutoMapper");
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new MyLogger(categoryName);
        }

        public void Dispose()
        {
        }
    }
}
