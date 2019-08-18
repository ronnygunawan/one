using System;

namespace Tests {
	internal static class ScopeExtensions {
		public static T Also<T>(this T it, Action<T> action) {
			action.Invoke(it);
			return it;
		}

		public static R Let<T, R>(this T it, Func<T, R> func) {
			return func.Invoke(it);
		}
	}
}
