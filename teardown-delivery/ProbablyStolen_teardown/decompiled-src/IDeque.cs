using System.Collections;
using System.Collections.Generic;

public interface IDeque<T> : ICollection<T>, IEnumerable<T>, IEnumerable, ICollection
{
	new int Count { get; }

	bool IsEmpty { get; }

	T this[int index] { get; set; }

	void PushRight(T item);

	void PushLeft(T item);

	T PopRight();

	T PopLeft();

	T PeekRight();

	T PeekLeft();
}
