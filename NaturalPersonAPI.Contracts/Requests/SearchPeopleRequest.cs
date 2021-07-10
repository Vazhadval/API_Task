using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalPersonAPI.Contracts.Requests
{
    public class SearchPeopleRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PesonalNumber { get; set; }

    }
}
