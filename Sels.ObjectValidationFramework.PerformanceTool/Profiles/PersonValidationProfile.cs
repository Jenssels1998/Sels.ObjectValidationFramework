using Sels.Core.Extensions.General.Generic;
using Sels.ObjectValidationFramework.PerformanceTool.Entities.Simple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sels.ObjectValidationFramework.PerformanceTool.Profiles
{
    public class PersonValidationProfile : ValidationProfile<string>
    {
        // Constants
        private const int LegalAgeToOwnCars = 16;
        private const int LegalAgeToMarry = 18;

        public PersonValidationProfile()
        {
            CreateValidator<Person>()
                .IfNull(() => $"{nameof(Person)} cannot be null.")
                .IgnorePropertyForValidation(x => x.Spouse)
                .CannotBeNullOrWhiteSpace(x => x.FirstName, (x, y) => $"Person[{y.Id}]: {nameof(Person.FirstName)} cannot be null or whitespace. Was: <{x}>")
                .CannotBeNullOrWhiteSpace(x => x.LastName, (x, y) => $"Person[{y.Id}]: {nameof(Person.LastName)} cannot be null or whitespace. Was: <{x}>")
                .MustBeInPast(x => x.BirthDate, (x, y) => $"Person[{y.Id}]: {nameof(Person.BirthDate)} cannot be in the future. Was: <{x}>")
                .CannotBeNullOrWhiteSpace(x => x.NickNames, (x, y) => $"Person[{y.Id}]: {nameof(Person.NickNames)} cannot be null or whitespace. Was: <{x}>")
                .ConditionalValidation(x => x.Age < LegalAgeToOwnCars, x =>
                {
                    x.MustBeEmpty(x => x.Cars, (x, y) => $"Person[{y.Id}]: People under the age of {LegalAgeToOwnCars} cannot own any cars. Age: <{y.Age}>. Cars Owned: <{x.Count()}>");
                })
                .ConditionalValidation(x => x.Age < LegalAgeToMarry, x =>
                {
                    x.MustBeNull(x => x.Spouse, (x, y) => $"Person[{y.Id}]: People under the age of {LegalAgeToMarry} cannot be married. Age: <{y.Age}>. Married to <{x.FullName}>");
                });
                

            CreateValidator<Car>()
                .IfNull(() => $"{nameof(Car)} cannot be null.")
                .CannotBeNullOrWhiteSpace(x => x.Brand, (x, y) => $"Car[{y.Id}]: {nameof(Car.Brand)} cannot be null or whitespace. Was: <{x}>")
                .CannotBeNullOrWhiteSpace(x => x.Model, (x, y) => $"Car[{y.Id}]: {nameof(Car.Model)} cannot be null or whitespace. Was: <{x}>")
                .MustBeInPast(x => x.ProductionDate, (x, y) => $"Car[{y.Id}]: {nameof(Car.ProductionDate)} cannot be in the future. Was: <{x}>");           
        }
    }
}
