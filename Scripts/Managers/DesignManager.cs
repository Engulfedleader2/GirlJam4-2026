using Godot;
using System.Collections.Generic;

// Tracks which clothing items have had their design unlocked.

public class DesignManager : Node
{
	public static DesignManager Instance { get; private set; }

	private readonly HashSet<string> unlockedIds = new HashSet<string>();

	public override void _Ready()
	{
		Instance = this;
	}

	public bool IsUnlocked(ClothingData item)
	{
		return unlockedIds.Contains(item.Id);
	}

	// Spends treasure and unlocks the item's design. Returns false if you can't afford it.
	public bool BuyDesign(ClothingData item)
	{
		if (IsUnlocked(item))
		{
			return true;
		}

		if (!GameManager.Instance.SpendTreasure(item.Price))
		{
			return false;
		}

		unlockedIds.Add(item.Id);
		return true;
	}
}
