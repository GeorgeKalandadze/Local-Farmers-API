namespace LocalFarmersApi.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<LocalFarmersApi.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(LocalFarmersApi.Models.ApplicationDbContext context)
        {
            base.Seed(context);

            context.Categories.AddOrUpdate(
                c => c.Name,
                new Category { Name = "Vegetables", Description = "Fresh vegetables" },
                new Category { Name = "Fruits", Description = "Fresh fruits" },
                new Category { Name = "Dairy", Description = "Dairy products" }
            );

            context.SaveChanges();
        }
    }
}
