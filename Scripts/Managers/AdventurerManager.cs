using Godot;
using System;
using System.Collections.Generic;

public class AdventurerManager : Node
{
	private const string ConfigPath = "res://Resources/Config/GameConfig.cfg";

	public static AdventurerManager Instance { get; private set; }

	private const int CatalogSize = 3;
	private const int PartySize = 3;

	// Base adventurer stats. Reads from GameConfig.cfg
	private int hireCost;
	private int minBaseHp;
	private int maxBaseHp;
	private int minBaseAttack;
	private int maxBaseAttack;

	public int HireCost => hireCost;

	private readonly Random random = new Random();

	// Adventurers currently available to hire
	public List<Adventurer> Catalog { get; private set; } = new List<Adventurer>();

	// Adventurers we already hired
	public List<Adventurer> Roster { get; private set; } = new List<Adventurer>();

	// Current party going into the dungeon
	public List<Adventurer> ActiveParty { get; private set; } = new List<Adventurer>();

	// Who's currently being dressed in the Closet.
	public Adventurer Selected { get; private set; }

	public override void _Ready()
	{
		Instance = this;

		var config = new ConfigFile();
		config.Load(ConfigPath);
		hireCost = (int)config.GetValue("Adventurers", "hire_cost", 10);
		minBaseHp = (int)config.GetValue("Adventurers", "min_base_hp", 8);
		maxBaseHp = (int)config.GetValue("Adventurers", "max_base_hp", 14);
		minBaseAttack = (int)config.GetValue("Adventurers", "min_base_attack", 2);
		maxBaseAttack = (int)config.GetValue("Adventurers", "max_base_attack", 5);
		RefreshCatalog();
	}

	public void RefreshCatalog()
	{
		Catalog.Clear();

		for (int i = 0; i < CatalogSize; i++)
		{
			Catalog.Add(GenerateAdventurer());
		}
	}

	public Adventurer GenerateAdventurer()
	{
		int baseMaxHp = random.Next(minBaseHp, maxBaseHp + 1);
		int baseAttack = random.Next(minBaseAttack, maxBaseAttack + 1);

		// Need to add the skin tone and face stuff later
		return new Adventurer
		{
			Name = Names.All[random.Next(Names.All.Length)],
			Dream = Dreams.All[random.Next(Dreams.All.Length)],
			Class = ClothingClass.None,
			BaseMaxHP = baseMaxHp,
			BaseAttack = baseAttack,
			MaxHP = baseMaxHp,
			CurrentHP = baseMaxHp,
			Attack = baseAttack,
		};
	}

	// Who's being dressed in the Closet right now.
	public void SelectAdventurer(Adventurer adventurer)
	{
		if (Roster.Contains(adventurer))
		{
			Selected = adventurer;
		}
	}
	//need to handle game over new games
	public void Reset() 
	{
		Catalog.Clear();
		Roster.Clear();
		ActiveParty.Clear();
		Selected = null;
		
		RefreshCatalog();
	}

	public bool HireAdventurer(Adventurer candidate)
	{
		if (!Catalog.Contains(candidate))
		{
			return false;
		}

		if (!GameManager.Instance.SpendTreasure(hireCost))
		{
			return false;
		}

		Catalog.Remove(candidate);
		Roster.Add(candidate);
		
		//trying this out activating adventurer on hire if theres currently less than 3 in roster 
		if(ActiveParty.Count < PartySize)
		{
			ActiveParty.Add(candidate);
		}

		return true;
	}

	// Add/remove an adventurer from the dungeon party
	public void SetActive(Adventurer adventurer, bool active)
	{
		if (!Roster.Contains(adventurer))
		{
			return;
		}

		if (active)
		{
			if (ActiveParty.Count >= PartySize || ActiveParty.Contains(adventurer))
			{
				return;
			}

			ActiveParty.Add(adventurer);
		}
		else
		{
			ActiveParty.Remove(adventurer);
		}
	}

	// Heal them when they make it back
	public void HandleReturningAdventurer(Adventurer adventurer)
	{
		adventurer.CurrentHP = adventurer.MaxHP;
	}

	// Remove them if they died
	public void HandleDeadAdventurer(Adventurer adventurer)
	{
		Roster.Remove(adventurer);
		ActiveParty.Remove(adventurer);

		if (Selected == adventurer)
		{
			Selected = null;
		}
	}
	
}
