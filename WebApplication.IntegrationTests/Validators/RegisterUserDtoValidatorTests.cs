using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WebApplication1.Entities;
using WebApplication1.Models;
using WebApplication1.Models.Validators;
using Xunit;

namespace WebApplication.IntegrationTests.Validators
{
    public class RegisterUserDtoValidatorTests
    {
        private RestaurantDbContext _dbContext;
        public RegisterUserDtoValidatorTests()
        {
            var builder = new DbContextOptionsBuilder<RestaurantDbContext>();
            builder.UseInMemoryDatabase("TestDb");
            _dbContext = new RestaurantDbContext(builder.Options);
            Seed();

        }
        private void Seed()
        {
            var testUsers = new List<User>
            {
                new User()
                {
                    Email = "test2@test.com"
                },
                new User()
                {
                    Email = "test3@test.com"
                },
            };
            _dbContext.Users.AddRange(testUsers);
            _dbContext.SaveChanges();
        }
        [Fact]
        public void Validate_ForValidModel_ReturnsSuccess()
        {
            //Arrange
            var model = new RegisterUserDto()
            {
                Email="test@test.com",
                Password="Pass123",
                ConfirmPassword= "Pass123"
            };
           
            var validator = new RegisterUserDtoValidator(_dbContext);

            //Act

            var result = validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ForInvalidModel_ReturnsFailure()
        {
            //Arrange
            var model = new RegisterUserDto()
            {
                Email = "test2@test.com",
                Password = "Pass123",
                ConfirmPassword = "Pass123"
            };

            var validator = new RegisterUserDtoValidator(_dbContext);

            //Act

            var result = validator.TestValidate(model);
            result.ShouldHaveAnyValidationError();
        }
    }
}
