namespace System.Collections.Generic {
	/// <summary>
	/// Factory for <see cref="One{T}"./>
	/// </summary>
	public static class One {
		/// <summary>
		/// Initializes a new instance of the <see cref="One{T}"/> class that contains the specified value.
		/// </summary>
		/// <typeparam name="TValue">The type of value in the instance.</typeparam>
		/// <param name="value">The value to be stored in the <see cref="One{T}"/>.</param>
		/// <returns>A new instance of <see cref="One{T}"/> that contains the specified value.</returns>
		public static One<TValue> Value<TValue>(TValue value) => new One<TValue>(value);
	}

	/// <summary>
	/// Represents a collection of exactly one object. Provides special LINQ methods to write closure-like statements.
	/// </summary>
	/// <typeparam name="T">The type of value in the instance.</typeparam>
	public class One<T> : ICollection<T> {
		// Internal value storage.
		protected readonly List<T> _list;

		/// <summary>
		/// Gets the single value stored in <see cref="One{T}"/>.
		/// </summary>
		public T Value => _list[0];

		/// <summary>
		/// Initializes a new instance of the <see cref="One{T}"/> class that contains the specified value.
		/// </summary>
		/// <param name="value">The value to be stored in the <see cref="One{T}"/>.</param>
		public One(T value) {
			_list = new List<T> { value };
		}

		#region ICollection<T> implementation
		/// <summary>
		/// Gets the number of elements contained in the <see cref="One{T}"/>. Always return 1.
		/// </summary>
		public int Count => 1;

		/// <summary>
		/// Gets a value indicating whether the <see cref="One{T}"/> is read-only. Always return true.
		/// </summary>
		public bool IsReadOnly => true;

		[Obsolete("Cannot add item. One<T> is readonly.", error: true)]
		public void Add(T item) => throw new InvalidOperationException($"Cannot add item. {typeof(One<T>).Name} is readonly.");

		[Obsolete("Cannot clear collection. One<T> is readonly.", error: true)]
		public void Clear() => throw new InvalidOperationException($"Cannot clear collection. {typeof(One<T>).Name} is readonly.");

		/// <summary>
		/// Determines whether the <see cref="One{T}"/> contains a specific value.
		/// </summary>
		/// <param name="item">The object to locate in the <see cref="One{T}"/>.</param>
		/// <returns>true if item is found in the <see cref="One{T}"/>; otherwise, false.</returns>
		public bool Contains(T item) => EqualityComparer<T>.Default.Equals(Value, item);

		/// <summary>
		/// Copies the elements of <see cref="One{T}"/> to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the element copied from <see cref="One{T}"/>. The <see cref="Array"/> must have zero-based indexing.</param>
		/// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
		public void CopyTo(T[] array, int arrayIndex) => array[arrayIndex] = Value;

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>An enumerator that can be used to iterate through the collection.</returns>
		public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

		[Obsolete("Cannot remove item. One<T> is readonly.", error: true)]
		public bool Remove(T item) => throw new InvalidOperationException($"Cannot remove item. {typeof(One<T>).Name} is readonly.");

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>An enumerator that can be used to iterate through the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
		#endregion

		protected virtual T UnboxValue() => Value;

		/// <summary>
		/// Implicitly converts <see cref="One{T}"/> to value stored in the <see cref="One{T}"/>.
		/// </summary>
		/// <param name="one">The <see cref="One{T}"/> to be converted.</param>
		public static implicit operator T(One<T> one) {
			return one.UnboxValue();
		}
	}
}
