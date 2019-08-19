using System.Linq;

namespace System.Collections.Generic {
	public static class QlosureOneExtensions {
		public static Qlosure<T> ToQlosure<T>(this One<T> one) {
			return new Qlosure<T>(one.Value);
		}

		public static Qlosure<T> DoNotDisposeValue<T>(this One<T> one) {
			return new Qlosure<T>(one.Value).DoNotDisposeValue();
		}

		public static Qlosure<TResult> Select<TSource, TResult>(this One<TSource> one, Func<TSource, TResult> selector) {
			return new Qlosure<TResult>(selector.Invoke(one.Value), one.ToQlosure());
		}

		public static Qlosure<TResult> SelectMany<TSource, TCollection, TResult>(this One<TSource> one, Func<TSource, Qlosure<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector) {
			Qlosure<TCollection> collection = collectionSelector.Invoke(one.Value);
			try {
				return new Qlosure<TResult>(resultSelector.Invoke(one.Value, collection.Value), collection, one.ToQlosure());
			} catch {
				collection.Dispose();
				throw;
			}
		}

		public static Qlosure<TResult> SelectMany<TSource, TCollection, TResult>(this One<TSource> one, Func<TSource, One<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector) {
			One<TCollection> collection = collectionSelector.Invoke(one.Value);
			return new Qlosure<TResult>(resultSelector.Invoke(one.Value, collection.Value), one.ToQlosure());
		}

		#region Invalid Linq operators
		[Obsolete("Second from clause must also enumerate a One<T>.", error: true)]
		public static Qlosure<TResult> SelectMany<TSource, TCollection, TResult>(this One<TSource> one, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector) => throw new InvalidOperationException($"Second from clause must also enumerate a One<T>.");

		[Obsolete("Where clause is not allowed on One<T>.", error: true)]
		public static Qlosure<TSource> Where<TSource>(this One<TSource> one, Func<TSource, bool> predicate) => throw new InvalidOperationException($"Where clause is not allowed on {typeof(One<TSource>).Name}.");

		[Obsolete("OrderBy clause is not allowed on One<T>.", error: true)]
		public static Qlosure<TSource> OrderBy<TSource, TKey>(this One<TSource> one, Func<TSource, TKey> keySelector) => throw new InvalidOperationException($"OrderBy clause is not allowed on {typeof(One<TSource>).Name}.");

		[Obsolete("OrderByDescending clause is not allowed on One<T>.", error: true)]
		public static Qlosure<TSource> OrderByDescending<TSource, TKey>(Func<TSource, TKey> keySelector) => throw new InvalidOperationException($"OrderByDescending clause is not allowed on {typeof(One<TSource>).Name}.");

		[Obsolete("GroupJoin clause is not allowed on One<T>.", error: true)]
		public static Qlosure<TResult> GroupJoin<TSource, TInner, TKey, TResult>(this One<TSource> one, One<TInner> inner, Func<TSource, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TSource, One<TInner>, TResult> resultSelector) => throw new InvalidOperationException($"GroupJoin clause is not allowed on {typeof(One<TSource>).Name}.");

		[Obsolete("Join clause is not allowed on a One<T>.", error: true)]
		public static Qlosure<TResult> Join<TSource, TInner, TKey, TResult>(this One<TSource> one, IEnumerable<TInner> inner, Func<TSource, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TSource, TInner, TResult> resultSelector) => throw new InvalidOperationException($"Join clause is not allowed on {typeof(One<TSource>).Name}.");

		[Obsolete("GroupBy clause is not allowed on One<T>.", error: true)]
		public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this One<TSource> one, Func<TSource, TKey> keySelector) => throw new InvalidOperationException($"GroupBy clause is not allowed on {typeof(One<TSource>).Name}.");

		[Obsolete("GroupBy clause is not allowed on One<T>.", error: true)]
		public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this One<TSource> one, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) => throw new InvalidOperationException($"GroupBy clause is not allowed on {typeof(One<TSource>).Name}.");
		#endregion
	}
}
