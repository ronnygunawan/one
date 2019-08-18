using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic {
	/// <summary>
	/// Factory for <see cref="One{T}"./>
	/// </summary>
	public static class One {
		/// <summary>
		/// Gets an instance of <see cref="One{T}"/> class that contains a single null value.
		/// </summary>
		public static One<object> Null { get; } = new One<object>(null);

		/// <summary>
		/// Initializes a new instance of the <see cref="One{T}"/> class that contains the specified value.
		/// </summary>
		/// <typeparam name="TValue">The type of value in the instance.</typeparam>
		/// <param name="value">The value to be stored in the <see cref="One{T}"/>. The value can be null for reference types.</param>
		/// <param name="doNotDisposeValueOnDispose">true to let <see cref="One{T}"/> dispose value on disposing; false if you prefer to dispose it yourself.</param>
		/// <returns>A new instance of <see cref="One{T}"/> that contains the specified value.</returns>
		public static One<TValue> Of<TValue>(TValue value, bool doNotDisposeValueOnDispose = false) => new One<TValue>(value, doNotDisposeValueOnDispose);
	}

	/// <summary>
	/// Represents a collection of exactly one object. Provides special LINQ methods to write closure-like statements.
	/// </summary>
	/// <typeparam name="T">The type of value in the instance.</typeparam>
	public sealed class One<T> : ICollection<T>, IDisposable {
		// Internal value storage.
		private readonly List<T> _list;
		private readonly List<IDisposable> _disposables;
		private readonly bool _doNotDisposeValueOnDispose;

		/// <summary>
		/// Gets the single value stored in <see cref="One{T}"/>.
		/// </summary>
		public T Value => _list[0];

		/// <summary>
		/// Initializes a new instance of the <see cref="One{T}"/> class that contains the specified value.
		/// </summary>
		/// <param name="value">The value to be stored in the <see cref="One{T}"/>. The value can be null for reference types.</param>
		/// <param name="doNotDisposeValueOnDispose">true to let <see cref="One{T}"/> dispose value on disposing; false if you prefer to dispose it yourself.</param>
		public One(T value, bool doNotDisposeValueOnDispose = false) {
			_list = new List<T> { value };
			_doNotDisposeValueOnDispose = doNotDisposeValueOnDispose;
		}

		private One(T value, params IDisposable[] disposables) {
			_list = new List<T> { value };
			_doNotDisposeValueOnDispose = false;
			_disposables = disposables.ToList();
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

		#region Linq operators
		// Support for explicit range variable type.
		// Example:
		//   from T x in e
		public One<TResult> Cast<TResult>() {
			TResult resultValue;
			try {
				resultValue = (TResult)Convert.ChangeType(Value, typeof(TResult));
			} catch {
				Dispose();
				throw;
			}
			return new One<TResult>(resultValue, this);
		}

		// Support for select clause.
		// Example:
		//   from x in e select x
		public One<TResult> Select<TResult>(Func<T, TResult> selector) {
			try {
				return new One<TResult>(selector.Invoke(Value), this);
			} catch {
				Dispose();
				throw;
			}
		}

		// Support for second from clause.
		// Example:
		//   from x1 in e1
		//   from x2 in e2
		//   select v
		public One<TResult> SelectMany<TCollection, TResult>(Func<T, One<TCollection>> collectionSelector, Func<T, TCollection, TResult> resultSelector) {
			One<TCollection> collection;
			try {
				collection = collectionSelector.Invoke(Value);
			} catch {
				Dispose();
				throw;
			}
			try {
				return new One<TResult>(resultSelector.Invoke(Value, collection), collection, this);
			} catch {
				collection.Dispose();
				Dispose();
				throw;
			}
		}

		// Support for join clause without into clause.
		// Example:
		//   from x1 in e1
		//   join x2 in e2 on 1 equals 1
		//   select v
		public One<TResult> Join<TInner, TKey, TResult>(One<TInner> inner, Func<T, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<T, TInner, TResult> resultSelector) {
			TInner innerValue = inner.Value;
			bool keysAreEqual;
			try {
				TKey outerKey = outerKeySelector(Value);
				TKey innerKey = innerKeySelector(innerValue);
				keysAreEqual = EqualityComparer<TKey>.Default.Equals(outerKey, innerKey);
			} catch {
				inner.Dispose();
				Dispose();
				throw;
			}
			if (keysAreEqual) {
				try {
					return new One<TResult>(resultSelector.Invoke(Value, inner), inner, this);
				} catch {
					inner.Dispose();
					Dispose();
					throw;
				}
			} else {
				inner.Dispose();
				Dispose();
				throw new InvalidOperationException($"Join clause on {typeof(One<T>).Name} must not produce empty result.");
			}
		}
		#endregion

		#region Invalid Linq operators
		[Obsolete("Second from clause must also enumerate a One<T>.", error: true)]
		public One<TResult> SelectMany<TCollection, TResult>(Func<T, IEnumerable<TCollection>> collectionSelector, Func<T, TCollection, TResult> resultSelector) => throw new InvalidOperationException($"Second from clause must also enumerate a One<T>.");

		[Obsolete("Where clause is not allowed on One<T>.", error: true)]
		public One<T> Where(Func<T, bool> predicate) => throw new InvalidOperationException($"Where clause is not allowed on {typeof(One<T>).Name}.");

		[Obsolete("OrderBy clause is not allowed on One<T>.", error: true)]
		public One<T> OrderBy<TKey>(Func<T, TKey> keySelector) => throw new InvalidOperationException($"OrderBy clause is not allowed on {typeof(One<T>).Name}.");

		[Obsolete("OrderByDescending clause is not allowed on One<T>.", error: true)]
		public One<T> OrderByDescending<TKey>(Func<T, TKey> keySelector) => throw new InvalidOperationException($"OrderByDescending clause is not allowed on {typeof(One<T>).Name}.");

		[Obsolete("GroupJoin clause is not allowed on One<T>.", error: true)]
		public One<TResult> GroupJoin<TInner, TKey, TResult>(One<TInner> inner, Func<T, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<T, One<TInner>, TResult> resultSelector) => throw new InvalidOperationException($"GroupJoin clause is not allowed on {typeof(One<T>).Name}.");

		[Obsolete("Join clause must enumerate a One<T>.", error: true)]
		public One<TInner> Join<TInner, TKey>(IEnumerable<TInner> inner, Func<T, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<T, TInner, T> resultSelector) => throw new InvalidOperationException($"Join clause on {typeof(One<T>).Name} must enumerate a One<T>.");

		[Obsolete("GroupBy clause is not allowed on One<T>.", error: true)]
		public IEnumerable<IGrouping<TKey, T>> GroupBy<TKey>(Func<T, TKey> keySelector) => throw new InvalidOperationException($"GroupBy clause is not allowed on {typeof(One<T>).Name}.");

		[Obsolete("GroupBy clause is not allowed on One<T>.", error: true)]
		public IEnumerable<IGrouping<TKey, TElement>> GroupBy<TKey, TElement>(Func<T, TKey> keySelector, Func<T, TElement> elementSelector) => throw new InvalidOperationException($"GroupBy clause is not allowed on {typeof(One<T>).Name}.");
		#endregion

		/// <summary>
		/// Implicitly converts <see cref="One{T}"/> to value stored in the <see cref="One{T}"/>.
		/// </summary>
		/// <param name="one">The <see cref="One{T}"/> to be converted.</param>
		public static implicit operator T(One<T> one) {
			one.DisposeDisposableValueOnly();
			return one.Value;
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		void Dispose(bool disposing) {
			if (!disposedValue) {
				if (disposing) {
					// dispose managed state (managed objects).
					if (_disposables != null && _disposables.Any()) {
						foreach (IDisposable disposable in _disposables) {
							disposable.Dispose();
						}
						_disposables.Clear();
					}
					if (!_doNotDisposeValueOnDispose) {
						if (Value is IDisposable disposableValue) {
							try {
								disposableValue.Dispose();
							} catch (ObjectDisposedException) { }
						} else if (Value != null && Value.GetType() is Type type && IsAnonymousType(type)) {
							foreach (PropertyInfo propertyInfo in type.GetTypeInfo().GetProperties().Reverse().Take(1)) {
								if (propertyInfo.PropertyType.GetInterfaces().Contains(typeof(IDisposable)) &&
									propertyInfo.GetValue(Value) is IDisposable disposable) {
									disposable.Dispose();
								}
							}
						}
					}
				}

				disposedValue = true;
			}
		}

		void DisposeDisposableValueOnly() {
			if (!disposedValue) {
				if (_disposables != null && _disposables.Any()) {
					foreach (IDisposable disposable in _disposables) {
						disposable.Dispose();
					}
					_disposables.Clear();
				}
				disposedValue = true;
			}
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose() {
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
		}

		private static bool IsAnonymousType(Type type) {
			return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
				&& type.IsGenericType && type.Name.Contains("AnonymousType")
				&& (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
				&& type.Attributes.HasFlag(TypeAttributes.NotPublic);
		}
		#endregion
	}
}
