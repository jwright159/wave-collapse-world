using System;
using System.Collections.Generic;

public static class ExtensionMethods
{
	public static T Pop<T>(this List<T> list)
	{
		T obj = list[0];
		list.RemoveAt(0);
		return obj;
	}

	public static void AddAllIf<T>(this List<T> list, Predicate<T> pred, params T[] objs)
	{
		foreach (T obj in objs)
			if (pred.Invoke(obj))
				list.Add(obj);
	}

	public static void AddAllAsSet<T>(this List<T> list, params T[] objs) => list.AddAllIf(obj => obj != null && !list.Contains(obj), objs);
}
