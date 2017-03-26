using System;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using JetBrains.Annotations;

namespace aaatest
{
    internal static class Check
    {
        [ContractAnnotation("value:null => void")]
        [ContractAnnotation("value:notnull => notnull")]
        [DebuggerStepThrough]
        public static T NotNull<T>([NoEnumeration] T value, [NotNull] string parameterName)
        {
            if (ReferenceEquals(value, null))
                throw new ArgumentNullException(parameterName);

            return value;
        }
    }
}
