using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NaturalPersonAPI.DataContext;
using NaturalPersonAPI.Domain;
using NaturalPersonAPI.Middlewares;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NaturalPersonAPI.Helper
{
    public static class ExtensionMethods
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>().HasData(
               new City
               {
                   CityName = "Asgard",
                   Id = 1,
               },
               new City
               {
                   CityName = "Gotham",
                   Id = 2,
               },
               new City
               {
                   CityName = "New York City",
                   Id = 3,
               },
               new City
               {
                   CityName = "Wakanda",
                   Id = 4,
               },
               new City
               {
                   CityName = "Kutaisi",
                   Id = 5,
               }
               );

            modelBuilder.Entity<NaturalPerson>().HasData(
                new NaturalPerson
                {
                    Id = 1,
                    FirstName = "Pitter",
                    LastName = "Parker",
                    PersonalNumber = "111",
                    BirthDate = new DateTime(1994, 1, 1),
                    CityId = 3,
                    Gender = "Male",
                },
                new NaturalPerson
                {
                    Id = 2,
                    FirstName = "Black",
                    LastName = "Panther",
                    PersonalNumber = "222",
                    BirthDate = new DateTime(1994, 1, 1),
                    CityId = 4,
                    Gender = "Male",
                },

                 new NaturalPerson
                 {
                     Id = 3,
                     FirstName = "Bruce",
                     LastName = "Wayne",
                     PersonalNumber = "333",
                     BirthDate = new DateTime(1994, 1, 1),
                     CityId = 2,
                     Gender = "Male",
                 },
                  new NaturalPerson
                  {
                      Id = 4,
                      FirstName = "Thor",
                      LastName = "Odinson",
                      PersonalNumber = "333",
                      BirthDate = new DateTime(1994, 1, 1),
                      CityId = 1,
                      Gender = "Male",
                  },
                  new NaturalPerson
                  {
                      Id = 5,
                      FirstName = "Vazha",
                      LastName = "Dvalishvili",
                      PersonalNumber = "333",
                      BirthDate = new DateTime(1994, 1, 1),
                      CityId = 5,
                      Gender = "Male",
                  }

                );

            modelBuilder.Entity<PhoneNumber>().HasData(
                    new PhoneNumber
                    {
                        Id = 1,
                        Phone = "555555111",
                        Type = "Home",
                        NaturalPersonId = 1,
                    },
                     new PhoneNumber
                     {
                         Id = 2,
                         Phone = "555111666",
                         Type = "Office",
                         NaturalPersonId = 1,
                     },

                      new PhoneNumber
                      {
                          Id = 3,
                          Phone = "555444666",
                          Type = "Home",
                          NaturalPersonId = 2,
                      },

                        new PhoneNumber
                        {
                            Id = 4,
                            Phone = "555888777",
                            Type = "Office",
                            NaturalPersonId = 2
                        }

                );

            modelBuilder.Entity<Relation>().HasData(

                    new Relation
                    {
                        Id = 1,
                        parentPersonId = 1,
                        RelatedPersonId = 2,
                        RelationType = "Friend"
                    },
                     new Relation
                     {
                         Id = 2,
                         parentPersonId = 1,
                         RelatedPersonId = 3,
                         RelationType = "Other"
                     },
                      new Relation
                      {
                          Id = 3,
                          parentPersonId = 2,
                          RelatedPersonId = 3,
                          RelationType = "Friend"
                      }
                );

        }


        public static IHost MigrateDatabase(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                using (var appContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
                {
                    try
                    {
                        if (appContext.Database.GetPendingMigrations().Any())
                        {
                            appContext.Database.Migrate();
                        }

                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }

            return host;
        }

        public static bool IsImage(this IFormFile postedFile)
        {

            if (postedFile.ContentType.ToLower() != "image/jpg" &&
                        postedFile.ContentType.ToLower() != "image/jpeg" &&
                        postedFile.ContentType.ToLower() != "image/pjpeg" &&
                        postedFile.ContentType.ToLower() != "image/gif" &&
                        postedFile.ContentType.ToLower() != "image/x-png" &&
                        postedFile.ContentType.ToLower() != "image/png")
            {
                return false;
            }

            if (Path.GetExtension(postedFile.FileName).ToLower() != ".jpg"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".png"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".gif"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".jpeg")
            {
                return false;
            }


            return true;
        }

        public static IApplicationBuilder UseErrorLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorLoggingMiddleware>();
        }
    }
}
