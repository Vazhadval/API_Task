using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NaturalPersonAPI.Domain
{
    public class City
    {
        [Key]
        public int Id { get; set; }
        public string CityName { get; set; }
    }
}
