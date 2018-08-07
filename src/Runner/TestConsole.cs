using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client.Interfaces;

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
		public void EndSession()
			=> inner.EndSession();
		public void InitializeExtensions(IEnumerable<string> pathToAdditionalExtensions)
			=> inner.InitializeExtensions(pathToAdditionalExtensions);
		public void RunTests(IEnumerable<string> sources, string runSettings, ITestRunEventsHandler testRunEventsHandler)
			=> inner.RunTests(sources, runSettings, testRunEventsHandler);
		public void RunTests(IEnumerable<string> sources, string runSettings, TestPlatformOptions options, ITestRunEventsHandler testRunEventsHandler)
			=> inner.RunTests(sources, runSettings, options, testRunEventsHandler);
		public void RunTests(IEnumerable<TestCase> testCases, string runSettings, ITestRunEventsHandler testRunEventsHandler)
			=> inner.RunTests(testCases, runSettings, testRunEventsHandler);
		public void RunTests(IEnumerable<TestCase> testCases, string runSettings, TestPlatformOptions options, ITestRunEventsHandler testRunEventsHandler)
			=> inner.RunTests(testCases, runSettings, options, testRunEventsHandler);
		public void RunTestsWithCustomTestHost(IEnumerable<string> sources, string runSettings, ITestRunEventsHandler testRunEventsHandler, ITestHostLauncher customTestHostLauncher)
			=> inner.RunTestsWithCustomTestHost(sources, runSettings, testRunEventsHandler, customTestHostLauncher);
		public void RunTestsWithCustomTestHost(IEnumerable<string> sources, string runSettings, TestPlatformOptions options, ITestRunEventsHandler testRunEventsHandler, ITestHostLauncher customTestHostLauncher)
			=> inner.RunTestsWithCustomTestHost(sources, runSettings, options, testRunEventsHandler, customTestHostLauncher);
		public void RunTestsWithCustomTestHost(IEnumerable<TestCase> testCases, string runSettings, ITestRunEventsHandler testRunEventsHandler, ITestHostLauncher customTestHostLauncher)
			=> inner.RunTestsWithCustomTestHost(testCases, runSettings, testRunEventsHandler, customTestHostLauncher);
		public void RunTestsWithCustomTestHost(IEnumerable<TestCase> testCases, string runSettings, TestPlatformOptions options, ITestRunEventsHandler testRunEventsHandler, ITestHostLauncher customTestHostLauncher)
			=> inner.RunTestsWithCustomTestHost(testCases, runSettings, options, testRunEventsHandler, customTestHostLauncher);
		public void StartSession()
			=> inner.StartSession();

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

			public void Dispose()
			{
				AppDomain.CurrentDomain.AssemblyResolve -= ResolveVSTestAssembly;
			}

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
