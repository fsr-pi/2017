using Microsoft.Extensions.Logging;
using System;

namespace Firma.Mvc.Util.Logging
{
  public class FirmaLogger : ILogger
  {
    private IServiceProvider serviceProvider;
    private Func<LogLevel, bool> filter;
    public FirmaLogger(IServiceProvider serviceProvider, Func<LogLevel, bool>  filter)
    {
      this.filter = filter;
      this.serviceProvider = serviceProvider;
    }
    public IDisposable BeginScope<TState>(TState state)
    {
      return null; //ne podržava scope...
    }

    public bool IsEnabled(LogLevel logLevel)
    {
      if (filter != null)
      {
        return filter(logLevel);
      }
      else
      {
        return true;
      }
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
      //sa serviceProvider.GetService<TIP>() se može pomoći DI-a dobiti konkretna implementacija nekog sučelja
      //npr. za slanje maila i slično
      //poruka za log se dobije pomoću formatter(state, exception) pri čemu state i exception smiju biti null
      //u principu treba paziti da se ne zavrtimo u rekurziju...      
    }
  }
}
