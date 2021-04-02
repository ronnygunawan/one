using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Linq {
	public class Qlosure<T> : One<T>, IDisposable {
		protected List<IDisposable> _disposables;

		/// <summary>
		/// Gets whether <see cref="Qlosure{T}"/> will try to dispose value on Dispose().
		/// </summary>
		public bool DisposeValueOnDispose { get; private set; } = true;

		/// <summary>
		/// Initializes a new instance of the <see cref="Qlosure{T}"/>.
		/// </summary>
		/// <param name="value">The value to be stored in the <see cref="Qlosure{T}"/>. The value can be null for reference types.</param>
		public Qlosure(T value) : base(value) { }

		public Qlosure(T value, params IDisposable[] disposables) : base(value) {
			_disposables = disposables.ToList();
		}

		/// <summary>
		/// Sets <see cref="Qlosure{T}"/> to not dispose value on Dispose().
		/// </summary>
		/// <returns>This <see cref="Qlosure{T}"/> object.</returns>
		public Qlosure<T> DoNotDisposeValue() {
			DisposeValueOnDispose = false;
			return this;
		}

		#region Linq operators
		// Support for explicit range variable type.
		// Example:
		//   from T x in e
		public Qlosure<TResult> Cast<TResult>() {
			TResult resultValue;
			try {
				resultValue = (TResult)Convert.ChangeType(Value, typeof(TResult));
			} catch {
				Dispose();
				throw;
			}
			return new Qlosure<TResult>(resultValue, this);
		}

		// Support for select clause.
		// Example:
		//   from x in e select x
		public Qlosure<TResult> Select<TResult>(Func<T, TResult> selector) {
			try {
				return new Qlosure<TResult>(selector.Invoke(Value), this);
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
		public Qlosure<TResult> SelectMany<TCollection, TResult>(Func<T, Qlosure<TCollection>> collectionSelector, Func<T, TCollection, TResult> resultSelector) {
			Qlosure<TCollection> collection;
			try {
				collection = collectionSelector.Invoke(Value);
			} catch {
				Dispose();
				throw;
			}
			try {
				return new Qlosure<TResult>(resultSelector.Invoke(Value, collection.Value), collection, this);
			} catch {
				collection.Dispose();
				Dispose();
				throw;
			}
		}

		// Support for second from clause.
		// Example:
		//   from x1 in e1
		//   from x2 in e2
		//   select v
		public Qlosure<TResult> SelectMany<TCollection, TResult>(Func<T, One<TCollection>> collectionSelector, Func<T, TCollection, TResult> resultSelector) {
			try {
				One<TCollection> collection = collectionSelector.Invoke(Value);
				return new Qlosure<TResult>(resultSelector.Invoke(Value, collection.Value), this);
			} catch {
				Dispose();
				throw;
			}
		}
		#endregion

		protected override T UnboxValue() {
			Dispose(true, alsoDisposeValue: false);
			return base.UnboxValue();
		}

		/// <summary>
		/// Implicitly converts <see cref="Qlosure{T}"/> to value stored in the <see cref="Qlosure{T}"/>.
		/// </summary>
		/// <param name="qlosure">The <see cref="Qlosure{T}"/> to be converted.</param>
		public static implicit operator T(Qlosure<T> qlosure) {
			return qlosure.UnboxValue();
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		private void Dispose(bool disposing, bool alsoDisposeValue) {
			if (!disposedValue) {
				if (disposing) {
					// dispose managed state (managed objects).
					if (_disposables != null && _disposables.Any()) {
						foreach (IDisposable disposable in _disposables) {
							disposable.Dispose();
						}
						_disposables.Clear();
					}
					if (alsoDisposeValue && DisposeValueOnDispose) {
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

		// This code added to correctly implement the disposable pattern.
		public void Dispose() {
			// Do not change this code. Put cleanup code in Dispose(bool disposing, bool alsoDisposeValue) above.
			Dispose(true, true);
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
