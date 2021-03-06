﻿using Sels.ObjectValidationFramework.Validator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Sels.ObjectValidationFramework.Extensions
{
    internal static class ValidatorExtensions
    {
        internal static IEnumerable<BaseValidator> GetValidatorsForType(this IEnumerable<BaseValidator> validators, Type type)
        {
            return validators.Where(x => x.TargetObjectType.IsAssignableFrom(type));
        }

        internal static IEnumerable<BaseValidator> GetValidatorsForProperty(this IEnumerable<BaseValidator> validators, PropertyInfo property)
        {
            var propertyType = property.PropertyType;
            return validators.Where(x => x.TargetObjectType.IsAssignableFrom(propertyType));
        }

        internal static bool HasValidatorForType(this IEnumerable<BaseValidator> validators, Type type)
        {
            return validators.Any(x => x.TargetObjectType.IsAssignableFrom(type));
        }

        internal static bool HasValidatorForProperty(this IEnumerable<BaseValidator> validators, PropertyInfo property)
        {
            var propertyType = property.PropertyType;
            return validators.Any(x => x.TargetObjectType.IsAssignableFrom(propertyType));
        }
    }
}
