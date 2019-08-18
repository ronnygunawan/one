using System.Linq;

namespace System.Collections.Generic {
	public static class OneObjectExtensions {
		/// <summary>
		/// Converts this object to a new instance of the <see cref="One{T}"/> that contains this object.
		/// </summary>
		/// <typeparam name="TValue">The type of object.</typeparam>
		/// <param name="value">The object to be converted to <see cref="One{T}"/>.</param>
		/// <param name="doNotDisposeValueOnDispose">true to let <see cref="One{T}"/> dispose value on disposing; false if you prefer to dispose it yourself.</param>
		/// <returns>A new instance of <see cref="One{T}"/> that contains this value.</returns>
		public static One<TValue> ToOne<TValue>(this TValue value, bool doNotDisposeValueOnDispose = false) => new One<TValue>(value, doNotDisposeValueOnDispose);

		/// <summary>
		/// Converts this <see cref="IEnumerable{T}"/> to a new instance of the <see cref="One{T}"/> that contains item of this <see cref="IEnumerable{T}"/>. Throws exception if this <see cref="IEnumerable{T}"/> does not contain exactly one item.
		/// </summary>
		/// <typeparam name="TValue">The type of item.</typeparam>
		/// <param name="enumerable">The <see cref="IEnumerable{T}"/> to be converted to <see cref="One{T}"/>.</param>
		/// <returns>A new instance of <see cref="One{T}"/> that contains item of this <see cref="IEnumerable{T}"/>.</returns>
		public static One<TValue> AsOne<TValue>(this IEnumerable<TValue> enumerable) => new One<TValue>(enumerable.Single());
	}
}
