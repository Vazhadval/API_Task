using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Localization;
using NaturalPersonAPI.Contracts;
using NaturalPersonAPI.Contracts.Dtos;
using NaturalPersonAPI.Contracts.Requests;
using NaturalPersonAPI.Contracts.Responses;
using NaturalPersonAPI.Domain;
using NaturalPersonAPI.Domain.Enums;
using NaturalPersonAPI.Helper;
using NaturalPersonAPI.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NaturalPersonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NaruralPersonController : ControllerBase
    {
        private readonly IStringLocalizer<NaruralPersonController> _localizer;
        private readonly INaturalPersonService _naturalPersonService;
        private readonly IFileProcessingService _fileProcessingService;
        private readonly IMapper _mapper;

        public NaruralPersonController(IStringLocalizer<NaruralPersonController> localizer, INaturalPersonService naturalPersonService, IFileProcessingService fileProcessingService, IMapper mapper)
        {
            _naturalPersonService = naturalPersonService;
            _fileProcessingService = fileProcessingService;
            _mapper = mapper;
            _localizer = localizer;
        }


        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateNaturalPersonRequest request)
        {
            if (await _naturalPersonService.PersonExistsAsync(request.PersonalNumber))
            {
                return BadRequest(new CreateNaturalPersonResponse
                {
                    Success = false,
                    Error = _localizer["PersonExists"]
                });
            }

            if (!await _naturalPersonService.CityExistsAsync(request.CityId))
            {
                return BadRequest(new CreateNaturalPersonResponse
                {
                    Success = false,
                    Error = _localizer["CityNotExist"]
                });
            }

            var person = _mapper.Map<NaturalPerson>(request);


            var result = await _naturalPersonService.CreatePersonAsync(person);
            if (result == null)
            {
                return BadRequest(new CreateNaturalPersonResponse
                {
                    Success = false,
                    Error = _localizer["IncorrectFields"]
                });
            }


            return Ok(new CreateNaturalPersonResponse
            {
                Success = true,
                CreatedPerson = _mapper.Map<NaturalPersonDto>(result)
            });
        }

        [HttpPatch("UpdatePerson/{personId}")]
        public async Task<IActionResult> UpdatePerson(long personId, [FromBody] JsonPatchDocument<NaturalPerson> patch)
        {
            var p = await _naturalPersonService.GetPersonByIdAsync(personId, false);
            if (p == null)
            {
                return NotFound(new UpdatePersonResponse
                {
                    Success = false,
                    Error = _localizer["PersonNotFound"]
                });
            }
            patch.CheckForNotUpdatableProperties();

            patch.ApplyTo(p, ModelState);

            await _naturalPersonService.SaveChangesAsync();

            return Ok(new UpdatePersonResponse
            {
                UpatedPerson = _mapper.Map<NaturalPersonDto>(p),
                Success = true
            });
        }

        [HttpPost("UploadOrEditPersonImage/{personId}")]
        public async Task<IActionResult> UploadOrdEditPersonPhoto(long personId, IFormFile photo)
        {
            var p = await _naturalPersonService.GetPersonByIdAsync(personId, false);
            if (p == null)
            {
                return NotFound(new UploadOrEditPersonPhotoResponse
                {
                    Error = _localizer["PersonNotFound"],
                    Success = false
                });
            }

            if (!photo.IsImage())
            {
                return BadRequest(new UploadOrEditPersonPhotoResponse
                {
                    Error = _localizer["IncorrectFileFormat"],
                    Success = false
                });
            }

            string relativePhotoPath = await _fileProcessingService.UploadFileAsync(photo);
            if (relativePhotoPath == string.Empty)
            {
                return BadRequest(new UploadOrEditPersonPhotoResponse
                {
                    Error = _localizer["UploadError"],
                    Success = false
                });
            }

            string photoPath = Request.Scheme + "://" + Request.Host + relativePhotoPath;

            await _naturalPersonService.SetPhotoToPersonAsync(personId, photoPath);

            return Ok(new UploadOrEditPersonPhotoResponse
            {
                PhotoPath = photoPath,
                Success = true
            });
        }

        [HttpPost("AddRelatedPerson/{parentPersonId}")]
        public async Task<IActionResult> AddRelatedPerson(long parentPersonId, RelationType relationType, CreateNaturalPersonRequest person)
        {
            if (await _naturalPersonService.PersonExistsAsync(person.PersonalNumber))
            {
                return BadRequest(new CreateNaturalPersonResponse
                {
                    Success = false,
                    Error = _localizer["PersonExists"]
                });
            }
            var p = _mapper.Map<NaturalPerson>(person);

            var result = await _naturalPersonService.AddRelatedPersonAsync(parentPersonId, relationType, p);
            if (result == null)
            {
                return BadRequest(new AddRelatedPersonResponse
                {
                    Success = false,
                    Error = _localizer["CityNotExist"],
                });
            }

            return Ok(new AddRelatedPersonResponse
            {
                Success = true,
                CreatedPerson = _mapper.Map<NaturalPersonDto>(result)
            });
        }


        [HttpDelete("DeleteRelatedPerson")]
        public async Task<IActionResult> DeleteRelatedPerson(DeleteRelatedPersonRequest request)
        {
            var result = await _naturalPersonService.DeleteRelatedPerson(request.ParentPersonId, request.RelatedPersonId);
            if (!result)
            {
                return BadRequest(_localizer["PersonNotFound"]);
            }

            return NoContent();
        }

        [HttpDelete("DeletePerson/{personId}")]
        public async Task<IActionResult> DeletePreson(long personId)
        {
            var result = await _naturalPersonService.DeletePerson(personId);
            if (!result)
            {
                return BadRequest(new DeletePersonResponse
                {
                    Error = _localizer["PersonNotFound"],
                    Success = false
                });
            }
            return NoContent();
        }

        [HttpGet("getPersonById/{personId}")]
        public async Task<IActionResult> GetPersonById(long personId)
        {
            var result = await _naturalPersonService.GetPersonByIdAsync(personId, includeRelatedPeople: true);
            if (result == null)
            {
                return BadRequest(new GetPersonByIdResponse
                {
                    Success = false,
                    Error = _localizer["PersonNotFound"]
                });
            }
            return Ok(new GetPersonByIdResponse
            {
                Person = result,
                Success = true
            });
        }

        [HttpGet("SearchPeople")]
        public IActionResult SearchPeople([FromQuery] SearchPeopleRequest request)
        {

            var people = _naturalPersonService.SearchPeople(request);

            var mappedPeople = people.Select(x => _mapper.Map<NaturalPersonDto>(x)).AsQueryable();

            var peopleDto = PagedList<NaturalPersonDto>.ToPagedList(mappedPeople, request.PageNumber, request.PageSize);

            return Ok(new SearchPeopleResponse
            {
                People = peopleDto,
                CurrentPage = people.CurrentPage,
                TotalPages = people.TotalPages,
                PageSize = people.PageSize,
                HasNext = people.HasNext,
                HasPrevious = people.HasPrevious,
                TotalCount = people.TotalCount,
                Success = true
            });
        }

        [HttpGet("GetPersonReport/{personId}")]
        public async Task<IActionResult> GetPersonReport(long personId)
        {
            var result = await _naturalPersonService.GetPersonReport(personId);
            if (result == null)
            {
                return BadRequest(new GetPersonReportResponse
                {
                    Success = false,
                    Error = _localizer["PersonNotFound"]
                });
            }
            return Ok(new GetPersonReportResponse
            {
                Success = true,
                Relations = result
            });
        }
    }
}
