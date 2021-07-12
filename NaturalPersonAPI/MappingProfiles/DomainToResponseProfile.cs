using AutoMapper;
using NaturalPersonAPI.Contracts.Dtos;
using NaturalPersonAPI.Contracts.Requests;
using NaturalPersonAPI.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NaturalPersonAPI.MappingProfiles
{
    public class DomainToResponseProfile : Profile
    {
        public DomainToResponseProfile()
        {
            CreateMap<NaturalPerson, NaturalPersonDto>()
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => new CityDto { CityName = src.City.CityName, Id = src.City.Id }))
                .ForMember(dest => dest.PhoneNumbers, opt =>
                   opt.MapFrom(src => src.PhoneNumbers.Select(x =>
                     new PhoneNumberDto
                     {
                         Id = x.Id,
                         NaturalPersonId = x.NaturalPersonId,
                         Phone = x.Phone,
                         Type = x.Type
                     })));

            CreateMap<CreateNaturalPersonRequest, NaturalPerson>()
                .ForMember(dest => dest.PhoneNumbers, opt =>
                  opt.MapFrom(src => src.PhoneNumbers.Select(x =>
                    new PhoneNumber
                    {
                        Type = x.PhoneNumberType,
                        Phone = x.Phone,
                    })));
        }
    }
}
