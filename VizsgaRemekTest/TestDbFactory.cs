using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VizsgaRemekBackend.Data;

namespace VizsgaRemekTest
{
    public static class TestDbFactory
    {

        public static AppDbContext CreateContext(string dbName)

        {

            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;


            return new AppDbContext(options);

        }

    }
}
