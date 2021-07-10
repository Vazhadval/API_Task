using NaturalPersonAPI.Domain;
using NaturalPersonAPI.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalPersonAPI.Contracts.Responses
{
    public class SearchPeopleResponse
    {
        public PagedList<NaturalPerson> People { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }
    }
}
