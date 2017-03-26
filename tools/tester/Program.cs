using System;
using System.Linq;
using System.Threading.Tasks;
using aaatest.executor;

namespace tester
{
    internal class Program
    {
        private static async Task MainAsync()
        {
            var executor = new Executor(ConsoleTracer.Instance, FluentAssertionsPlugin.Instance);
            var results =
                await Task.WhenAll(
                    new TestClassCrawler<UnitTest>(ConsoleTracer.Instance).EnumerateTests()
                        .Select(test => executor.Execute(test).AsTask()));


            Console.WriteLine();
            foreach (var result in results)
                Console.WriteLine(
                    $"{result.TestCase.Name}({result.TestCase.Identifier}) was {result.Outcome} in {result.ExecutionTime} because {result.Exception}");
        }

        private static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();

            Console.ReadLine();
        }
    }
}