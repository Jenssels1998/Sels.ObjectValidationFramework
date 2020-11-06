using Microsoft.Extensions.Logging;
using Sels.Core.Extensions.General.Generic;
using Sels.Core.Extensions.General.Validation;
using Sels.Core.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sels.ObjectValidationFramework.Validator
{
    internal abstract class BaseValidationExecutor<TObject>
    {
        // Fields
        protected readonly ILogger _logger;
        private readonly Predicate<TObject> _condition;
        // Properties
        internal ValidationType ValidationType { get; }
        internal BaseValidationExecutor(ValidationType validationType, Predicate<TObject> condition, ILogger logger)
        {
            logger.ValidateVariable(nameof(logger));

            ValidationType = validationType;
            _condition = condition;
            _logger = logger;
        }

        protected bool ShouldGenerateError(bool validationResult)
        {
            return (ValidationType == ValidationType.Valid && !validationResult) || (ValidationType == ValidationType.Invalid && validationResult);
        }

        internal bool CanRun(TObject value)
        {
            if (_condition.HasValue())
            {
                _logger.LogMessage(LogLevel.Debug, () => $"Checking if conditional validation on Object({typeof(TObject)}) can run");
                return _condition(value);
            }

            return true;
        }

        // Abstractions
        internal abstract Delegate ValidateDelegate { get; }
    }
}
