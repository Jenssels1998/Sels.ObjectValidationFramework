﻿using Sels.Core.Components.Console;
using Sels.Core.Extensions.General.Generic;
using Sels.Core.Extensions.Object.String;
using Sels.ObjectValidationFramework.TestTool.Objects;
using Sels.ObjectValidationFramework.TestTool.ValidationProfiles;
using System;
using System.Collections.Generic;

namespace Sels.ObjectValidationFramework.TestTool
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleHelper.Run(() =>
            {
                Console.WriteLine("Testing person validation");

                var profile = new PersonValidationProfile();

                var validPerson = new Person()
                {
                    FirstName = "Jens",
                    LastName = "Sels",
                    NickName = "Dragonborn",
                    Age = 22,
                    Gender = Gender.Male,
                    Parent = new Person()
                    {
                        FirstName = "Jimmy",
                        LastName = "Sels",
                        NickName = "Undefined",
                        Age = 46,
                        Gender = Gender.Male
                    }
                };

                var inValidPerson = new Person()
                {
                    FirstName = "",
                    LastName = null,
                    Age = -19,
                    Children = new List<Person>() { 
                        new Person()
                        {
                            FirstName = null,
                            LastName = "\t",
                            Age = 0
                        },new Person()
                        {
                            FirstName = "Some name",
                            LastName = "Same last name",
                            Age = 36,
                            Gender = Gender.Male                        
                        }
                    }
                };

                var validErrors = ObjectValidator.Validate(profile, validPerson);

                Console.WriteLine($"Errors on valid person: {(validErrors.HasValue() ? validErrors.JoinNewLine() : "None")}");

                var inValidErrors = ObjectValidator.Validate(profile, inValidPerson);

                Console.WriteLine($"Errors on invalid person: {(inValidErrors.HasValue() ? inValidErrors.JoinNewLine() : "None")}");

            });
        }
    }
}