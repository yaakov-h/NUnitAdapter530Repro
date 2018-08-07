using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Mono.Cecil;

namespace Runner
{
	static class IVsTestConsoleWrapperExtensions
	{
		public static async Task<IEnumerable<TestCase>> DiscoverTestsAsync(this IVsTestConsoleWrapper wrapper, string assemblyPath)
		{
			var handler = new TestDiscoveryEventsHandler();
			wrapper.DiscoverTests(new[] { assemblyPath }, GetRunSettingsXml(assemblyPath), handler);
			return await handler.Task.ConfigureAwait(false);
		}

		public static async Task<IEnumerable<TestResult>> RunTestsAsync(this IVsTestConsoleWrapper wrapper, string assemblyPath, IEnumerable<TestCase> testCases)
		{
			if (!testCases.Any())
			{
				throw new ArgumentException("Must provide at least one test case to run.", nameof(testCases));
			}

			var handler = new TestRunEventsHandler();
			wrapper.RunTests(testCases, GetRunSettingsXml(assemblyPath), handler);
			return await handler.Task.ConfigureAwait(false);
		}

		class TestMessageEventHandler : ITestMessageEventHandler
		{
			public TestMessageEventHandler()
			{
			}

			public void HandleLogMessage(TestMessageLevel level, string message)
			{
			}

			public void HandleRawMessage(string rawMessage)
			{
			}
		}

		sealed class TestRunEventsHandler : TestMessageEventHandler, ITestRunEventsHandler
		{
			public TestRunEventsHandler()
				: base()
			{
			}

			readonly TaskCompletionSource<IEnumerable<TestResult>> tcs = new TaskCompletionSource<IEnumerable<TestResult>>();
			readonly HashSet<TestResult> results = new HashSet<TestResult>();

			public Task<IEnumerable<TestResult>> Task => tcs.Task;

			public void HandleTestRunComplete(TestRunCompleteEventArgs testRunCompleteArgs, TestRunChangedEventArgs lastChunkArgs, ICollection<AttachmentSet> runContextAttachments, ICollection<string> executorUris)
			{
				HandleTestResults(lastChunkArgs?.NewTestResults);

				if (testRunCompleteArgs.Error != null)
				{
					tcs.TrySetException(testRunCompleteArgs.Error);
				}
				else if (testRunCompleteArgs.IsCanceled || testRunCompleteArgs.IsAborted)
				{
					tcs.TrySetCanceled();
				}
				else
				{
					tcs.TrySetResult(results);
				}
			}

			public void HandleTestRunStatsChange(TestRunChangedEventArgs testRunChangedArgs)
			{
				HandleTestResults(testRunChangedArgs.NewTestResults);
			}

			public int LaunchProcessWithDebuggerAttached(TestProcessStartInfo testProcessStartInfo)
			{
				return -1;
			}

			void HandleTestResults(IEnumerable<TestResult> results)
			{
				if (results == null)
				{
					return;
				}

				foreach (var result in results)
				{
					this.results.Add(result);
				}
			}
		}

		sealed class TestDiscoveryEventsHandler : TestMessageEventHandler, ITestDiscoveryEventsHandler
		{
			public TestDiscoveryEventsHandler()
				: base()
			{
			}

			readonly TaskCompletionSource<IEnumerable<TestCase>> tcs = new TaskCompletionSource<IEnumerable<TestCase>>();
			readonly HashSet<TestCase> tests = new HashSet<TestCase>();

			public Task<IEnumerable<TestCase>> Task => tcs.Task;

			public void HandleDiscoveredTests(IEnumerable<TestCase> discoveredTestCases)
			{
				foreach (var test in discoveredTestCases)
				{
					tests.Add(test);
				}
			}

			public void HandleDiscoveryComplete(long totalTests, IEnumerable<TestCase> lastChunk, bool isAborted)
			{
				if (lastChunk != null)
				{
					HandleDiscoveredTests(lastChunk);
				}

				if (!isAborted)
				{
					tcs.TrySetResult(tests);
				}
				else
				{
					tcs.TrySetCanceled();
				}
			}
		}

		static string GetRunSettingsXml(string pathToAssembly)
		{
			var targetFrameworkVersion = GetTargetFrameworkVersion(pathToAssembly);

			if (targetFrameworkVersion == null)
			{
				return null;
			}

			var xml = new XElement(
				"RunSettings",
				new XElement(
					"RunConfiguration",
					new XElement(
						"TargetFrameworkVersion",
						targetFrameworkVersion)));

			return xml.ToString();
		}

		 static string GetTargetFrameworkVersion(string pathToAssembly)
		{
			using (var assembly = AssemblyDefinition.ReadAssembly(pathToAssembly))
			{
				var targetFrameworkVersion = assembly.CustomAttributes
					.FirstOrDefault(a => a.AttributeType.FullName == "System.Runtime.Versioning.TargetFrameworkAttribute")
					?.ConstructorArguments[0]
					.Value;
				return targetFrameworkVersion as string;
			}
		}
	}
}
