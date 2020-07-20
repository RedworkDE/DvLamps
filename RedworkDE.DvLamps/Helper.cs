using System;
using System.Collections.Generic;

namespace RedworkDE.DvLamps
{
	public static class Helper
	{
		public static T FindMax<T>(this IEnumerable<T> source, Func<T, float> selector)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			if (selector == null)
				throw new ArgumentNullException(nameof(selector));
			float f;
			T val;
			using var enumerator = source.GetEnumerator();
			if (!enumerator.MoveNext())
				throw new InvalidOperationException("Sequence contains no elements");
			for (val = enumerator.Current, f = selector(val); float.IsNaN(f); val = enumerator.Current, f = selector(val))
			{
				if (!enumerator.MoveNext())
					return val;
			}
			while (enumerator.MoveNext())
			{
				var num = selector(enumerator.Current);
				if (num > f)
				{
					f = num;
					val = enumerator.Current;
				}
			}

			return val;
			
		}
	}
}