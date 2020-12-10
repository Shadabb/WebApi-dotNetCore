using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreCodeCamp.Data.Entities;

namespace CoreCodeCamp.Data
{
    public static class PrepDB
    {
        public static void PrepPopulation(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            SeedData(serviceScope.ServiceProvider.GetService<CampContext>());
        }

        public static void SeedData(CampContext context)
        {
            System.Console.WriteLine("Appling Migrations.....");

            context.Database.Migrate();

            if (!context.Camps.Any())
            {
                System.Console.WriteLine("Adding data - seeding....");

                context.Camps.AddRange(
                    new Camp()
                    {
                        CampId = 1,
                        Moniker = "ATL2018",
                        Name = "Atlanta Code Camp",
                        EventDate = new DateTime(2018, 10, 18),
                        Location = new Location()
                        {
                            LocationId = 1,
                            VenueName = "Atlanta Convention Center",
                            Address1 = "123 Main Street",
                            CityTown = "Atlanta",
                            StateProvince = "GA",
                            PostalCode = "12345",
                            Country = "USA"
                        },
                        Length = 1
                    }
                );

                context.SaveChanges();
            }
            else
            {
                System.Console.WriteLine("Already have data - not seeding");
            }
        }
    }
}
