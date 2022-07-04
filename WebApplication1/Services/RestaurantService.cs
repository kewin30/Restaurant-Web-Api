using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using WebApplication1.Authorization;
using WebApplication1.Entities;
using WebApplication1.Exceptions;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IRestaurantService
    {
        RestaurantDto GetById(int id);
        PageResult<RestaurantDto> GetAll(RestaurantQuery restaurantQuery);
        int Create(CreateRestaurantDto dto);
        void Delete(int id);
        void Update(int id, UpdateRestaurantDto dto);
    }
    public class RestaurantService : IRestaurantService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<RestaurantService> _logger;
        private readonly IAuthorizationService authorizationService;
        private readonly IUserContextService userContextService;

        public RestaurantService(RestaurantDbContext dbContext, IMapper mapper, ILogger<RestaurantService> logger,
            IAuthorizationService authorizationService, IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            this.authorizationService = authorizationService;
            this.userContextService = userContextService;
        }
        public void Update(int id, UpdateRestaurantDto dto)
        {

            var restaurants = _dbContext
               .Restaurants
               .FirstOrDefault(r => r.Id == id);
            if (restaurants is null)
            {
                throw new NotFoundException("restaurant not found");
            }

            var authorizationResult = authorizationService.AuthorizeAsync(userContextService.User, restaurants,
                new ResourceOperationRequirement(ResourceOperation.Update)).Result;

            if(!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

            restaurants.Name = dto.Name;
            restaurants.Description = dto.Description;
            restaurants.HasDelivery = dto.HasDelivery;

            _dbContext.SaveChanges();
        }
        public void Delete(int id)
        {
            _logger.LogError($"Restaurant with id: {id} DELETE action invoked");
            var restaurants = _dbContext
               .Restaurants
               .FirstOrDefault(r => r.Id == id);
            if (restaurants is null)
            {
                throw new NotFoundException("restaurant not found");
            }

            var authorizationResult = authorizationService.AuthorizeAsync(userContextService.User, restaurants,
              new ResourceOperationRequirement(ResourceOperation.Delete)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

            _dbContext.Restaurants.Remove(restaurants);
            _dbContext.SaveChanges();
        }

        public RestaurantDto GetById(int id)
        {
            var restaurants = _dbContext
                .Restaurants
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .FirstOrDefault(r => r.Id == id);
            if (restaurants is null)
            {
                throw new NotFoundException("restaurant not found");
            }

            var result = _mapper.Map<RestaurantDto>(restaurants);
            return result;
        }
        public PageResult<RestaurantDto> GetAll(RestaurantQuery restaurantQuery)
        {
            var baseQuery = _dbContext
                .Restaurants
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .Where(r => restaurantQuery.SearchPhrase == null || (r.Name.ToLower().Contains(restaurantQuery.SearchPhrase.ToLower())
                                                || r.Description.ToLower().Contains(restaurantQuery.SearchPhrase.ToLower())));

            if(!string.IsNullOrEmpty(restaurantQuery.SortBy))
            {
                var columnsSelectors = new Dictionary<string, Expression<Func<Restaurant, object>>>
                {
                    {nameof(Restaurant.Name),r=>r.Name },
                    {nameof(Restaurant.Description),r=>r.Description },
                    {nameof(Restaurant.Category),r=>r.Category },
                };

                var selectedColumn = columnsSelectors[restaurantQuery.SortBy];

               baseQuery= restaurantQuery.SortDirection== SortDirection.ASC?
                    baseQuery.OrderBy(selectedColumn)
                    :baseQuery.OrderByDescending(selectedColumn);
            }

            var restaurants = baseQuery
                .Skip(restaurantQuery.Pagesize*(restaurantQuery.PageNumber-1))
                .Take(restaurantQuery.Pagesize)
                .ToList();

            int totalItemsCount=baseQuery.Count();
            var restaurantDtos = _mapper.Map<List<RestaurantDto>>(restaurants);

            var result = new PageResult<RestaurantDto>(restaurantDtos, totalItemsCount, restaurantQuery.Pagesize, restaurantQuery.PageNumber);

            return result;
        }
        public int Create(CreateRestaurantDto dto)
        {
            var restaurant = _mapper.Map<Restaurant>(dto);
            restaurant.CreatedById = userContextService.GetUserId;
            _dbContext.Restaurants.Add(restaurant);
            _dbContext.SaveChanges();
            return restaurant.Id;
        }
    }
}
