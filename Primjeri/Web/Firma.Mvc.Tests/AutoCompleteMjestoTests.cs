using Firma.Mvc.Controllers.AutoComplete;
using Firma.Mvc.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Firma.Mvc.Tests
{
  public class AutoCompleteMjestoTests
  {
    FirmaContext ctx;
    IOptions<AppSettings> options;
    public AutoCompleteMjestoTests()
    {
      //Arrange
      var builder = new ConfigurationBuilder()
          .AddUserSecrets("Firma")
          .AddJsonFile("appsettings.json");        
      var Configuration = builder.Build();

      var appSection = Configuration.GetSection("AppSettings");

      string connectionString = appSection.GetValue<string>("ConnectionString");
      connectionString = connectionString.Replace("sifra", Configuration["FirmaSqlPassword"]);

      var dbContextBuilder = new DbContextOptionsBuilder<FirmaContext>()
                       .UseSqlServer(connectionString);

      ctx = new FirmaContext(dbContextBuilder.Options);

      options = Options.Create<AppSettings>(new AppSettings
      {
        ConnectionString = connectionString,
        PageOffset = int.Parse(appSection["PageOffset"]),
        PageSize = int.Parse(appSection["PageSize"])
      });
    }

    [Trait("Category", "AutoComplete")]    
    [Theory]
    [InlineData("varaždin")]
    [InlineData("VARAŽDIN")]
    [InlineData("ara")]
    [InlineData("din")]
    public void PronadjenNazivMjesta(string value)
    {
      //Act
      var controller = new Controllers.AutoComplete.MjestoController(ctx, options);
      IEnumerable<IdLabel> result = controller.Get(value);

      //Assert
      string naziv = "Varaždin";
      var containsValue = result.Select(idlabel => idlabel.Label)
                                .Any(label => label.IndexOf(naziv, StringComparison.CurrentCultureIgnoreCase) != -1);

      Assert.True(containsValue);

      string nazivKojegNema = "Split";

      containsValue = result.Select(idlabel => idlabel.Label)
                                .Any(label => label.IndexOf(nazivKojegNema, StringComparison.CurrentCultureIgnoreCase) != -1);

      Assert.False(containsValue);
    }
  }
}
