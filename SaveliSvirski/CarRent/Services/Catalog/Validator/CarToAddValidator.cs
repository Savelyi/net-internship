using System.Text.RegularExpressions;
using CQRS.Commands;
using FluentValidation;

namespace Validator
{
    public class CarToAddValidator : AbstractValidator<CarToAddCommand>
    {
        public CarToAddValidator()
        {
            var regex = new Regex(@"^\d{4}[A-Z]{2}-\d$");

            RuleFor(c => c.Model).NotEmpty()
                .WithMessage("The Model field cant be empty");

            RuleFor(c => c.RentPrice).NotEmpty()
                .WithMessage("The Price field cant be empty");

            RuleFor(c => c.Make).NotEmpty()
                .WithMessage("The Make field cant be empty");
            RuleFor(c => c.VehicleNumber).NotEmpty()
                .Length(8)
                .Matches(regex)
                .WithMessage("Vehicle Number does not matches the pattern or less or more than 8 characters");
        }
    }
}