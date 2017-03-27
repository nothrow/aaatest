using System;
using System.Linq.Expressions;
using System.Reflection;

namespace aaatest.framework
{
    /// <summary>
    /// Model containing all necessary data for unit test
    /// </summary>
    /// <typeparam name="TClass"></typeparam>
    /// <typeparam name="TActResult"></typeparam>
    public sealed class TestCase<TClass, TActResult> : TestCase
    {
        public TestCase(Func<TestingContext<TClass>, TClass> arrange, Expression<Func<TClass, TActResult>> act,
            Action<TActResult> assert)
        {
            Arrange = arrange;
            Act = act;
            Assert = assert;
        }

        public Func<TestingContext<TClass>, TClass> Arrange { get; }
        public Expression<Func<TClass, TActResult>> Act { get; }
        public Action<TActResult> Assert { get; }
    }

    /// <summary>
    /// Nongeneric model for test case
    /// </summary>
    public abstract class TestCase
    {
        public string Name { get; private set; }
        public string Identifier { get; private set; }

        public TestCase SetName(string name)
        {
            if (Name == null)
                Name = name;

            return this;
        }

        public TestCase SetTestIdentifier(string identifier)
        {
            if (Identifier == null)
                Identifier = identifier;
            else
                throw new InvalidOperationException($"Identifier for test {Identifier} already set!");

            return this;
        }
    }
}