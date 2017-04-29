using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using Vjezbe5.DB;
using MailKit;
using MimeKit;
using MailKit.Net.Smtp;

namespace Vjezbe5
{
    class Program
    {
        public static IConfigurationRoot Configuration { get; set; }

        static void Main(string[] args)
        {

            Console.WriteLine("Hello World!");

            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

            Configuration = builder.Build();

            //Console.WriteLine($"option1 = {Configuration["option1"]}");
            //Console.WriteLine($"option2 = {Configuration["option2"]}");
            //Console.WriteLine(
            //    $"suboption1 = {Configuration["subsection:suboption1"]}");
            //Console.WriteLine();

            //Console.WriteLine("Wizards:");
            //Console.Write($"{Configuration["wizards:0:Name"]}, ");
            //Console.WriteLine($"age {Configuration["wizards:0:Age"]}");
            //Console.Write($"{Configuration["wizards:1:Name"]}, ");
            //Console.WriteLine($"age {Configuration["wizards:1:Age"]}");

            Console.WriteLine(Configuration["AppSettings:Basics:AppName"]);
            Console.WriteLine(Configuration["Data:DefaultConnection:lokalnaBaza"]);

            GenerirajDatoteku();
            PosaljiEmail();
        }

        private static void GenerirajDatoteku()
        {
            List<string> a = Procedure.GetBankaSteteCPICPID();

            var fileLocation = "C:/Users/davor.skobic/Documents/visual studio 2017/Projects/Vjezbe5/Vjezbe5/test.txt";
            var writer = System.IO.File.CreateText(fileLocation);

            foreach (string zapis in a)
            {
                writer.WriteLine(zapis.ToString());
            }
            writer.Dispose();
        }

        private static void PosaljiEmail()
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Obavijest", "paravan46@gmail.com"));
            message.To.Add(new MailboxAddress("ObavijestPrimatelj", "marko.vrhovac.fsr@gmail.com"));
            message.Subject = "Obavijest u privitku";

            var builder = new BodyBuilder();
            builder.TextBody = @"Hello World - A mail from ASPNET Core";
            builder.Attachments.Add(@"C:/Users/davor.skobic/Documents/visual studio 2017/Projects/Vjezbe5/Vjezbe5/test.txt");
            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587);
                client.Authenticate("nekiMail@gmail.com", "lozinka");
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}