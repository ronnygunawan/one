using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Tests {
	public class DisposableTests {
		[Fact]
		public void LinqClosureDisposesAllAllocatedResourcesOnSelect() {
			DummyResource1 resource1 = new("Foo");
			DummyResource2 resource2 = new("Bar");
			DummyResource3 resource3Ref = null;
			DummyResource4 resource4Ref = null;

			string result = from res1 in One.Value(resource1)
							from res2 in One.Value(resource2)
							let res3 = resource3Ref = new DummyResource3("Wkwk")
							let res4 = resource4Ref = new DummyResource4("Akwoakoawk")
							let temp = $"{res3.Text} {res4.Text}"
							select $"{res1.Text} {res2.Text} {temp}";
			result.Should().Be("Foo Bar Wkwk Akwoakoawk");

			resource1.Disposed.Should().BeTrue();
			resource2.Disposed.Should().BeTrue();
			resource3Ref.Disposed.Should().BeTrue();
			resource4Ref.Disposed.Should().BeTrue();
		}

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
