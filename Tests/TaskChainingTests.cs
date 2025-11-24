using Shouldly;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Tests {
	public class TaskChainingTests {
		[Fact]
		public async Task TaskChainingWorksWithLinqSyntax() {
			// Simulate async operations
			var result = await from num in GetNumberAsync()
			                   from doubled in DoubleAsync(num)
			                   select doubled;

			result.ShouldBe(84);
		}

		[Fact]
		public async Task TaskChainingWorksWithMultipleSteps() {
			var result = await from num in GetNumberAsync()
			                   from doubled in DoubleAsync(num)
			                   from added in AddTenAsync(doubled)
			                   select added;

			result.ShouldBe(94);
		}

		[Fact]
		public async Task TaskChainingWorksWithTransformations() {
			var result = await from num in GetNumberAsync()
			                   from str in ConvertToStringAsync(num)
			                   select str;

			result.ShouldBe("42");
		}

		[Fact]
		public async Task TaskChainingWorksWithSelectClause() {
			var result = await from num in GetNumberAsync()
			                   from doubled in DoubleAsync(num)
			                   select doubled * 2;

			result.ShouldBe(168);
		}

		[Fact]
		public async Task TaskChainingWorksLikeHttpExample() {
			// This simulates the example from the issue:
			// await from response in httpClient.GetAsync(uri)
			//       from json in response.Content.ReadAsStringAsync()
			//       select JsonConvert.DeserializeObject<Person>(json);

			var person = await from response in GetMockHttpResponseAsync()
			                   from json in GetJsonFromResponseAsync(response)
			                   select ParsePerson(json);

			person.Name.ShouldBe("John Doe");
			person.Age.ShouldBe(30);
		}

		private static Task<int> GetNumberAsync() {
			return Task.FromResult(42);
		}

		private static Task<int> DoubleAsync(int value) {
			return Task.FromResult(value * 2);
		}

		private static Task<int> AddTenAsync(int value) {
			return Task.FromResult(value + 10);
		}

		private static Task<string> ConvertToStringAsync(int value) {
			return Task.FromResult(value.ToString());
		}

		private static Task<MockHttpResponse> GetMockHttpResponseAsync() {
			return Task.FromResult(new MockHttpResponse { 
				Content = new MockHttpContent { Data = "{\"name\":\"John Doe\",\"age\":30}" }
			});
		}

		private static Task<string> GetJsonFromResponseAsync(MockHttpResponse response) {
			return Task.FromResult(response.Content.Data);
		}

		private static Person ParsePerson(string json) {
			// Simple JSON parsing for test purposes
			var nameStart = json.IndexOf("\"name\":\"") + 8;
			var nameEnd = json.IndexOf("\"", nameStart);
			var name = json.Substring(nameStart, nameEnd - nameStart);

			var ageStart = json.IndexOf("\"age\":") + 6;
			var ageEnd = json.IndexOf("}", ageStart);
			var age = int.Parse(json.Substring(ageStart, ageEnd - ageStart));

			return new Person { Name = name, Age = age };
		}

		private class MockHttpResponse {
			public MockHttpContent Content { get; set; } = null!;
		}

		private class MockHttpContent {
			public string Data { get; set; } = null!;
		}

		private class Person {
			public string Name { get; set; } = null!;
			public int Age { get; set; }
		}
	}
}
