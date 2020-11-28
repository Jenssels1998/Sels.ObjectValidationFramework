using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sels.Core.Components.Caching;
using Sels.Core.Extensions.General.Validation;
using Sels.Core.Extensions.Logging;
using Sels.Core.Extensions.Object.Time;
using Sels.Core.Extensions.Reflection.Expressions;
using Sels.Core.Extensions.Reflection;
using Sels.Core.Extensions.Object.ItemContainer;
using Sels.ObjectValidationFramework.Validator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Sels.Core.Extensions.Reflection.Object;
using System.Linq;
using Sels.Core.Extensions.ReflectionExtensions;
using Sels.Core.Extensions.Execution.Linq;
using Sels.Core.Extensions.Reflection.Types;

namespace Sels.ObjectValidationFramework
{
    public class ValidationProfile<TError>
    {
        // Constants
        private const string LogCategory = "ObjectValidationFramework";

        // Fields
        private ILogger _logger;
        private readonly List<BaseValidator> _validators = new List<BaseValidator>();

        // Properties
        internal ReadOnlyCollection<BaseValidator> Validators => new ReadOnlyCollection<BaseValidator>(_validators);
        internal Dictionary<Type, List<PropertyInfo>> IgnoredProperties { get; } = new Dictionary<Type, List<PropertyInfo>>();
        internal List<Predicate<Type>> IgnoredTypeCheckers { get; } = new List<Predicate<Type>>();
        internal ILogger Logger => _logger;

        public ValidationProfile() : this(new NullLoggerFactory())
        {
        }

        public ValidationProfile(ILoggerFactory factory)
        {
            factory.ValidateVariable(nameof(factory));
            _logger = factory.CreateLogger(LogCategory);

            // Add default ignored types
            IgnoreTypeForFallThrough(x => x.IsPrimitive);
            IgnoreTypeForFallThrough(typeof(string));
            IgnoreTypeForFallThrough(x => x.IsItemContainer());
        }

        #region Validation Setup
        /// <summary>
        /// Creates a validator that can be used to setup validation for TObject
        /// </summary>
        /// <typeparam name="TObject">Object to be validated</typeparam>
        /// <returns>Validator for TObject</returns>
        public IValidator<TObject, TError> CreateValidator<TObject>()
        {
            using(_logger.CreateTimedLogger(LogLevel.Information, () => $"Trying to create Validator for type {typeof(TObject)}", x => $"Created Validator<{typeof(TObject)}> in {x.PrintTotalMs()}"))
            {
                var validator = new Validator<TObject, TError>(_logger);

                _validators.Add(validator);

                return validator;
            }        
        }

        #region Profile importing
        /// <summary>
        /// Used to add validators from profileInstance to this profile
        /// </summary>
        /// <param name="profileInstance"></param>
        public void ImportProfile(ValidationProfile<TError> profileInstance)
        {
            profileInstance.ValidateVariable(nameof(profileInstance));
            _logger.LogMessage(LogLevel.Information, () => $"Joining validators from profile {profileInstance.GetType()}");

            _validators.AddRange(profileInstance.Validators);
            IgnoredProperties.Merge(profileInstance.IgnoredProperties);
            profileInstance.IgnoredTypeCheckers.Execute(x => IgnoreTypeForFallThrough(x));
        }

        /// <summary>
        /// Used to add validators from TProfile to this profile
        /// </summary>
        /// <typeparam name="TProfile">Type of validation profile</typeparam>
        public void ImportProfile<TProfile>() where TProfile : ValidationProfile<TError>, new()
        {
            _logger.LogMessage(LogLevel.Information, () => $"Joining validators from profile {typeof(TProfile)}");
            var profileInstance = new TProfile();

            ImportProfile(profileInstance);
        }
        #endregion

        #region Ignoring Properties
        /// <summary>
        /// Ignores a property for fallthrough validation. By default the ObjectValidator will check if any IValidators exist for the types on the properties. 
        /// </summary>
        /// <typeparam name="TObject">Object containing the property to ignore</typeparam>
        /// <param name="property">Property to ignore</param>
        public void IgnorePropertyForFallThrough<TObject>(Expression<Func<TObject, object>> property)
        {
            var propertyInfo = property.ExtractProperty(nameof(property));
            var objectType = typeof(TObject);

            IgnoredProperties.AddValue(objectType, propertyInfo);
        }

        internal bool IsIgnored(PropertyInfo property)
        {
            property.ValidateVariable(nameof(property));

            var parentType = property.DeclaringType;

            foreach(var pair in IgnoredProperties)
            {
                if (pair.Key.IsAssignableFrom(parentType))
                {
                    foreach(var ignoredProperty in pair.Value)
                    {
                        if (property.AreEqual(ignoredProperty))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        #endregion

        #region Ignoring Types
        /// <summary>
        /// Ignores a type for fallthrough validation. By default the ObjectValidator will check if any IValidators exist for the types on the properties. 
        /// </summary>
        /// <param name="typeChecker">Delegate that checks if the type is ignored</param>
        public void IgnoreTypeForFallThrough(Predicate<Type> typeChecker)
        {
            typeChecker.ValidateVariable(nameof(typeChecker));

            IgnoredTypeCheckers.AddUnique(typeChecker);
        }
        /// <summary>
        /// Ignores a type for fallthrough validation. By default the ObjectValidator will check if any IValidators exist for the types on the properties. 
        /// </summary>
        /// <param name="type">Type to be ignored</param>
        public void IgnoreTypeForFallThrough(Type type)
        {
            type.ValidateVariable(nameof(type));
            IgnoreTypeForFallThrough(x => type.IsAssignableFrom(x));
        }

        internal bool IsIgnored(Type type)
        {
            type.ValidateVariable(nameof(type));

            if (IgnoredTypeCheckers.Any(typeChecker => typeChecker(type)))
            {
                return true;
            }

            return false;
        }
        #endregion
        #endregion
    }
}
