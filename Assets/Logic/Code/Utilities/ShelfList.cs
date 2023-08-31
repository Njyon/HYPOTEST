using System;
using System.Collections.Generic;
using System.Linq;

public class ShelfList<T>
{
	private readonly int maxSize;
	private LinkedList<T> internalList;

	public ShelfList(int maxSize)
	{
		if (maxSize <= 0)
		{
			throw new ArgumentException("Max size must be greater than 0.");
		}

		this.maxSize = maxSize;
		internalList = new LinkedList<T>();
	}

	public void Add(T item)
	{
		internalList.AddFirst(item);

		if (internalList.Count >= maxSize)
		{
			internalList.RemoveLast();
		}
	}

	public T this[int index]
	{
		get
		{
			if (index < 0 || index >= internalList.Count)
			{
				throw new IndexOutOfRangeException();
			}
			return internalList.ElementAt(index);
		}
	}

	public int Count => internalList.Count;

	public bool Contains(T item)
	{
		return internalList.Contains(item);
	}

	public int ContainedItemNum(T item)
	{
		int count = 0;
		foreach (T item2 in internalList)
		{
			if (item2.Equals(item)) count++;
		}
		return count;
	}
}