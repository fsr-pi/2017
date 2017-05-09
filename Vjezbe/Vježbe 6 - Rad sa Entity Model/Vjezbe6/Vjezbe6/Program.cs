using System;
using Vjezbe6.Models;

namespace Vjezbe6
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //dodavanje zapisa
            //using (var context = new InsuranceSalesContext())
            //{
            //    Pribavitelj a = new Pribavitelj
            //    {
            //        Ime = "Ivan",
            //        Prezime = "Ivić",
            //        Godine = 59
            //    };

            //    context.Pribavitelj.Add(a);
            //    context.SaveChanges();
            //}

            //using (var context = new InsuranceSalesContext())
            //{
            //    Pribavitelj b = context.Pribavitelj.Find(3);
            //    b.Godine = 77;
            //    context.SaveChanges();
            //}

            using (var context = new InsuranceSalesContext())
            {
                Pribavitelj c = context.Pribavitelj.Find(7);
                context.Pribavitelj.Remove(c);
                context.SaveChanges();
            }



        }
    }
}