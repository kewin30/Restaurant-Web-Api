using FluentValidation.TestHelper;
using System.Collections.Generic;
using System.Linq;
using WebApplication1.Entities;
using WebApplication1.Models;
using WebApplication1.Models.Validators;
using Xunit;

namespace WebApplication.IntegrationTests.Validators
{
    public class RestaurantQueryValidatorTests
    {
        public static IEnumerable<object[]> GetSampleValidData()
        {
            var list = new List<RestaurantQuery>()
            {
                new RestaurantQuery()
                {
                    PageNumber = 1,
                    Pagesize = 15
                },
                new RestaurantQuery()
                {
                    PageNumber = 2,
                    Pagesize = 15
                },
                new RestaurantQuery()
                {
                    PageNumber = 1,
                    Pagesize = 5,
                    SortBy = nameof(Restaurant.Name)
                },
                 new RestaurantQuery()
                {
                    PageNumber = 100,
                    Pagesize = 10,
                    SortBy = nameof(Restaurant.Category)
                }
            };
            return list.Select(q => new object[] { q });
        }
        [Theory]
        [MemberData(nameof(GetSampleValidData))]
        public void Validate_ForCorrectModel_ReturnsSuccess(RestaurantQuery model)
        {
            //Arrange
            var validator = new RestaurantQueryValidator();
            //Act
            var result = validator.TestValidate(model);

            //Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        public static IEnumerable<object[]> GetSampleInvalidData()
        {
            var list = new List<RestaurantQuery>()
            {
                new RestaurantQuery()
                {
                    PageNumber = 1,
                    Pagesize = 13
                },
                new RestaurantQuery()
                {
                    PageNumber = 10,
                    Pagesize = 7
                },
                new RestaurantQuery()
                {
                    PageNumber = 0,
                    Pagesize = 0
                },
                new RestaurantQuery()
                {
                    PageNumber = 1,
                    Pagesize = 5,
                    SortBy = nameof(Restaurant.Id)
                },
                 new RestaurantQuery()
                {
                    PageNumber = 100,
                    Pagesize = 100,
                    SortBy = nameof(Restaurant.ContactEmail)
                }
            };
            return list.Select(q => new object[] { q });
        }
        [Theory]
        [MemberData(nameof(GetSampleInvalidData))]
        public void Validate_ForIncorrectModel_ReturnsFailure(RestaurantQuery model)
        {
            //Arrange
            var validator = new RestaurantQueryValidator();
            //Act
            var result = validator.TestValidate(model);

            //Assert
            result.ShouldHaveAnyValidationError();
        }
    }
}
