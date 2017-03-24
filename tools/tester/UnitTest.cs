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

        public IEnumerable<TestCase> MultipleTestcases()
        {
            yield return Test(
                context => context.CreateSubject(),
                subject => subject.AddTwoValues(5, 6),
                result => result.Should().Be(11)).SetName("Multiple #1");

            yield return Test(
                context => context.CreateSubject(),
                subject => subject.AddTwoValues(8, 9),
                result => result.Should().Be(17)).SetName("Multiple #2");
        }
    }
}