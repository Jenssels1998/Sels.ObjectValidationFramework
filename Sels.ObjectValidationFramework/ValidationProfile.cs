using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sels.Core.Components.Caching;
using Sels.Core.Extensions.General.Validation;
using Sels.Core.Extensions.Logging;
using Sels.Core.Extensions.Object.Time;
using Sels.ObjectValidationFramework.Validator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private List<BaseValidator> _validators = new List<BaseValidator>();

        // Properties
        internal ValueCache<ReadOnlyCollection<BaseValidator>> Validators { get; }

        public ValidationProfile()
        {
            var factory = new NullLoggerFactory();
            _logger = factory.CreateLogger(LogCategory);
            Validators = new ValueCache<ReadOnlyCollection<BaseValidator>>(() => new ReadOnlyCollection<BaseValidator>(_validators));
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

        /// <summary>
        /// Used to add validators from profileInstance to this profile
        /// </summary>
        /// <param name="profileInstance"></param>
        public void JoinProfile(ValidationProfile<TError> profileInstance)
        {
            profileInstance.ValidateVariable(nameof(profileInstance));
            _logger.LogMessage(LogLevel.Information, () => $"Joining validators from profile {profileInstance.GetType()}");

            _validators.AddRange(profileInstance._validators);
        }

        /// <summary>
        /// Used to add validators from TProfile to this profile
        /// </summary>
        /// <typeparam name="TProfile">Type of validation profile</typeparam>
        public void JoinProfile<TProfile>() where TProfile : ValidationProfile<TError>, new()
        {
            _logger.LogMessage(LogLevel.Information, () => $"Joining validators from profile {typeof(TProfile)}");
            var profileInstance = new TProfile();

            _validators.AddRange(profileInstance._validators);
        }
        #endregion
    }
}
