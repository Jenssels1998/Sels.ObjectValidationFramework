using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Sels.Core.Extensions.General.Validation;

namespace Sels.ObjectValidationFramework
{
    public static class ValidatorExecutionExtensions
    {
        public static IEnumerable<TError> Validate<TObject, TError>(this TObject objectToValidate, ValidationProfile<TError> profile)
        {
            profile.ValidateVariable(nameof(profile));
            return ObjectValidator.Validate(profile, objectToValidate);
        }

        public static IEnumerable<TError> Validate<TProfile, TObject, TError>(this TObject objectToValidate) where TProfile : ValidationProfile<TError>, new()
        {
            var profile = new TProfile();
            return ObjectValidator.Validate(profile, objectToValidate);
        }
    }
}
