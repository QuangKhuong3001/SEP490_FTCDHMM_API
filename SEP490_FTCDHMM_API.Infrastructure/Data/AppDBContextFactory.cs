using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SEP490_FTCDHMM_API.Infrastructure.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDBContext>
    {
        public AppDBContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDBContext>();

            optionsBuilder.UseSqlServer(
                "server=.;database=SEP490_FTCDHMM;uid=sa;pwd=quangkhuong3010;TrustServerCertificate=True;");

            return new AppDBContext(optionsBuilder.Options);
        }
    }
}
