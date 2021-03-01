using Sels.Core.Extensions;
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
            IgnorePropertyForFallThrough<Person>(x => x.Spouse);

            CreateValidator<Person>()
                .IfNull(() => $"{nameof(Person)} cannot be null.")
                .CannotBeNullOrWhiteSpace(x => x.FirstName, x => $"Person[{x.Object.Id}]: {nameof(Person.FirstName)} cannot be null or whitespace. Was: <{x.PropertyValue}>")
                .CannotBeNullOrWhiteSpace(x => x.LastName, x => $"Person[{x.Object.Id}]: {nameof(Person.LastName)} cannot be null or whitespace. Was: <{x.PropertyValue}>")
                .MustBeInPast(x => x.BirthDate, x => $"Person[{x.Object.Id}]: {nameof(Person.BirthDate)} cannot be in the future. Was: <{x.PropertyValue}>")
                .CannotBeNullOrWhiteSpace(x => x.NickNames, x => $"Person[{x.Object.Id}]: {nameof(Person.NickNames)} cannot be null or whitespace. Was: <{x.PropertyValue}>")
                .ConditionalValidation(x => x.Age < LegalAgeToOwnCars, x =>
                {
                    x.MustBeEmpty(x => x.Cars, x => $"Person[{x.Object.Id}]: People under the age of {LegalAgeToOwnCars} cannot own any cars. Age: <{x.Object.Age}>. Cars Owned: <{x.PropertyValue.Count()}>");
                })
                .ConditionalValidation(x => x.Age < LegalAgeToMarry, x =>
                {
                    x.MustBeNull(x => x.Spouse, x => $"Person[{x.Object.Id}]: People under the age of {LegalAgeToMarry} cannot be married. Age: <{x.Object.Age}>. Married to <{x.PropertyValue.FullName}>");
                });
                

            CreateValidator<Car>()
                .IfNull(() => $"{nameof(Car)} cannot be null.")
                .CannotBeNullOrWhiteSpace(x => x.Brand, x => $"Car[{x.Object.Id}]: {nameof(Car.Brand)} cannot be null or whitespace. Was: <{x.PropertyValue}>")
                .CannotBeNullOrWhiteSpace(x => x.Model, x => $"Car[{x.Object.Id}]: {nameof(Car.Model)} cannot be null or whitespace. Was: <{x.PropertyValue}>")
                .MustBeInPast(x => x.ProductionDate, x => $"Car[{x.Object.Id}]: {nameof(Car.ProductionDate)} cannot be in the future. Was: <{x.PropertyValue}>");           
        }
    }
}
