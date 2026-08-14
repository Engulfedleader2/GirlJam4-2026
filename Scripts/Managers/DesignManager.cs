using Godot;
using System.Collections.Generic;

// Tracks which clothing items have had their design unlocked.

public class DesignManager : Node
{
	private const string ConfigPath = "res://Resources/Config/GameConfig.cfg";
	private const string PriceConfigSection = "Prices";

	public static DesignManager Instance { get; private set; }

	private readonly HashSet<string> unlockedIds = new HashSet<string>();
	private readonly ConfigFile config = new ConfigFile();

	public override void _Ready()
	{
		Instance = this;
		config.Load(ConfigPath);
	}

	public bool IsUnlocked(ClothingData item)
	{
		return unlockedIds.Contains(item.Id);
	}

	// Prices live in GameConfig.cfg so they can be tuned without touching each .tres file.
	public int GetPrice(ClothingData item)
	{
		if (config.HasSectionKey(PriceConfigSection, item.Id))
		{
			return (int)config.GetValue(PriceConfigSection, item.Id);
		}

		return item.Price;
	}

	// Spends treasure and unlocks the item's design. Returns false if you can't afford it.
	public bool BuyDesign(ClothingData item)
	{
		if (IsUnlocked(item))
		{
			return true;
		}

		if (!GameManager.Instance.SpendTreasure(GetPrice(item)))
		{
			return false;
		}

		unlockedIds.Add(item.Id);
		return true;
	}
}
