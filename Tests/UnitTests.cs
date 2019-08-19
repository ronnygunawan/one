using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Tests {
	public class UnitTests {
		[Fact]
		public void OneCanBeCreatedUsingConstructor() {
			One<int> oneInt = new One<int>(1);
			oneInt.Value.Should().Be(1);

			One<string> oneString = new One<string>("Hello world");
			oneString.Value.Should().Be("Hello world");

			One<(int, int)> oneTuple = new One<(int, int)>((1, 2));
			oneTuple.Value.Should().Be((1, 2));

			One<KeyValuePair<string, decimal>> oneKeyValuePair = new One<KeyValuePair<string, decimal>>(new KeyValuePair<string, decimal>("iPhone X", 599m));
			oneKeyValuePair.Value.Key.Should().Be("iPhone X");
			oneKeyValuePair.Value.Value.Should().Be(599m);
		}

		[Fact]
		public void OneCanBeCreatedUsingFactory() {
			One<object> oneNull = One.Null;
			oneNull.Value.Should().BeNull();

			One<string> oneString = One.Value("Hello world");
			oneString.Value.Should().Be("Hello world");
		}

		[Fact]
		public void OneCanBeImplicitlyCastIntoValue() {
			int intValue = One.Value(1234);
			intValue.Should().Be(1234);
		}

		[Fact]
		public void OneSupportsLinqSyntax() {
			string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			var randomCharacter = from seed in One.Value(DateTime.UtcNow.Millisecond)
								  let random = new Random(seed)
								  let index = random.Next(26)
								  select characters[index];
			randomCharacter.GetType().Should().Be(typeof(Qlosure<char>));
		}
	}
}
