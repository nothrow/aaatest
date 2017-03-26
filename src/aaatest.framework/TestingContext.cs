using System;

namespace aaatest.framework
{
    public sealed class TestingContext<TClass>
    {
        /// <summary>
        /// Creates subject of the test, using DI
        /// </summary>
        /// <returns></returns>
        public TClass CreateSubject()
        {
            return (TClass) Activator.CreateInstance(typeof(TClass));
        }
    }
}