using System;
using System.Text;

namespace Pozdrav
{
  public class Program
  {
    public static void Main(string[] args)
    {      
      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);//.NET Core inicijalno podržava manji broj kodnih stranica
      Console.WriteLine("Pozdrav: ");
     
      string line = Console.ReadLine();
      Console.WriteLine("Upisani tekst je " + line);
      Console.WriteLine("Upisani tekst je {0}",  line);
      Console.WriteLine($"Upisani tekst je {line}");
    }
  }
}
