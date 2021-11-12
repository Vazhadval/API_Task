using Microsoft.EntityFrameworkCore;
using NaturalPersonAPI.Domain;
using NaturalPersonAPI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NaturalPersonAPI.DataContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }


        public DbSet<NaturalPerson> NaturalPeople { get; set; }
        public DbSet<PhoneNumber> PhoneNumbers { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Relation> Relations { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Seed();
        }
    }
}
