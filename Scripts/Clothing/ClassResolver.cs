using System.Collections.Generic;

public static class ClassResolver
{
	private const int ClassThreshold = 3;

	public static ClothingClass Resolve(
		IEnumerable<ClothingData> equippedItems
	)
	{
		var counts = new Dictionary<ClothingClass, int>
		{
			{ ClothingClass.Tank, 0 },
			{ ClothingClass.DPS, 0 },
			{ ClothingClass.Healer, 0 }
		};

		foreach (ClothingData item in equippedItems)
		{
			if (item == null)
			{
				continue;
			}

			if (counts.ContainsKey(item.Class))
			{
				counts[item.Class]++;
			}
		}

		foreach (KeyValuePair<ClothingClass, int> entry in counts)
		{
			if (entry.Value >= ClassThreshold)
			{
				return entry.Key;
			}
		}

		return ClothingClass.None;
	}
}
