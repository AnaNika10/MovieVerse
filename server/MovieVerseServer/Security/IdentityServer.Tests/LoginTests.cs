using Xunit;
using IdentityServer.DTOs;
using IdentityServer.Services;
using Moq;
using IdentityServer.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using IdentityServer.Entities;
using AutoMapper;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using IdentityServer.Mapper;
using static IdentityServer.Tests.HelperMethods;

namespace IdentityServer.Tests
{
    public class LoginTests
    {
        [Fact]
        public async void LoginUser_ReturnsStatusUnauthorized()
        {
            //Arrange
            UserCredentialsDto user = new()
            {
                UserName = "noUser",
                Password = "noPassword"
            };

            User notFound = null;

            //mocking logger
            var logger = new Mock<ILogger<AuthenticationController>>();

            //mocking automapper
            var config = new MapperConfiguration(cfg => cfg.AddProfile<IdentityProfile>());
            var mapper = config.CreateMapper();

            //mocking user manager
            var userManager = MockUserManager<User>();

            //mocking role manager
            var roleManager = MockRoleManager<IdentityRole>();

            //mocking auth
            var authService = new Mock<IAuthenticationService>();
            authService.Setup(x => x.ValidateUser(user)).ReturnsAsync(notFound);

            var controller = new AuthenticationController(logger.Object, mapper, userManager.Object, roleManager.Object, authService.Object);

            //Act
            IActionResult response = await controller.Login(user);

            //Assert
            StatusCodeResult codeResponse = Assert.IsType<UnauthorizedResult>(response);
            Assert.Equal(401, codeResponse.StatusCode);

        }

        [Fact]
        public async void LoginUser_ReturnsStatusOk()
        {
            //Arrange
            UserCredentialsDto user = new()
            {
                UserName = "mradojicic",
                Password = "test3333"
            };

            User foundUser = new()
            {
                UserName = "mradojicic",
                Email = "mradojicic@matf.rs"
            };

            AuthenticationModel userToken = new() { AccessToken = "mradojicic+test3333" };

            //mocking logger
            var logger = new Mock<ILogger<AuthenticationController>>();

            //mocking automapper
            var config = new MapperConfiguration(cfg => cfg.AddProfile<IdentityProfile>());
            var mapper = config.CreateMapper();

            //mocking user manager
            var userManager = MockUserManager<User>();

            //mocking role manager
            var roleManager = MockRoleManager<IdentityRole>();

            //mocking auth
            var authService = new Mock<IAuthenticationService>();
            authService.Setup(x => x.ValidateUser(user)).ReturnsAsync(foundUser);
            authService.Setup(x => x.CreateAuthenticationModel(foundUser)).ReturnsAsync(userToken);

            var controller = new AuthenticationController(logger.Object, mapper, userManager.Object, roleManager.Object, authService.Object);

            //Act
            IActionResult response = await controller.Login(user);

            //Assert
            OkObjectResult codeResponse = Assert.IsType<OkObjectResult>(response);
            Assert.Equal(200, codeResponse.StatusCode);
            Assert.Equal(userToken, codeResponse.Value);

        }

    }
}
