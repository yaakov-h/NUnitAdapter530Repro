using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Runner
{
	class Program
	{
		public static async Task<int> Main(string[] args)
		{
			var pathToVSTest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages", "microsoft.testplatform", "15.8.0", "tools", "net451", "Common7", "IDE", "Extensions", "TestPlatform");
			if (!Directory.Exists(pathToVSTest))
			{
				Console.Error.WriteLine("VSTest was not found in the current user's NuGet cache. Please run \"dotnet restore\" on this project.");
				return -1;
			}

			var assembliesToTest = new[]
			{
				Path.Combine("..", "..", "Tests", "net46", "Tests.dll"),
				Path.Combine("..", "..", "Tests", "netcoreapp2.0", "Tests.dll")
			};

			foreach (var assembly in assembliesToTest)
			{
				await DiscoverTestsAsync(pathToVSTest, assembly);
			}

			return 0;
		}

		static readonly HashSet<string> ExpectedTests = new HashSet<string>(new[]
		{
			"Tests.Fixture.PassingTest",
			"Tests.Fixture.PassingTestCase(null)",
			"Tests.Fixture.IgnoredTest",
			"Tests.Fixture.IgnoredTestCase(null)",
			"Tests.Fixture.ExplicitTest",
			"Tests.Fixture.ExplicitTestCase(null)",
		});

		static async Task DiscoverTestsAsync(string pathToVSTest, string assemblyPath)
		{
			using (var console = TestConsole.Create(pathToVSTest, Path.GetDirectoryName(assemblyPath), debug: false))
			{
				Console.WriteLine(assemblyPath);

				var tests = await console.DiscoverTestsAsync(assemblyPath);
				var discovered = tests.ToDictionary(t => t.FullyQualifiedName, t => t);

				foreach (var name in ExpectedTests)
				{
					if (discovered.TryGetValue(name, out var test))
					{
						Console.WriteLine("\tFOUND: {0}", test);
						Console.WriteLine("\t\tTraits: {0}", string.Join(", ", test.Traits.Select(t => $"{t.Name} = \"{t.Value}\"")));
					}
					else
					{
						Console.WriteLine("\tMISSING: {0}", name);
					}
				}

				foreach (var name in discovered.Keys)
				{
					if (!ExpectedTests.Contains(name))
					{
						Console.WriteLine("\tUNEXPECTED: {0}", name);
					}
				}
			}
		}
	}
}
