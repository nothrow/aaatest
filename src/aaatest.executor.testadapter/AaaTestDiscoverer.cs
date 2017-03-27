using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using aaatest.framework;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace aaatest.executor.testadapter
{
    [DefaultExecutorUri(AaaTestExecutor.ExecutorUrl)]
    [UsedImplicitly]
    public class AaaTestDiscoverer : ITestDiscoverer
    {
        private ITracer _tracer;

        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger,
            ITestCaseDiscoverySink discoverySink)
        {
            _tracer = new MessageLoggerTracer(logger);

            _tracer.Debug(
                $"Test discovery starting - using {RuntimeInformation.FrameworkDescription}, PID {Process.GetCurrentProcess().Id}");

            DiscoverTestsAsync(sources, discoverySink).Wait();
        }

        private Task DiscoverTestsAsync(IEnumerable<string> sources, ITestCaseDiscoverySink discoverySink)
        {
            return Task.WhenAll(sources.Select(async source =>
            {
                using (_tracer.MeasureTrace($"Processing {source}", ts => $"Processing {source} took {ts}"))
                {
                    try
                    {
                        var asm = Assembly.Load(new AssemblyName(Path.GetFileNameWithoutExtension(source)));
                        await DiscoverAssembly(discoverySink, asm, source);
                    }
                    catch (FileLoadException flex)
                    {
                        _tracer.Error($"Unable to load {source}: {flex.Message} 0x{flex.HResult:X8}");
                    }
                }
            }));
        }

        private Task DiscoverAssembly(ITestCaseDiscoverySink discoverySink, Assembly asm,
            string source)
        {
            _tracer.Debug($"Processing assembly {asm.FullName}");
            return
                Task.WhenAll(
                    asm.GetTypes()
                        .Where(IsUnitTestType)
                        .Select(type => DiscoverType(discoverySink, type, source)));
        }

        private static bool IsUnitTestType(Type type)
        {
            if (type.IsConstructedGenericType
                && type.GetGenericTypeDefinition() == typeof(TestingClass<>))
                return true;

            return type.GetTypeInfo().BaseType != null && IsUnitTestType(type.GetTypeInfo().BaseType);
        }

        private Task DiscoverType(ITestCaseDiscoverySink discoverySink, Type type, string source)
        {
            return Task.Run(() =>
            {
                _tracer.Debug($"Processing class {type.FullName}");

                var crawler = new TestClassCrawler(_tracer, type);

                foreach (var test in crawler.EnumerateTests())
                    discoverySink.SendTestCase(test.ConvertToVsTest(source));
            });
        }
    }
}