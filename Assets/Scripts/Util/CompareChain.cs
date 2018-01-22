using UnityEngine;
using System.Collections;

public class CompareChain<T> : System.Collections.Generic.IComparer<T>
{
	private CompareChain<T> next;

	public CompareChain(bool reverseAll = false) { this.reverseAll = reverseAll; }

	private delegate int Comparator(T lhs, T rhs);
	private Comparator comparator;
	private bool reverseAll;


	public delegate U FieldGetter<U>(T t) where U : System.IComparable;


	private void SetComparator<U>(FieldGetter<U> field, bool reverse) where U : System.IComparable
	{
		if (!reverse)
		{
			comparator = (T lhs, T rhs) => { return field(lhs).CompareTo(field(rhs)); };
		}
		else
		{
			comparator = (T lhs, T rhs) => { return field(rhs).CompareTo(field(lhs)); };
		}
	}


	public CompareChain<T> Add<U>(FieldGetter<U> field, bool reverse = false) where U : System.IComparable
	{
		CompareChain<T> tail = this;
		while (tail.next != null)
		{
			tail = tail.next;
		}

		tail.next = new CompareChain<T>();
		tail.next.SetComparator(field, reverse);

		return this;
	}


	public int Compare(T lhs, T rhs)
	{
		int result;
		if (comparator == null)
		{
			result = next == null ? 0 : next.Compare(lhs, rhs);
		}
		else
		{
			result = comparator(lhs, rhs);
			if (result == 0 && next != null)
			{
				result = next.Compare(lhs, rhs);
			}
		}

		return reverseAll ? -result : result;
	}
}