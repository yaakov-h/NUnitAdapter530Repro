using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client.Interfaces;
using Microsoft.VisualStudio.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;

namespace Runner
{
	sealed class TestConsole : IVsTestConsoleWrapper, IDisposable
	{
		TestConsole(IVsTestConsoleWrapper wrapper, IDisposable assemblyResolver)
		{
			inner = wrapper ?? throw new ArgumentNullException(nameof(wrapper));
			this.assemblyResolver = assemblyResolver ?? throw new ArgumentNullException(nameof(assemblyResolver));
		}

		readonly IVsTestConsoleWrapper inner;
		readonly IDisposable assemblyResolver;

		public static IDisposable ResolveAssemblies(string pathToVSTest) => new VSTestAssemblyResolver(pathToVSTest);

		public static TestConsole Create(string pathToVSTest, string pathToAssemblyDirectory, bool debug)
		{
			var resolver = ResolveAssemblies(pathToVSTest);
			var vstestConsolePath = Path.Combine(pathToVSTest, "vstest.console.exe");

			var parameters = new ConsoleParameters();
			if (debug)
			{
				parameters.LogFilePath = Path.Combine(Path.GetTempPath(), $"VSTestAdapter_{DateTime.Now:yyyyMMdd-HHmmss}.log");
			}

			var console = new VsTestConsoleWrapper(vstestConsolePath, parameters);
			console.StartSession();

			var extensions = Directory.GetFiles(pathToAssemblyDirectory, "*.testadapter.dll", SearchOption.TopDirectoryOnly);
			console.InitializeExtensions(extensions);

			return new TestConsole(console, resolver);
		}

        #region IVsTestConsoleWrapper
#pragma warning disable CS0618 // Type or member is obsolete

