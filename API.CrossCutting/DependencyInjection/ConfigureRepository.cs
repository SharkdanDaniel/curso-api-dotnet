using API.Data.Context;
using API.Data.Implementations;
using API.Data.Repository;
using API.Domain.Interfaces;
using API.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.CrossCutting.DependencyInjection
{
    public class ConfigureRepository
    {
        public static void ConfigureDependenciesRepository(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
            serviceCollection.AddScoped<IUserRepository, UserImplementation>();

            var connectionString = "Server=localhost; port=3306; Database=dbAPI; Uid=root; Pwd=mysql";
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 23));

            serviceCollection.AddDbContext<MyContext>(
                options => options.UseMySql(connectionString, serverVersion)
            );
        }
    }
}
