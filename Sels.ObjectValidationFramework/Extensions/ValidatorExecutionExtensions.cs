using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Sels.Core.Extensions;

namespace Sels.ObjectValidationFramework
{
    public static class ValidatorExecutionExtensions
    {
        public static IEnumerable<TError> Validate<TError>(this object objectToValidate, ValidationProfile<TError> profile)
        {
            profile.ValidateVariable(nameof(profile));
            return ObjectValidator.Validate(profile, objectToValidate);
        }

        public static IEnumerable<TError> Validate<TProfile, TError>(this object objectToValidate) where TProfile : ValidationProfile<TError>, new()
        {
            var profile = new TProfile();
            return ObjectValidator.Validate(profile, objectToValidate);
        }
    }
}
