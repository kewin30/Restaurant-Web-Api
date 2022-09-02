using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
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
        public static IEnumerable<object[]>GetSampleValidData()
        {
            var List = new List<RegisterUserDto>()
            {
                new RegisterUserDto()
                {
                    Email="test@test.com",
                    Password="Pass123",
                    ConfirmPassword= "Pass123"
                },
                new RegisterUserDto()
                {
                    Email="test@test1.com",
                    Password="Mypassword",
                    ConfirmPassword= "Mypassword"
                },
                new RegisterUserDto()
                {
                    Email="test@test4.com",
                    Password="MySuperSpecialPassword123",
                    ConfirmPassword= "MySuperSpecialPassword123"
                },
                new RegisterUserDto()
                {
                    Email="ItsMyOwnEmail@gmail.com",
                    Password="321drowssaP",
                    ConfirmPassword= "321drowssaP"
                }
            };
            return List.Select(q => new object[] { q });
        }
        [Theory]
        [MemberData(nameof(GetSampleValidData))]
        public void Validate_ForValidModel_ReturnsSuccess(RegisterUserDto model)
        {
            var validator = new RegisterUserDtoValidator(_dbContext);

            //Act

            var result = validator.TestValidate(model);

            //Assert

            result.ShouldNotHaveAnyValidationErrors();
        }


        public static IEnumerable<object[]> GetSampleInvalidData()
        {
            var List = new List<RegisterUserDto>()
            {
                new RegisterUserDto()
                {
                    Email = "test2@test.com",
                    Password = "Pass123",
                    ConfirmPassword = "Pass123"
                },
                new RegisterUserDto()
                {
                    Email="test@test1.com",
                    Password="Mypassw",
                    ConfirmPassword= "Mypassword"
                },
                new RegisterUserDto()
                {
                    Email="test@test4.com",
                    Password="A",
                    ConfirmPassword= "A"
                },
                new RegisterUserDto()
                {
                    Email="",
                    Password="321drowssaP",
                    ConfirmPassword= "321drowssaP"
                },
                new RegisterUserDto()
                {
                    Email="",
                    Password="",
                    ConfirmPassword= ""
                }
            };
            return List.Select(q => new object[] { q });
        }
        [Theory]
        [MemberData(nameof(GetSampleInvalidData))]
        public void Validate_ForInvalidModel_ReturnsFailure(RegisterUserDto model)
        {
            //Arrange
            var validator = new RegisterUserDtoValidator(_dbContext);

            //Act
            var result = validator.TestValidate(model);
            //Assert
            result.ShouldHaveAnyValidationError();
        }
    }
}
