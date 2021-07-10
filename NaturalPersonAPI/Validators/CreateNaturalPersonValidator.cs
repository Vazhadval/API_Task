using FluentValidation;
using NaturalPersonAPI.Contracts.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NaturalPersonAPI.Validators
{
    public class CreateNaturalPersonValidator : AbstractValidator<CreateNaturalPersonRequest>
    {
        public CreateNaturalPersonValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty();
        }
    }
}
