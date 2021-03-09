using System.Linq;

namespace System.Collections.Generic {
	public static class OneEnumerableExtensions {
		public static One<T> AsOne<T>(this IEnumerable<T> enumerable) => new One<T>(enumerable.Single());
		public static One<T> AsOne<T>(this T value) => new One<T>(value);
	}
}
