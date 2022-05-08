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
    public class RegistrationTests
    {
        [Fact]
        public async void RegisterUser_ReturnsStatusCreated()
        {
            //Arrange
            NewUserDto user = new()
            {
                FirstName = "Marija",
                LastName = "Lakic",
                UserName = "mlakic",
                Password = "test1111",
                Email = "mlakic@matf.rs",
                PhoneNumber = "123-456789"
            };

            //mocking logger
            var logger = new Mock<ILogger<AuthenticationController>>();

            //mocking automapper
            var config = new MapperConfiguration(cfg => cfg.AddProfile<IdentityProfile>());
            var mapper = config.CreateMapper();

            //mockign user manager
            List<User> users = new List<User> { new User() { FirstName = "Marija", LastName = "Lakic" } };
            var userManager = MockUserManager<User>();
            userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<User, string>((x, y) => users.Add(x));

            //mocking role manager
            var roleManager = MockRoleManager<IdentityRole>();
            roleManager.Setup(x => x.RoleExistsAsync("User")).ReturnsAsync(true);

            //mocking auth
            var authService = new Mock<IAuthenticationService>();

            var controller = new AuthenticationController(logger.Object, mapper, userManager.Object, roleManager.Object, authService.Object);

            //Act
            IActionResult response = await controller.RegisterUser(user);

            //Assert
            StatusCodeResult codeResponse = Assert.IsType<StatusCodeResult>(response);
            Assert.Equal(201, codeResponse.StatusCode);

        }

        [Fact]
        public async void RegisterBusinessUser_ReturnsStatusCreated()
        {
            //Arrange
            NewUserDto user = new()
            {
                FirstName = "Ana",
                LastName = "Nikacevic",
                UserName = "anikacevic",
                Password = "test2222",
                Email = "anikacevic@matf.rs",
                PhoneNumber = "123-456789"
            };

            //mocking logger
            var logger = new Mock<ILogger<AuthenticationController>>();

            //mocking automapper
            var config = new MapperConfiguration(cfg => cfg.AddProfile<IdentityProfile>());
            var mapper = config.CreateMapper();

            //mockign user manager
            List<User> users = new List<User> { new User() { FirstName = "Ana", LastName = "Nikacevic" } };
            var userManager = MockUserManager<User>();
            userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<User, string>((x, y) => users.Add(x));

            //mocking role manager
            var roleManager = MockRoleManager<IdentityRole>();
            roleManager.Setup(x => x.RoleExistsAsync("BusinessUser")).ReturnsAsync(true);

            //mocking auth
            var authService = new Mock<IAuthenticationService>();

            var controller = new AuthenticationController(logger.Object, mapper, userManager.Object, roleManager.Object, authService.Object);

            //Act
            IActionResult response = await controller.RegisterBusinessUser(user);

            //Assert
            StatusCodeResult codeResponse = Assert.IsType<StatusCodeResult>(response);
            Assert.Equal(201, codeResponse.StatusCode);

        }


        [Fact]
        public async void RegisterAdministrator_ReturnsStatusCreated()
        {
            //Arrange
            NewUserDto user = new()
            {
                FirstName = "Milica",
                LastName = "Radojicic",
                UserName = "mradojicic",
                Password = "test3333",
                Email = "mradojicic@matf.rs",
                PhoneNumber = "123-456789"
            };

            //mocking logger
            var logger = new Mock<ILogger<AuthenticationController>>();

            //mocking automapper
            var config = new MapperConfiguration(cfg => cfg.AddProfile<IdentityProfile>());
            var mapper = config.CreateMapper();

            //mockign user manager
            List<User> users = new List<User> { new User() { FirstName = "Milica", LastName = "Radojicic" } };
            var userManager = MockUserManager<User>();
            userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<User, string>((x, y) => users.Add(x));

            //mocking role manager
            var roleManager = MockRoleManager<IdentityRole>();
            roleManager.Setup(x => x.RoleExistsAsync("Administrator")).ReturnsAsync(true);

            //mocking auth
            var authService = new Mock<IAuthenticationService>();

            var controller = new AuthenticationController(logger.Object, mapper, userManager.Object, roleManager.Object, authService.Object);

            //Act
            IActionResult response = await controller.RegisterAdministrator(user);

            //Assert
            StatusCodeResult codeResponse = Assert.IsType<StatusCodeResult>(response);
            Assert.Equal(201, codeResponse.StatusCode);

        }
    }
}