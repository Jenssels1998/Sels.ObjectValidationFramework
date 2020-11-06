using Microsoft.Extensions.Logging;
using Sels.Core.Components.Serialization;
using Sels.Core.Extensions.General.Validation;
using Sels.Core.Extensions.Logging;
using Sels.Core.Extensions.Reflection.Object;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sels.ObjectValidationFramework.Validator.Object
{
    internal class ObjectValidationExecutor<TObject, TError> : BaseValidationExecutor<TObject>
    {
        // Fields
        private readonly Func<TObject, TError> _errorMessage;
        private readonly Predicate<TObject> _objectValueChecker;

        // Properties
        internal override Delegate ValidateDelegate { get; }

        public ObjectValidationExecutor(Predicate<TObject> objectValueChecker, Func<TObject, TError> errorMessage, ValidationType validationType, Predicate<TObject> condition, ILogger logger) : base(validationType, condition, logger)
        {
            objectValueChecker.ValidateVariable(nameof(objectValueChecker));
            errorMessage.ValidateVariable(nameof(errorMessage));

            _objectValueChecker = objectValueChecker;
            _errorMessage = errorMessage;

            ValidateDelegate = this.CreateDelegateForMethod(nameof(Validate));
        }

        internal (bool HasError, TError Error) Validate(TObject objectToValidate)
        {
            _logger.LogMessage(LogLevel.Debug, () => $"Validating Object({typeof(TObject)})");
            TError error = default;

            var result = _objectValueChecker(objectToValidate);

            if (!ShouldGenerateError(result))
            {
                _logger.LogObject<JsonProvider>(LogLevel.Debug, () => $"Object({typeof(TObject)}) passed validation (ValidationType {ValidationType}). Was:", objectToValidate);
                return (false, error);
            }
            else
            {
                _logger.LogObject<JsonProvider>(LogLevel.Debug, () => $"Object({typeof(TObject)}) did not pass validation (ValidationType {ValidationType}). Was:", objectToValidate);
                error = _errorMessage(objectToValidate);
                return (true, error);
            }
        }
    }
}
