using Firma.Mvc.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Firma.Mvc.Util.Logging
{
  public class FirmaLoggerProvider : ILoggerProvider
  {
    private IServiceProvider serviceProvider;
    private Func<LogLevel, bool> filter;
    public FirmaLoggerProvider(IServiceProvider serviceProvider, Func<LogLevel, bool> filter)
    {
      this.filter = filter;
      this.serviceProvider = serviceProvider;
    }
    public ILogger CreateLogger(string categoryName)
    {      
      return new FirmaLogger(serviceProvider, filter);
    }

    public void Dispose()
    {
      
    }
  }
}
