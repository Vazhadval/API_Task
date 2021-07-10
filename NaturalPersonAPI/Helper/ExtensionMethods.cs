using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NaturalPersonAPI.Domain;
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
                   CityName = "Kutaisi",
                   Id = 1,
               },
               new City
               {
                   CityName = "Tbilisi",
                   Id = 2,
               },
               new City
               {
                   CityName = "Batumi",
                   Id = 3,
               },
               new City
               {
                   CityName = "Rustavi",
                   Id = 4,
               },
               new City
               {
                   CityName = "Gori",
                   Id = 5,
               }
               );

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
    }
}
