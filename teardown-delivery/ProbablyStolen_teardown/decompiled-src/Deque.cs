using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class Deque<T> : IDeque<T>, ICollection<T>, IEnumerable<T>, IEnumerable, ICollection
{
	public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
	{
		private readonly Deque<T> _deque;

		private readonly int _version;

		private T _current;

		private int _virtualIndex;

		public T Current => _current;

		object IEnumerator.Current => Current;

		internal Enumerator(Deque<T> deque)
		{
			_deque = deque;
			_version = _deque._version;
			_current = default(T);
			_virtualIndex = -1;
		}

		public bool MoveNext()
		{
			Validate();
			if (_virtualIndex == _deque.Count - 1)
			{
				return false;
			}
			_virtualIndex++;
			_current = _deque[_virtualIndex];
			return true;
		}

		void IEnumerator.Reset()
		{
			Validate();
			_virtualIndex = -1;
			_current = default(T);
		}

		public void Dispose()
		{
		}

		private void Validate()
		{
			if (_version != _deque._version)
			{
				throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
			}
		}
	}

	private const int DefaultCapacity = 4;

	private static readonly T[] EmptyBuffer = new T[0];

	private int _version;

	private object _syncRoot;

	private T[] _buffer;

	private int _leftIndex;

	public int Count { get; private set; }

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot
	{
		get
		{
			if (_syncRoot == null)
			{
				Interlocked.CompareExchange(ref _syncRoot, new object(), null);
			}
			return _syncRoot;
		}
	}

	public bool IsEmpty => Count == 0;

	bool ICollection<T>.IsReadOnly => false;

	public int Capacity
	{
		get
		{
			return _buffer.Length;
		}
		set
		{
			if (value < Count)
			{
				throw new ArgumentOutOfRangeException("value", "capacity was less than the current size.");
			}
			if (value != Capacity)
			{
				T[] array = new T[value];
				CopyTo(array, 0);
				LeftIndex = 0;
				_buffer = array;
			}
		}
	}

	private bool LoopsAround => Count > Capacity - LeftIndex;

	private int LeftIndex
	{
		get
		{
			return _leftIndex;
		}
		set
		{
			_leftIndex = CalcIndex(value);
		}
	}

	private T Left
	{
		get
		{
			return this[0];
		}
		set
		{
			this[0] = value;
		}
	}

	private T Right
	{
		get
		{
			return this[Count - 1];
		}
		set
		{
			this[Count - 1] = value;
		}
	}

	public T this[int index]
	{
		get
		{
			return _buffer[VirtualIndexToBufferIndex(index)];
		}
		set
		{
			_buffer[VirtualIndexToBufferIndex(index)] = value;
		}
	}

	public Deque()
	{
		_buffer = EmptyBuffer;
	}

	public Deque(int capacity)
	{
		if (capacity < 0)
		{
			throw new ArgumentOutOfRangeException("capacity", "capacity was less than zero.");
		}
		_buffer = ((capacity == 0) ? EmptyBuffer : new T[capacity]);
	}

	public Deque(IEnumerable<T> collection)
	{
		if (collection == null)
		{
			throw new ArgumentNullException("collection");
		}
		InitializeFromCollection(collection);
	}

	private void InitializeFromCollection(IEnumerable<T> enumerable)
	{
		ICollection<T> collection = (enumerable as ICollection<T>) ?? enumerable.ToArray();
		int count = collection.Count;
		if (count == 0)
		{
			_buffer = EmptyBuffer;
			return;
		}
		_buffer = new T[count];
		collection.CopyTo(_buffer, 0);
		Count = count;
	}

	public void PushRight(T item)
	{
		EnsureCapacity(Count + 1);
		Count++;
		Right = item;
		_version++;
	}

	public void PushLeft(T item)
	{
		EnsureCapacity(Count + 1);
		LeftIndex--;
		Count++;
		Left = item;
		_version++;
	}

	public T PopRight()
	{
		if (IsEmpty)
		{
			throw new InvalidOperationException("The deque is empty");
		}
		T right = Right;
		Right = default(T);
		Count--;
		_version++;
		return right;
	}

	public T PopLeft()
	{
		if (IsEmpty)
		{
			throw new InvalidOperationException("The deque is empty");
		}
		T left = Left;
		Left = default(T);
		LeftIndex++;
		Count--;
		_version++;
		return left;
	}

	public T PeekRight()
	{
		if (IsEmpty)
		{
			throw new InvalidOperationException("The deque is empty");
		}
		return Right;
	}

	public T PeekLeft()
	{
		if (IsEmpty)
		{
			throw new InvalidOperationException("The deque is empty");
		}
		return Left;
	}

	public void Clear()
	{
		if (LoopsAround)
		{
			Array.Clear(_buffer, LeftIndex, Capacity - LeftIndex);
			Array.Clear(_buffer, 0, LeftIndex + (Count - Capacity));
		}
		else
		{
			Array.Clear(_buffer, LeftIndex, Count);
		}
		Count = 0;
		LeftIndex = 0;
		_version++;
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(this);
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	void ICollection<T>.Add(T item)
	{
		PushRight(item);
	}

	bool ICollection<T>.Remove(T item)
	{
		EqualityComparer<T> @default = EqualityComparer<T>.Default;
		int num = -1;
		int num2 = 0;
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				T current = enumerator.Current;
				if (@default.Equals(item, current))
				{
					num = num2;
					break;
				}
				num2++;
			}
		}
		switch (num)
		{
		case -1:
			return false;
		case 0:
			PopLeft();
			break;
		default:
			if (num == Count - 1)
			{
				PopRight();
				break;
			}
			if (num < Count / 2)
			{
				for (int num3 = num - 1; num3 >= 0; num3--)
				{
					this[num3 + 1] = this[num3];
				}
				Left = default(T);
				LeftIndex++;
				Count--;
			}
			else
			{
				for (int i = num + 1; i < Count; i++)
				{
					this[i - 1] = this[i];
				}
				Right = default(T);
				Count--;
			}
			_version++;
			break;
		}
		return true;
	}

	public bool Contains(T item)
	{
		return this.Contains(item, EqualityComparer<T>.Default);
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		((ICollection)this).CopyTo((Array)array, arrayIndex);
	}

	void ICollection.CopyTo(Array array, int arrayIndex)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (array.Rank != 1)
		{
			throw new ArgumentException("Only single dimensional arrays are supported for the requested action.");
		}
		if (arrayIndex < 0)
		{
			throw new ArgumentOutOfRangeException("arrayIndex", "Index was less than the array's lower bound.");
		}
		if (arrayIndex >= array.Length)
		{
			throw new ArgumentOutOfRangeException("arrayIndex", "Index was greater than the array's upper bound.");
		}
		if (Count == 0)
		{
			return;
		}
		if (array.Length - arrayIndex < Count)
		{
			throw new ArgumentException("Destination array was not long enough");
		}
		try
		{
			if (!LoopsAround)
			{
				Array.Copy(_buffer, LeftIndex, array, arrayIndex, Count);
				return;
			}
			Array.Copy(_buffer, LeftIndex, array, arrayIndex, Capacity - LeftIndex);
			Array.Copy(_buffer, 0, array, arrayIndex + Capacity - LeftIndex, LeftIndex + (Count - Capacity));
		}
		catch (ArrayTypeMismatchException)
		{
			throw new ArgumentException("Target array type is not compatible with the type of items in the collection.");
		}
	}

	public void TrimExcess()
	{
		Capacity = Count;
	}

	private void EnsureCapacity(int min)
	{
		if (Capacity < min)
		{
			int val = ((Capacity == 0) ? 4 : (Capacity * 2));
			val = Math.Max(val, min);
			Capacity = val;
		}
	}

	private int CalcIndex(int position)
	{
		if (Capacity != 0)
		{
			return position.Mod(Capacity);
		}
		return 0;
	}

	private int VirtualIndexToBufferIndex(int index)
	{
		if (index < 0 || index >= Count)
		{
			throw new ArgumentOutOfRangeException("index", "Index was out of range. Must be non-negative and less than the size of the collection.");
		}
		return CalcIndex(LeftIndex + index);
	}
}
