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

			One<string> oneString = One.Of("Hello world");
			oneString.Value.Should().Be("Hello world");
		}

		[Fact]
		public void OneCanBeCreatedUsingExtensions() {
			One<int> oneInt = 1.ToOne();
			oneInt.Value.Should().Be(1);

			One<string> oneString = "Hello world".ToOne();
			oneString.Value.Should().Be("Hello world");

			One<(int, int)> oneTuple = (1, 2).ToOne();
			oneTuple.Value.Should().Be((1, 2));

			One<KeyValuePair<string, decimal>> oneKeyValuePair = new KeyValuePair<string, decimal>("iPhone X", 599m).ToOne();
			oneKeyValuePair.Value.Key.Should().Be("iPhone X");
			oneKeyValuePair.Value.Value.Should().Be(599m);

			One<double> oneDouble = new List<double> { 10.0 }.AsOne();
			oneDouble.Value.Should().Be(10.0);
		}

		[Fact]
		public void EmptyListCannotBeConvertedToOne() {
			Action action = () => new List<int>().AsOne();
			action.Should().Throw<InvalidOperationException>().WithMessage("Sequence contains no elements");
		}

		[Fact]
		public void ListContainingMoreThanOneElementCannotBeConvertedToOne() {
			Action action = () => new List<int> { 1, 2, 3 }.AsOne();
			action.Should().Throw<InvalidOperationException>().WithMessage("Sequence contains more than one element");
		}

		[Fact]
		public void OneCanBeImplicitlyCastIntoValue() {
			int intValue = 1234.ToOne();
			intValue.Should().Be(1234);
		}

		[Fact]
		public void OneSupportsLinqSyntax() {
			string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			var randomCharacter = from seed in DateTime.UtcNow.Millisecond.ToOne()
								  let random = new Random(seed)
								  let index = random.Next(26)
								  select characters[index];
			randomCharacter.GetType().Should().Be(typeof(One<char>));
		}
	}
}
