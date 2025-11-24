using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Tests {
	public class DisposableTests {
		[Fact]
		public void LinqClosureDisposesAllAllocatedResourcesOnSelect() {
			DummyResource1 resource1 = new("Foo");
			DummyResource2 resource2 = new("Bar");
			DummyResource3? resource3Ref = null;
			DummyResource4? resource4Ref = null;

			string result = from res1 in One.Value(resource1)
							from res2 in One.Value(resource2)
							let res3 = resource3Ref = new DummyResource3("Wkwk")
							let res4 = resource4Ref = new DummyResource4("Akwoakoawk")
							let temp = $"{res3.Text} {res4.Text}"
							select $"{res1.Text} {res2.Text} {temp}";
			result.ShouldBe("Foo Bar Wkwk Akwoakoawk");

			resource1.Disposed.ShouldBeTrue();
			resource2.Disposed.ShouldBeTrue();
			resource3Ref.ShouldNotBeNull();
			resource3Ref.Disposed.ShouldBeTrue();
			resource4Ref.ShouldNotBeNull();
			resource4Ref.Disposed.ShouldBeTrue();
		}

		[Fact]
		public void OneCanContainNullableType() {
			// Test that One<T?> properly handles nullable types without direct assignment to non-nullable
			One<string?> nullableString = One.Value<string?>(null);
			nullableString.Value.ShouldBeNull();

			One<int?> nullableInt = One.Value<int?>(null);
			nullableInt.Value.ShouldBeNull();

			One<int?> nonNullInt = One.Value<int?>(42);
			nonNullInt.Value.ShouldBe(42);
		}

		[Fact]
		public void OneWithRecordAndNullConditionalOperators() {
			// Test that One works with records and null-conditional operators
			var result = from foo in new Foo(null).AsOne()
						 let length = foo.Name?.Length
						 select length?.ToString();
			
			result.Value.ShouldBeNull();
		}

		private record Foo(string? Name);

		private abstract class DummyResource : IDisposable {
			protected readonly string _text;
			public string Text {
				get {
					if (Disposed) throw new ObjectDisposedException(nameof(Text));
					return _text;
				}
			}

			public bool Disposed { get; private set; } = false;

			public DummyResource(string text) {
				_text = text;
			}

			public void Dispose() {
				if (Disposed) throw new ObjectDisposedException(nameof(Text));
				Disposed = true;
			}
		}

		private class DummyResource1 : DummyResource {
			public DummyResource1(string text) : base(text) {
			}
		}

		private class DummyResource2 : DummyResource {
			public DummyResource2(string text) : base(text) {
			}
		}

		private class DummyResource3 : DummyResource {
			public DummyResource3(string text) : base(text) {
			}
		}

		private class DummyResource4 : DummyResource {
			public DummyResource4(string text) : base(text) {
			}
		}
	}
}
