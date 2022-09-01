using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using WebApplication1;
using System.Threading.Tasks;
using FluentAssertions;
using System.Net.Http;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;
using Microsoft.Extensions.DependencyInjection;
using WebApplication1.Models;
using Newtonsoft.Json;
using System.Text;
using System;
using Microsoft.AspNetCore.Authorization.Policy;
using WebApplication.IntegrationTests.Helpers;

namespace WebApplication.IntegrationTests
{
    public class RestaurantControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient _client;
        private WebApplicationFactory<Startup> _factory;
        public RestaurantControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var dbContextOptions = services
                        .SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<RestaurantDbContext>));

                        services.Remove(dbContextOptions);

                        services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                        services.AddMvc(option => option.Filters.Add(new FakeUserFilter()));
                        services.AddDbContext<RestaurantDbContext>(options => options.UseInMemoryDatabase("RestaurantDb"));

                    });
                });
            _client = _factory.CreateClient();
        }
        [Fact]
        public async Task CreateCrestaurant_WithValidModel_ReturnsCreatedStatus()
        {
            //Arrange
            var model = new CreateRestaurantDto()
            {
                Name = "TestRestaurant",
                City = "Kraków",
                Street = "Długa 5"
            };

            //Act
            var httpContent = model.ToJsonHttpContent();
            var response = await _client.PostAsync("/api/restaurant", httpContent);

            //Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();
        }
        [Fact]
        public async Task CreateRestaurant_WithInvalidModel_ReturnsBadRequest()
        {
            //Arrange
            CreateRestaurantDto model = new CreateRestaurantDto()
            {
                ContactEmail = "TestRestaurant@wp.pl",
                Description = "Gdansk test",
                ContactNumber = "999 000 888"
            };
            var httpContent = model.ToJsonHttpContent();
            //Act
            var response = await _client.PostAsync("/api/restaurant", httpContent);

            //Arrange
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
        private void SeedRestaurant(Restaurant restaurant)
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<RestaurantDbContext>();

            _dbContext.Restaurants.Add(restaurant);
            _dbContext.SaveChanges();
        }
        [Fact]
        public async Task Delete_ForNonRestaurantOwner_ReturnsForbidden()
        {
            //Arrange
            var restaurant = new Restaurant()
            {
                CreatedById = 900,
                Name = "Test"
            };
            //Seed
            SeedRestaurant(restaurant);
            //Act
            var response = await _client.DeleteAsync("/api/restaurant/" + restaurant.Id);

            //Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
        }
        [Fact]
        public async Task Delete_ForRestaurantOwner_ReturnsNoContent()
        {
            //Arrange
            var restaurant = new Restaurant()
            {
                CreatedById = 1,
                Name = "Test"
            };
            //Seed
            SeedRestaurant(restaurant);
            //Act
            var response = await _client.DeleteAsync("/api/restaurant/" + restaurant.Id);

            //Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }
        [Fact]
        public async Task Delete_ForNonExistringRestaurant_ReturnsNotFound()
        {
            //Act
            var response = await _client.DeleteAsync("/api/restaurant/999");

            //Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }
        [Theory]
        [InlineData("pageSize=5&pageNumber=1")]
        [InlineData("pageSize=15&pageNumber=2")]
        [InlineData("pageSize=15&pageNumber=300")]
        public async Task GetAll_WithQueryParameters_ReturnsOkResult(string queryParams)
        {

            var response = await _client.GetAsync("/api/restaurant?"+queryParams);
            //Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("pageSize=7&pageNumber=300")]
        [InlineData("pageSize=16&pageNumber=3")]
        [InlineData("pageSize=155&pageNumber=3")]
        [InlineData(null)]
        [InlineData("")]
        public async Task GetAll_withInvalidQueryParams_ReturnsBadRequest(string queryParams)
        {
            var response = await _client.GetAsync("/api/restaurant?" + queryParams);
            //Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
    }
}
