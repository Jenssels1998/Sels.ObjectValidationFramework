using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sels.Core.Components.Caching;
using Sels.Core.Extensions.General.Validation;
using Sels.Core.Extensions.Logging;
using Sels.Core.Extensions.Object.Time;
using Sels.Core.Extensions.Reflection.Expressions;
using Sels.Core.Extensions.Object.ItemContainer;
using Sels.ObjectValidationFramework.Validator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

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

        public ValidationProfile()
        {
            var factory = new NullLoggerFactory();
            _logger = factory.CreateLogger(LogCategory);
        }

        public ValidationProfile(ILoggerFactory factory)
        {
            factory.ValidateVariable(nameof(factory));
            _logger = factory.CreateLogger(LogCategory);
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
        /// Ignores a property for validation. By default the ObjectValidator will check if any IValidators exist for the types on the properties and the underlying types on IEnumerable properties. 
        /// </summary>
        /// <typeparam name="TObject">Object containing the property to ignore</typeparam>
        /// <param name="property">Property to ignore</param>
        public void IgnorePropertyForValidation<TObject>(Expression<Func<TObject, object>> property)
        {
            var propertyInfo = property.ExtractProperty(nameof(property));
            var objectType = typeof(TObject);

            IgnoredProperties.AddValue(objectType, propertyInfo);
        }

        internal bool IsIgnored(PropertyInfo property)
        {
            property.ValidateVariable(nameof(property));

            var parentType = property.DeclaringType;

            return IgnoredProperties.ContainsItem(parentType, property);
        }
        #endregion
        #endregion
    }
}
