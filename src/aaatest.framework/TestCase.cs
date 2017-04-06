using System;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace aaatest.framework
{
    /// <summary>
    /// Model containing all necessary data for unit test
    /// </summary>
    /// <typeparam name="TClass"></typeparam>
    /// <typeparam name="TActResult"></typeparam>
    public sealed class TestCase<TClass, TActResult> : TestCase
    {
        public TestCase([NotNull] Func<TestingContext<TClass>, TClass> arrange,
            [NotNull] Expression<Func<TClass, TActResult>> act,
            [NotNull] Action<TActResult> assert, [NotNull] CallerInformation callerInformation) : base(callerInformation)
        {
            Check.NotNull(arrange, nameof(arrange));
            Check.NotNull(act, nameof(act));
            Check.NotNull(assert, nameof(assert));

            Arrange = arrange;
            Act = act;
            Assert = assert;
        }

        [NotNull]
        public Func<TestingContext<TClass>, TClass> Arrange { get; }
        [NotNull]
        public Expression<Func<TClass, TActResult>> Act { get; }
        [NotNull]
        public Action<TActResult> Assert { get; }
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
    }
}