        public void AbortTestRun()
            => inner.AbortTestRun();
        public void CancelDiscovery()
            => inner.CancelDiscovery();
        public void CancelTestRun()
            => inner.CancelTestRun();
        public void DiscoverTests(IEnumerable<string> sources, string discoverySettings, ITestDiscoveryEventsHandler discoveryEventsHandler)
            => inner.DiscoverTests(sources, discoverySettings, discoveryEventsHandler);
        public void DiscoverTests(IEnumerable<string> sources, string discoverySettings, TestPlatformOptions options, ITestDiscoveryEventsHandler2 discoveryEventsHandler)
            => inner.DiscoverTests(sources, discoverySettings, options, discoveryEventsHandler);
        public void DiscoverTests(IEnumerable<string> sources, string discoverySettings, TestPlatformOptions options, TestSessionInfo testSessionInfo, ITestDiscoveryEventsHandler2 discoveryEventsHandler)
            => inner.DiscoverTests(sources, discoverySettings, options, testSessionInfo, discoveryEventsHandler);
        public Task DiscoverTestsAsync(IEnumerable<string> sources, string discoverySettings, ITestDiscoveryEventsHandler discoveryEventsHandler)
            => inner.DiscoverTestsAsync(sources, discoverySettings, discoveryEventsHandler);
        public Task DiscoverTestsAsync(IEnumerable<string> sources, string discoverySettings, TestPlatformOptions options, ITestDiscoveryEventsHandler2 discoveryEventsHandler)
            => inner.DiscoverTestsAsync(sources, discoverySettings, options, discoveryEventsHandler);
        public Task DiscoverTestsAsync(IEnumerable<string> sources, string discoverySettings, TestPlatformOptions options, TestSessionInfo testSessionInfo, ITestDiscoveryEventsHandler2 discoveryEventsHandler)
            => inner.DiscoverTestsAsync(sources, discoverySettings, options, testSessionInfo, discoveryEventsHandler);
        public void EndSession()
            => inner.EndSession();
        public void InitializeExtensions(IEnumerable<string> pathToAdditionalExtensions)
            => inner.InitializeExtensions(pathToAdditionalExtensions);
        public Task InitializeExtensionsAsync(IEnumerable<string> pathToAdditionalExtensions)
            => inner.InitializeExtensionsAsync(pathToAdditionalExtensions);
        public Task ProcessTestRunAttachmentsAsync(IEnumerable<AttachmentSet> attachments, string processingSettings, bool isLastBatch, bool collectMetrics, ITestRunAttachmentsProcessingEventsHandler eventsHandler, CancellationToken cancellationToken)
            => inner.ProcessTestRunAttachmentsAsync(attachments, processingSettings, isLastBatch, collectMetrics, eventsHandler, cancellationToken);
        public Task ProcessTestRunAttachmentsAsync(IEnumerable<AttachmentSet> attachments, IEnumerable<InvokedDataCollector> invokedDataCollectors, string processingSettings, bool isLastBatch, bool collectMetrics, ITestRunAttachmentsProcessingEventsHandler eventsHandler, CancellationToken cancellationToken)
            => inner.ProcessTestRunAttachmentsAsync(attachments, invokedDataCollectors, processingSettings, isLastBatch, collectMetrics, eventsHandler, cancellationToken);
        public void RunTests(IEnumerable<string> sources, string runSettings, ITestRunEventsHandler testRunEventsHandler)
            => inner.RunTests(sources, runSettings, testRunEventsHandler);
        public void RunTests(IEnumerable<string> sources, string runSettings, TestPlatformOptions options, ITestRunEventsHandler testRunEventsHandler)
            => inner.RunTests(sources, runSettings, options, testRunEventsHandler);
        public void RunTests(IEnumerable<string> sources, string runSettings, TestPlatformOptions options, TestSessionInfo testSessionInfo, ITestRunEventsHandler testRunEventsHandler)
            => inner.RunTests(sources, runSettings, options, testSessionInfo, testRunEventsHandler);
        public void RunTests(IEnumerable<TestCase> testCases, string runSettings, ITestRunEventsHandler testRunEventsHandler)
            => inner.RunTests(testCases, runSettings, testRunEventsHandler);
        public void RunTests(IEnumerable<TestCase> testCases, string runSettings, TestPlatformOptions options, ITestRunEventsHandler testRunEventsHandler)
            => inner.RunTests(testCases, runSettings, options, testRunEventsHandler);
        public void RunTests(IEnumerable<TestCase> testCases, string runSettings, TestPlatformOptions options, TestSessionInfo testSessionInfo, ITestRunEventsHandler testRunEventsHandler)
            => inner.RunTests(testCases, runSettings, options, testSessionInfo, testRunEventsHandler);
        public Task RunTestsAsync(IEnumerable<string> sources, string runSettings, ITestRunEventsHandler testRunEventsHandler)
            => inner.RunTestsAsync(sources, runSettings, testRunEventsHandler);
        public Task RunTestsAsync(IEnumerable<string> sources, string runSettings, TestPlatformOptions options, ITestRunEventsHandler testRunEventsHandler)
            => inner.RunTestsAsync(sources, runSettings, options, testRunEventsHandler);
        public Task RunTestsAsync(IEnumerable<string> sources, string runSettings, TestPlatformOptions options, TestSessionInfo testSessionInfo, ITestRunEventsHandler testRunEventsHandler)
            => inner.RunTestsAsync(sources, runSettings, options, testSessionInfo, testRunEventsHandler);
        public Task RunTestsAsync(IEnumerable<TestCase> testCases, string runSettings, ITestRunEventsHandler testRunEventsHandler)
            => inner.RunTestsAsync(testCases, runSettings, testRunEventsHandler);
        public Task RunTestsAsync(IEnumerable<TestCase> testCases, string runSettings, TestPlatformOptions options, ITestRunEventsHandler testRunEventsHandler)
            => inner.RunTestsAsync(testCases, runSettings, options, testRunEventsHandler);
        public Task RunTestsAsync(IEnumerable<TestCase> testCases, string runSettings, TestPlatformOptions options, TestSessionInfo testSessionInfo, ITestRunEventsHandler testRunEventsHandler)
            => inner.RunTestsAsync(testCases, runSettings, options, testSessionInfo, testRunEventsHandler);
        public void RunTestsWithCustomTestHost(IEnumerable<string> sources, string runSettings, ITestRunEventsHandler testRunEventsHandler, ITestHostLauncher customTestHostLauncher)
            => inner.RunTestsWithCustomTestHost(sources, runSettings, testRunEventsHandler, customTestHostLauncher);
        public void RunTestsWithCustomTestHost(IEnumerable<string> sources, string runSettings, TestPlatformOptions options, ITestRunEventsHandler testRunEventsHandler, ITestHostLauncher customTestHostLauncher)
            => inner.RunTestsWithCustomTestHost(sources, runSettings, options, testRunEventsHandler, customTestHostLauncher);
        public void RunTestsWithCustomTestHost(IEnumerable<string> sources, string runSettings, TestPlatformOptions options, TestSessionInfo testSessionInfo, ITestRunEventsHandler testRunEventsHandler, ITestHostLauncher customTestHostLauncher)
            => inner.RunTestsWithCustomTestHost(sources, runSettings, options, testSessionInfo, testRunEventsHandler, customTestHostLauncher);
        public void RunTestsWithCustomTestHost(IEnumerable<TestCase> testCases, string runSettings, ITestRunEventsHandler testRunEventsHandler, ITestHostLauncher customTestHostLauncher)
            => inner.RunTestsWithCustomTestHost(testCases, runSettings, testRunEventsHandler, customTestHostLauncher);
        public void RunTestsWithCustomTestHost(IEnumerable<TestCase> testCases, string runSettings, TestPlatformOptions options, ITestRunEventsHandler testRunEventsHandler, ITestHostLauncher customTestHostLauncher)
            => inner.RunTestsWithCustomTestHost(testCases, runSettings, options, testRunEventsHandler, customTestHostLauncher);
        public void RunTestsWithCustomTestHost(IEnumerable<TestCase> testCases, string runSettings, TestPlatformOptions options, TestSessionInfo testSessionInfo, ITestRunEventsHandler testRunEventsHandler, ITestHostLauncher customTestHostLauncher)
            => inner.RunTestsWithCustomTestHost(testCases, runSettings, options, testSessionInfo, testRunEventsHandler, customTestHostLauncher);
        public Task RunTestsWithCustomTestHostAsync(IEnumerable<string> sources, string runSettings, ITestRunEventsHandler testRunEventsHandler, ITestHostLauncher customTestHostLauncher)
            => inner.RunTestsWithCustomTestHostAsync(sources, runSettings, testRunEventsHandler, customTestHostLauncher);
        public Task RunTestsWithCustomTestHostAsync(IEnumerable<string> sources, string runSettings, TestPlatformOptions options, ITestRunEventsHandler testRunEventsHandler, ITestHostLauncher customTestHostLauncher)
            => inner.RunTestsWithCustomTestHostAsync(sources, runSettings, options, testRunEventsHandler, customTestHostLauncher);
        public Task RunTestsWithCustomTestHostAsync(IEnumerable<string> sources, string runSettings, TestPlatformOptions options, TestSessionInfo testSessionInfo, ITestRunEventsHandler testRunEventsHandler, ITestHostLauncher customTestHostLauncher)
            => inner.RunTestsWithCustomTestHostAsync(sources, runSettings, options, testSessionInfo, testRunEventsHandler, customTestHostLauncher);
        public Task RunTestsWithCustomTestHostAsync(IEnumerable<TestCase> testCases, string runSettings, ITestRunEventsHandler testRunEventsHandler, ITestHostLauncher customTestHostLauncher)
            => inner.RunTestsWithCustomTestHostAsync(testCases, runSettings, testRunEventsHandler, customTestHostLauncher);
        public Task RunTestsWithCustomTestHostAsync(IEnumerable<TestCase> testCases, string runSettings, TestPlatformOptions options, ITestRunEventsHandler testRunEventsHandler, ITestHostLauncher customTestHostLauncher)
            => inner.RunTestsWithCustomTestHostAsync(testCases, runSettings, options, testRunEventsHandler, customTestHostLauncher);
        public Task RunTestsWithCustomTestHostAsync(IEnumerable<TestCase> testCases, string runSettings, TestPlatformOptions options, TestSessionInfo testSessionInfo, ITestRunEventsHandler testRunEventsHandler, ITestHostLauncher customTestHostLauncher)
            => inner.RunTestsWithCustomTestHostAsync(testCases, runSettings, options, testSessionInfo, testRunEventsHandler, customTestHostLauncher);
        public void StartSession()
            => inner.StartSession();
        public ITestSession StartTestSession(IList<string> sources, string runSettings, ITestSessionEventsHandler eventsHandler)
            => inner.StartTestSession(sources, runSettings, eventsHandler);
        public ITestSession StartTestSession(IList<string> sources, string runSettings, TestPlatformOptions options, ITestSessionEventsHandler eventsHandler)
            => inner.StartTestSession(sources, runSettings, options, eventsHandler);
        public ITestSession StartTestSession(IList<string> sources, string runSettings, TestPlatformOptions options, ITestSessionEventsHandler eventsHandler, ITestHostLauncher testHostLauncher)
            => inner.StartTestSession(sources, runSettings, options, eventsHandler, testHostLauncher);
        public Task StartSessionAsync()
            => inner.StartSessionAsync();
        public Task<ITestSession> StartTestSessionAsync(IList<string> sources, string runSettings, ITestSessionEventsHandler eventsHandler)
            => inner.StartTestSessionAsync(sources, runSettings, eventsHandler);
        public Task<ITestSession> StartTestSessionAsync(IList<string> sources, string runSettings, TestPlatformOptions options, ITestSessionEventsHandler eventsHandler)
            => inner.StartTestSessionAsync(sources, runSettings, options, eventsHandler);
        public Task<ITestSession> StartTestSessionAsync(IList<string> sources, string runSettings, TestPlatformOptions options, ITestSessionEventsHandler eventsHandler, ITestHostLauncher testHostLauncher)
            => inner.StartTestSessionAsync(sources, runSettings, options, eventsHandler, testHostLauncher);
        public bool StopTestSession(TestSessionInfo testSessionInfo, ITestSessionEventsHandler eventsHandler)
            => inner.StopTestSession(testSessionInfo, eventsHandler);
        public bool StopTestSession(TestSessionInfo testSessionInfo, TestPlatformOptions options, ITestSessionEventsHandler eventsHandler)
            => inner.StopTestSession(testSessionInfo, options, eventsHandler);
        public Task<bool> StopTestSessionAsync(TestSessionInfo testSessionInfo, ITestSessionEventsHandler eventsHandler)
            => inner.StopTestSessionAsync(testSessionInfo, eventsHandler);
        public Task<bool> StopTestSessionAsync(TestSessionInfo testSessionInfo, TestPlatformOptions options, ITestSessionEventsHandler eventsHandler)
            => inner.StopTestSessionAsync(testSessionInfo, options, eventsHandler);

#pragma warning restore CS0618 // Type or member is obsolete
        #endregion

        #region IDisposable

        public void Dispose()
        {
            inner.EndSession();
            assemblyResolver.Dispose();
        }
        #endregion

        sealed class VSTestAssemblyResolver : IDisposable
		{
			public VSTestAssemblyResolver(string pathToVSTest)
			{
				this.pathToVSTest = pathToVSTest;
				AppDomain.CurrentDomain.AssemblyResolve += ResolveVSTestAssembly;
			}

			readonly string pathToVSTest;

            public void Dispose() => AppDomain.CurrentDomain.AssemblyResolve -= ResolveVSTestAssembly;

            Assembly ResolveVSTestAssembly(object sender, ResolveEventArgs args)
			{
				var assemblyPath = Path.Combine(pathToVSTest, new AssemblyName(args.Name).Name + ".dll");
				if (!File.Exists(assemblyPath))
				{
					return null;
				}
				return Assembly.LoadFrom(assemblyPath);
			}
		}
	}
}
