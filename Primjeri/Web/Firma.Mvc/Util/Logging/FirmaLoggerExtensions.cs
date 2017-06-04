using Microsoft.Extensions.Logging;
using System;

namespace Firma.Mvc.Util.Logging
{
  public static class FirmaLoggerExtensions
  {
    public static ILoggerFactory AddFirmaLogger(this ILoggerFactory factory, IServiceProvider serviceProvider,  Func<LogLevel, bool> filter = null)
    {
      factory.AddProvider(new FirmaLoggerProvider(serviceProvider, filter));
      return factory;
    }
  }
}
