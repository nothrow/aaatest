using System;
using System.Linq.Expressions;

namespace aaatest.framework
{
    /// <summary>
    ///     Defines which class is being unit tested
    /// </summary>
    /// <typeparam name="TClass"></typeparam>
    public abstract class TestingClass<TClass> where TClass : class
    {
        protected TestCase Test<TActResult>(Func<TestingContext<TClass>, TClass> arrange,
            Expression<Func<TClass, TActResult>> act, Action<TActResult> assert)
        {
            return new TestCase<TClass, TActResult>(arrange, act, assert);
        }
    }

    public sealed class TestingContext<TClass>
    {
        public TClass CreateSubject()
        {
            return (TClass) Activator.CreateInstance(typeof(TClass));
        }
    }

    public abstract class TestCase
    {
        public string Name { get; private set; }

        public TestCase SetName(string name)
        {
            if (Name == null)
                Name = name;

            return this;
        }
    }

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
}