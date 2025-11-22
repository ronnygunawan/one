using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace System.Linq {
	public static class QlosureTaskExtensions {
		/// <summary>
		/// Gets an awaiter for a <see cref="Qlosure{Task}"/> to enable await syntax.
		/// </summary>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="qlosure">The qlosure containing a task.</param>
		/// <returns>A task awaiter.</returns>
		public static TaskAwaiter<TResult> GetAwaiter<TResult>(this Qlosure<Task<TResult>> qlosure) {
			return qlosure.Value.GetAwaiter();
		}

		/// <summary>
		/// Converts a <see cref="Task{T}"/> to a <see cref="Qlosure{Task}"/> for use in LINQ expressions.
		/// </summary>
		/// <typeparam name="T">The type of the task result.</typeparam>
		/// <param name="task">The task to convert.</param>
		/// <returns>A qlosure containing the task.</returns>
		public static Qlosure<Task<T>> AsQlosure<T>(this Task<T> task) {
			return new Qlosure<Task<T>>(task);
		}

		/// <summary>
		/// Select operation for tasks in LINQ expressions.
		/// </summary>
		/// <typeparam name="TSource">The type of the source task result.</typeparam>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="task">The source task.</param>
		/// <param name="selector">The selector function.</param>
		/// <returns>A qlosure containing the result task.</returns>
		public static Qlosure<Task<TResult>> Select<TSource, TResult>(this Task<TSource> task, Func<TSource, TResult> selector) {
			async Task<TResult> AsyncSelect() {
				TSource sourceValue = await task.ConfigureAwait(false);
				return selector.Invoke(sourceValue);
			}
			return new Qlosure<Task<TResult>>(AsyncSelect());
		}

		/// <summary>
		/// SelectMany operation for tasks in LINQ expressions to support task chaining.
		/// </summary>
		/// <typeparam name="TSource">The type of the source task result.</typeparam>
		/// <typeparam name="TCollection">The type of the collection task result.</typeparam>
		/// <typeparam name="TResult">The type of the final result.</typeparam>
		/// <param name="task">The source task.</param>
		/// <param name="collectionSelector">The collection selector function.</param>
		/// <param name="resultSelector">The result selector function.</param>
		/// <returns>A qlosure containing the result task.</returns>
		public static Qlosure<Task<TResult>> SelectMany<TSource, TCollection, TResult>(
			this Task<TSource> task,
			Func<TSource, Task<TCollection>> collectionSelector,
			Func<TSource, TCollection, TResult> resultSelector) {
			async Task<TResult> AsyncSelectMany() {
				TSource sourceValue = await task.ConfigureAwait(false);
				Task<TCollection> collectionTask = collectionSelector.Invoke(sourceValue);
				TCollection collectionValue = await collectionTask.ConfigureAwait(false);
				return resultSelector.Invoke(sourceValue, collectionValue);
			}
			return new Qlosure<Task<TResult>>(AsyncSelectMany());
		}

		/// <summary>
		/// SelectMany operation for Qlosure&lt;Task&gt; in LINQ expressions to support task chaining.
		/// </summary>
		/// <typeparam name="TSource">The type of the source task result.</typeparam>
		/// <typeparam name="TCollection">The type of the collection task result.</typeparam>
		/// <typeparam name="TResult">The type of the final result.</typeparam>
		/// <param name="qlosure">The source qlosure containing a task.</param>
		/// <param name="collectionSelector">The collection selector function.</param>
		/// <param name="resultSelector">The result selector function.</param>
		/// <returns>A qlosure containing the result task.</returns>
		public static Qlosure<Task<TResult>> SelectMany<TSource, TCollection, TResult>(
			this Qlosure<Task<TSource>> qlosure,
			Func<TSource, Task<TCollection>> collectionSelector,
			Func<TSource, TCollection, TResult> resultSelector) {
			async Task<TResult> AsyncSelectMany() {
				TSource sourceValue = await qlosure.Value.ConfigureAwait(false);
				Task<TCollection> collectionTask = collectionSelector.Invoke(sourceValue);
				TCollection collectionValue = await collectionTask.ConfigureAwait(false);
				return resultSelector.Invoke(sourceValue, collectionValue);
			}
			return new Qlosure<Task<TResult>>(AsyncSelectMany(), qlosure);
		}
	}
}
