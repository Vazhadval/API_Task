using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalPersonAPI.Contracts.Requests
{
    public class SearchPeopleRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PesonalNumber { get; set; }

    }
}
