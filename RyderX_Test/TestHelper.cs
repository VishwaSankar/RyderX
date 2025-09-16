using Microsoft.EntityFrameworkCore;
using RyderX_Server.Context;

namespace RyderX_Tests
{
    public static class TestHelper
    {
        public static ApplicationDbContext GetInMemoryDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new ApplicationDbContext(options);
        }
    }
}
