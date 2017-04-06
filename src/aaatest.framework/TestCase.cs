using System;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace aaatest.framework
{
    /// <summary>
    /// Harness, containing configuration for the unit test.
    /// </summary>
    /// <typeparam name="TClass"></typeparam>
    public class TestCase<TClass> : TestCase
    {
        public TestCase([NotNull] Func<TestingContext<TClass>, TClass> arrange, CallerInformation callerInformation) : base(callerInformation)
        {
            Check.NotNull(arrange, nameof(arrange));
            Arrange = arrange;
        }

        [NotNull]
        public Func<TestingContext<TClass>, TClass> Arrange { get; }
    }

    /// <summary>
    /// Model containing all necessary data for unit test
    /// </summary>
    /// <typeparam name="TClass"></typeparam>
    /// <typeparam name="TActResult"></typeparam>
    public sealed class TestCase<TClass, TActResult> : TestCase<TClass>
    {
        public TestCase([NotNull] Func<TestingContext<TClass>, TClass> arrange,
            [NotNull] Expression<Func<TClass, TActResult>> act,
            [NotNull] Action<TActResult> assert, [NotNull] CallerInformation callerInformation) : base(arrange, callerInformation)
        {
            Check.NotNull(act, nameof(act));
            Check.NotNull(assert, nameof(assert));

            Act = act;
            Assert = assert;
        }


        [NotNull]
        public Expression<Func<TClass, TActResult>> Act { get; }
        [NotNull]
        public Action<TActResult> Assert { get; }


        public override bool HarnessOnly => false;
    }

    /// <summary>
    /// Nongeneric model for test case
    /// </summary>
    public abstract class TestCase
    {
        protected TestCase(CallerInformation callerInformation)
        {
            Check.NotNull(callerInformation, nameof(callerInformation));

            CallerInformation = callerInformation;
        }

        [NotNull]
        public CallerInformation CallerInformation { get; private set; }

        [CanBeNull]
        public string Name { get; private set; }
        [CanBeNull]
        public string Identifier { get; private set; }

        [NotNull]
        public TestCase SetName([NotNull] string name)
        {
            Check.NotNull(name, nameof(name));

            if (Name == null)
                Name = name;

            return this;
        }

        [NotNull]
        public TestCase SetTestIdentifier([NotNull] string identifier)
        {
            Check.NotNull(identifier, nameof(identifier));

            if (Identifier == null)
                Identifier = identifier;
            else
                throw new InvalidOperationException($"Identifier for test {Identifier} already set!");

            return this;
        }

        /// <summary>
        /// true, if this test is not executable (harness only - initialization)
        /// </summary>
        public virtual bool HarnessOnly => true;
    }
}