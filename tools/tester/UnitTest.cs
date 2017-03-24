using System.Collections.Generic;
using aaatest.framework;
using FluentAssertions;

namespace tester
{
    internal class UnitTest : TestingClass<TestingSubject>
    {
        public TestCase AddTwoValuesWorks()
        {
            return Test(
                context => context.CreateSubject(),
                subject => subject.AddTwoValues(1, 2),
                result => result.Should().Be(3));
        }

        public TestCase AddTwoValuesFails()
        {
            return Test(
                context => context.CreateSubject(),
                subject => subject.AddTwoValues(1, 2),
                result => result.Should().Be(4));
        }
    }
}