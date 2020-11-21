using Sels.Core.Components.Caching;
using Sels.ObjectValidationFramework.Validator.Case;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;

namespace Sels.ObjectValidationFramework.Validator
{
    public abstract class BaseValidator
    {
        internal abstract Type TargetObjectType { get; }

        internal abstract Delegate ValidateDelegate { get; }
    }
}
