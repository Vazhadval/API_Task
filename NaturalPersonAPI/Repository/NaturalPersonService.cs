using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using NaturalPersonAPI.Contracts.Requests;
using NaturalPersonAPI.DataContext;
using NaturalPersonAPI.Domain;
using NaturalPersonAPI.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NaturalPersonAPI.Repository
{
    public class NaturalPersonService : INaturalPersonService
    {
        private readonly AppDbContext _context;

        public NaturalPersonService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<NaturalPerson> AddRelatedPersonAsync(long parentPersonId, RelationType relationType, NaturalPerson p)
        {
            var parent = await _context.NaturalPeople.FirstOrDefaultAsync(x => x.Id == parentPersonId);
            if (parent == null)
            {
                return null;
            }

            await CreatePersonAsync(p);

            await _context.Relations.AddAsync(new Relation { parentPersonId = parentPersonId, RelatedPersonId = p.Id, RelationType = relationType.ToString() });

            await _context.SaveChangesAsync();
            return p;

        }

        public async Task<bool> CityExistsAsync(int cityId)
        {
            return await _context.Cities.FirstOrDefaultAsync(x => x.Id == cityId) != null;
        }

        public async Task<NaturalPerson> CreatePersonAsync(NaturalPerson p)
        {
            if (p.PhoneNumbers?.Count() > 0)
            {
                foreach (var phone in p.PhoneNumbers)
                {
                    await _context.PhoneNumbers.AddAsync(phone);
                }
            }

            await _context.NaturalPeople.AddAsync(p);

            var created = await _context.SaveChangesAsync();


            return created > 0 ? p : null;
        }

        public async Task<bool> DeleteRelatedPerson(long parentId, long relatedId)
        {
            var relation = await _context.Relations.FirstOrDefaultAsync(x => x.parentPersonId == parentId && x.RelatedPersonId == relatedId);
            if (relation == null)
            {
                return false;
            }

            var relatedPerson = await _context.NaturalPeople.FirstOrDefaultAsync(x => x.Id == relatedId);

            _context.NaturalPeople.Remove(relatedPerson);

            _context.Relations.Remove(relation);

            return true;
        }

        public Task<NaturalPerson> GetPersonByIdAsync(long id)
        {
            return _context.NaturalPeople.Include(x => x.City).Include(x => x.PhoneNumbers).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public IEnumerable<NaturalPerson> SearchPeople()
        {
            return _context.NaturalPeople.Include(x => x.City).Include(x => x.PhoneNumbers);
        }

        public async Task<bool> SetPhotoToPersonAsync(long personId, string photoPath)
        {
            var p = await _context.NaturalPeople.FirstOrDefaultAsync(x => x.Id == personId);
            p.Photo = photoPath;


            return await _context.SaveChangesAsync() > 0;
        }
    }
}
