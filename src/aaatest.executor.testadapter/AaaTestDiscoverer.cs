using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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


            foreach (var source in sources)
                using (_tracer.MeasureTrace($"Processing {source}", ts => $"Processing {source} took {ts}"))
                {
                    try
                    {
                        var asm = Assembly.Load(new AssemblyName(Path.GetFileNameWithoutExtension(source)));
                        DiscoverAssembly(discoverySink, asm, _tracer, source);
                    }
                    catch (FileLoadException flex)
                    {
                        _tracer.Error($"Unable to load {source}: {flex.Message} 0x{flex.HResult:X8}");
                    }
                }
        }

        private static void DiscoverAssembly(ITestCaseDiscoverySink discoverySink, Assembly asm, ITracer tracer,
            string source)
        {
            tracer.Debug($"Processing assembly {asm.FullName}");
            foreach (var type in asm.GetTypes().Where(IsUnitTestType))
                DiscoverType(discoverySink, tracer, type, source);
        }

        private static bool IsUnitTestType(Type type)
        {
            if (type.IsConstructedGenericType
                && type.GetGenericTypeDefinition() == typeof(TestingClass<>))
                return true;

            return type.GetTypeInfo().BaseType != null && IsUnitTestType(type.GetTypeInfo().BaseType);
        }

        private static void DiscoverType(ITestCaseDiscoverySink discoverySink, ITracer tracer, Type type, string source)
        {
            tracer.Debug($"Processing class {type.FullName}");
            var crawler = new TestClassCrawler(tracer, type);

            foreach (var test in crawler.EnumerateTests())
                discoverySink.SendTestCase(test.ConvertToVsTest(source));
        }
    }
}