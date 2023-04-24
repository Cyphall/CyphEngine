namespace CyphEngine.Helper;

public static class ListExt
{
	public static bool RemoveLast<T>(this List<T> list, T item)
	{
		int index = list.LastIndexOf(item);
		if (index >= 0)
		{
			list.RemoveAt(index);
			return true;
		}

		return false;
	}
}