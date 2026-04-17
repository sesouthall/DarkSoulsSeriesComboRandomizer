namespace DarkSoulsItemMigrator
{
    static class ArrayExtensions
	{
		public static bool ArrayEquals<T>(this T[] sourceArray, T[] targetArray) where T : IEquatable<T>
		{
			if (sourceArray == null && targetArray == null) return true;
			if (sourceArray == null || targetArray == null) return false;
			if (sourceArray.Length != targetArray.Length) return false;
			for (int i = 0; i < sourceArray.Length; i++)
			{
				if (!sourceArray[i].Equals(targetArray[i])) return false;
			}
			return true;
		}
	}
}
