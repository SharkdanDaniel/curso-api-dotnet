using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Data.Context
{
    public class ContextFactory : IDesignTimeDbContextFactory<MyContext>
    {
        public MyContext CreateDbContext(string[] args)
        {
            // Usado para criar as migrações
            var connectionString = "Server=localhost; port=3306; Database=dbAPI; Uid=root; Pwd=mysql";
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 23));
            var optionsBuilder = new DbContextOptionsBuilder<MyContext>();
            optionsBuilder.UseMySql(connectionString, serverVersion);
            return new MyContext(optionsBuilder.Options);
        }
    }
}
