using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Tests {
	public class UnitTests {
		[Fact]
		public void OneCanBeCreatedUsingConstructor() {
			One<int> oneInt = new(1);
			oneInt.Value.ShouldBe(1);

			One<string> oneString = new("Hello world");
			oneString.Value.ShouldBe("Hello world");

			One<(int, int)> oneTuple = new((1, 2));
			oneTuple.Value.ShouldBe((1, 2));

			One<KeyValuePair<string, decimal>> oneKeyValuePair = new(new KeyValuePair<string, decimal>("iPhone X", 599m));
			oneKeyValuePair.Value.Key.ShouldBe("iPhone X");
			oneKeyValuePair.Value.Value.ShouldBe(599m);
		}

		[Fact]
		public void OneCanBeCreatedUsingFactory() {
			One<string> oneString = One.Value("Hello world");
			oneString.Value.ShouldBe("Hello world");
		}

		[Fact]
		public void OneCanBeImplicitlyCastIntoValue() {
			int intValue = One.Value(1234);
			intValue.ShouldBe(1234);
		}

		[Fact]
		public void OneSupportsLinqSyntax() {
			string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			var randomCharacter = from seed in One.Value(DateTime.UtcNow.Millisecond)
								  let random = new Random(seed)
								  let index = random.Next(26)
								  select characters[index];
			randomCharacter.GetType().ShouldBe(typeof(Qlosure<char>));
		}
	}
}
