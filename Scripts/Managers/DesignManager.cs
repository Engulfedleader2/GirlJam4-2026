using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class DesignManager : Node
{
	private const string ConfigPath = "res://Resources/Config/GameConfig.cfg";
	private const string PriceConfigSection = "Prices";
	private const string ClothingDataRoot = "res://Resources/Clothing";

	public static DesignManager Instance { get; private set; }

	private readonly HashSet<string> unlockedIds = new HashSet<string>();
	private readonly ConfigFile config = new ConfigFile();
	private readonly Random random = new Random();

	private List<ClothingData> allClothing = new List<ClothingData>();
	private int stockSize;
	private int refreshCost;

	public List<ClothingData> Stock { get; private set; } = new List<ClothingData>();

	public override void _Ready()
	{
		Instance = this;
		config.Load(ConfigPath);
		stockSize = (int)config.GetValue("Shop", "design_stock_size", 3);
		refreshCost = (int)config.GetValue("Shop", "design_refresh_cost", 50);

		LoadAllClothing();
		RefreshStock();
	}
	
	public void Reset()
	{
		unlockedIds.Clear();
		RefreshStock();
	}
	private void LoadAllClothing()
	{
		allClothing.Clear();

		foreach (ClothingSlot slot in Enum.GetValues(typeof(ClothingSlot)))
		{
			string folder = $"{ClothingDataRoot}/{slot}";
			var dir = new Directory();

			if (dir.Open(folder) != Error.Ok)
			{
				continue;
			}

			dir.ListDirBegin(true, true);
			string fileName = dir.GetNext();

			while (!string.IsNullOrEmpty(fileName))
			{
				if (fileName.EndsWith(".tres"))
				{
					allClothing.Add(GD.Load<ClothingData>($"{folder}/{fileName}"));
				}

				fileName = dir.GetNext();
			}
		}
	}

	// Picks a fresh random batch of locked items for the shop's stock.
	public void RefreshStock()
	{
		Stock = allClothing
			.Where(item => !IsUnlocked(item))
			.OrderBy(_ => random.Next())
			.Take(stockSize)
			.ToList();
	}

	// Spends treasure to reroll the stock. Returns false if you can't afford it.
	public bool RefreshStockPaid()
	{
		if (!GameManager.Instance.SpendTreasure(refreshCost))
		{
			return false;
		}

		RefreshStock();
		return true;
	}

	public int RefreshCost => refreshCost;

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

		int stockIndex = Stock.IndexOf(item);
		if (stockIndex >= 0)
		{
			ClothingData replacement = allClothing
				.Where(candidate => !IsUnlocked(candidate) && !Stock.Contains(candidate))
				.OrderBy(_ => random.Next())
				.FirstOrDefault();

			if (replacement != null)
			{
				Stock[stockIndex] = replacement;
			}
			else
			{
				Stock.RemoveAt(stockIndex);
			}
		}

		return true;
	}
}
