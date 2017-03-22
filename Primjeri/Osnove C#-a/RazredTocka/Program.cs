using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazredTocka
{
  public class Program
  {
    public static void Main(string[] args)
    {
      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);//.NET Core inicijalno podržava manji broj kodnih stranica
      Tocka t = new Tocka(1, -2);
      Console.WriteLine("Točka ({0},{1}) je u {2}.kvadrantu.", t.x, t.y, t.Kvadrant());
    }
  }
}
