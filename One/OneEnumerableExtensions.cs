using System.Collections.Generic;

namespace System.Linq {
	public static class OneEnumerableExtensions {
		public static One<T> AsOne<T>(this IEnumerable<T> enumerable) => new One<T>(enumerable.Single());
		public static One<T> AsOne<T>(this T value) => new One<T>(value);
		public static One<T> Cast<T>(this T value) => new One<T>(value);
		public static IEnumerable<T> Cast<T>(this IEnumerable<T> source) => Enumerable.Cast<T>(source);
	}
}
