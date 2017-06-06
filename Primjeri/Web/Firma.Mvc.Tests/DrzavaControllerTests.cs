using Firma.Mvc.Models;
using Firma.Mvc.ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using Xunit;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections;
using Firma.Mvc.Controllers.AutoComplete;
using System.Collections.Generic;
using Firma.Mvc.Controllers;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Firma.Mvc.Tests
{
  public class DrzavaControllerTests
  {   
    IOptions<AppSettings> options;
    public DrzavaControllerTests()
    {
      options = Options.Create<AppSettings>(new AppSettings
      {
        ConnectionString = "nebitno",
        PageOffset = 10,
        PageSize = 10
      });
    }
   
    [Trait("Category", "DrzavaController")]    
    [Fact]
    public void TestNemaDrzava()
    {
      // Arrange      
      var mockLogger = new Mock<ILogger<DrzavaController>>();
      
      var dbOptions = new DbContextOptionsBuilder<FirmaContext>()
                .UseInMemoryDatabase(databaseName: "FirmaMemory1")
                .Options;

      using (var context = new FirmaContext(dbOptions))
      {
        var controller = new DrzavaController(context, options, mockLogger.Object);
        var tempDataMock = new Mock<ITempDataDictionary>();                
        controller.TempData = tempDataMock.Object;

        // Act
        var result = controller.Index();

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Create", redirectToActionResult.ActionName);

        mockLogger.Verify(l => l.Log(LogLevel.Information,
                                    It.IsAny<EventId>(),
                                    It.IsAny<object>(),
                                    It.IsAny<Exception>(),
                                    It.IsAny<Func<object, Exception, string>>())
                                 , Times.Once());
      }
    }


    [Trait("Category", "DrzavaController")]
    [Fact]
    public void TestIspravanBrojDrzavaNaStranici()
    {
      // Arrange      
      var mockLogger = new Mock<ILogger<DrzavaController>>();

      var dbOptions = new DbContextOptionsBuilder<FirmaContext>()
                .UseInMemoryDatabase(databaseName: "FirmaMemory2")
                .Options;

      using (var context = new FirmaContext(dbOptions))
      {
        for (int i = 0; i < 50; i++)
        {
          context.Add(new Drzava
          {
            SifDrzave = i
          });
        }
        context.SaveChanges();
        var controller = new DrzavaController(context, options, mockLogger.Object);
        var tempDataMock = new Mock<ITempDataDictionary>();
        controller.TempData = tempDataMock.Object;

        // Act
        var result = controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        DrzaveViewModel model = Assert.IsType<DrzaveViewModel>(viewResult.Model);
        Assert.Equal(options.Value.PageSize, model.Drzave.Count());        
      }
    }
  }
}
