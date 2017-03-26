using System;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace aaatest.framework
{
    /// <summary>
    /// Defines which class is being unit tested
    /// </summary>
    /// <typeparam name="TClass"></typeparam>
    public abstract class TestingClass<TClass> where TClass : class
    {
        /// <summary>
        /// Full Arrange, Act, Assert unit test
        /// </summary>
        /// <typeparam name="TActResult"></typeparam>
        /// <param name="arrange"></param>
        /// <param name="act"></param>
        /// <param name="assert"></param>
        /// <returns></returns>
        protected TestCase Test<TActResult>([NotNull] Func<TestingContext<TClass>, TClass> arrange,
            [NotNull] Expression<Func<TClass, TActResult>> act, [NotNull] Action<TActResult> assert)
        {
            Check.NotNull(arrange, nameof(arrange));
            Check.NotNull(act, nameof(act));
            Check.NotNull(assert, nameof(assert));

            return new TestCase<TClass, TActResult>(arrange, act, assert);
        }

        /// <summary>
        /// Test without arrangment
        /// </summary>
        /// <typeparam name="TActResult"></typeparam>
        /// <param name="act"></param>
        /// <param name="assert"></param>
        /// <returns></returns>
        protected TestCase Test<TActResult>([NotNull] Expression<Func<TClass, TActResult>> act,
            [NotNull] Action<TActResult> assert)
        {
            Check.NotNull(act, nameof(act));
            Check.NotNull(assert, nameof(assert));

            return Test(ctx => ctx.CreateSubject(), act, assert);
        }
    }
}