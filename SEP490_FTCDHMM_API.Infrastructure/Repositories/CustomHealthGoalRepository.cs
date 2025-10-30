﻿using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Infrastructure.Data;

namespace SEP490_FTCDHMM_API.Infrastructure.Repositories
{
    public class CustomHealthGoalRepository : EfRepository<CustomHealthGoal>, ICustomHealthGoalRepository
    {
        private readonly AppDbContext _dbContext;

        public CustomHealthGoalRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
