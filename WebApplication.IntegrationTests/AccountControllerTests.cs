﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebApplication.IntegrationTests.Helpers;
using WebApplication1;
using WebApplication1.Entities;
using WebApplication1.Models;
using WebApplication1.Services;
using Xunit;

namespace WebApplication.IntegrationTests
{
    public class AccountControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient _client;
        private Mock<IAccountService> _accountServiceMock = new Mock<IAccountService>();
        public AccountControllerTests(WebApplicationFactory<Startup> factory)
        {
            _client = factory
                   .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var dbContextOptions = services
                        .SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<RestaurantDbContext>));

                        services.Remove(dbContextOptions);
                        services.AddSingleton <IAccountService>(_accountServiceMock.Object);
                        services.AddDbContext<RestaurantDbContext>(options => options.UseInMemoryDatabase("RestaurantDb"));

                    });
                })
               .CreateClient();
        }
        [Fact]
        public async Task RegisterUser_ForValidModel_ReturnsOk()
        {
            // Arrange
            var registerUser = new RegisterUserDto()
            {
                Email = "test@test.com",
                Password = "password123",
                ConfirmPassword = "password123"
            };
            var httpContent = registerUser.ToJsonHttpContent();
            //Act
            var response = await _client.PostAsync("/api/account/register", httpContent);

            //Asser
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task RegisterUser_ForInvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var registerUser = new RegisterUserDto()
            {
                //Email = "test@test.com",
                Password = "password123",
                ConfirmPassword = "123"
            };
            var httpContent = registerUser.ToJsonHttpContent();
            //Act
            var response = await _client.PostAsync("/api/account/register", httpContent);

            //Asser
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
        [Fact]
        public async Task Login_ForRegisteredUser_ReturnsOk()
        {
            //Arrange
            _accountServiceMock
                .Setup(e => e.GenerateJwt(It.IsAny<LoginDto>()))
                .Returns("jwt");
            var loginDto = new LoginDto()
            {
                Email = "test@test.com",
                Password = "password123"
            };
            var httpContent = loginDto.ToJsonHttpContent();

            //Act
           var response = await _client.PostAsync("api/account/login", httpContent);

            //Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }
    }
}
