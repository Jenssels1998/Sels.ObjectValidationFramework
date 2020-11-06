using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Sels.ObjectValidationFramework.Validator.Case
{
    internal abstract class BasePropertyValidationExecutor<TObject> : BaseValidationExecutor<TObject>
    {
        internal abstract PropertyInfo TargetProperty { get; }

        internal BasePropertyValidationExecutor(ValidationType validationType, Predicate<TObject> condition, ILogger logger) : base (validationType, condition, logger)
        {
        }
    }
}
