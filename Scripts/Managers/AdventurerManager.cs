using Godot;
using System;
using System.Collections.Generic;

public class AdventurerManager : Node
{
	public static AdventurerManager Instance { get; private set; }

	private const int CatalogSize = 3;
	private const int PartySize = 3;

	// Base adventurer stats
	private const int HireCost = 10;
	private const int MinBaseHP = 8;
	private const int MaxBaseHP = 14;
	private const int MinBaseAttack = 2;
	private const int MaxBaseAttack = 5;

	private readonly Random random = new Random();

	// Adventurers currently available to hire
	public List<Adventurer> Catalog { get; private set; } = new List<Adventurer>();

	// Adventurers we already hired
	public List<Adventurer> Roster { get; private set; } = new List<Adventurer>();

	// Current party going into the dungeon
	public List<Adventurer> ActiveParty { get; private set; } = new List<Adventurer>();

	public override void _Ready()
	{
		Instance = this;
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
		int maxHp = random.Next(MinBaseHP, MaxBaseHP + 1);

		// Name will probably be picked when hiring
		// Need to add the skin tone and face stuff later
		return new Adventurer
		{
			Dream = Dreams.All[random.Next(Dreams.All.Length)],
			Class = ClothingClass.None,
			MaxHP = maxHp,
			CurrentHP = maxHp,
			Attack = random.Next(MinBaseAttack, MaxBaseAttack + 1)
		};
	}

	public bool HireAdventurer(Adventurer candidate)
	{
		if (!Catalog.Contains(candidate))
		{
			return false;
		}

		if (!GameManager.Instance.SpendTreasure(HireCost))
		{
			return false;
		}

		Catalog.Remove(candidate);
		Roster.Add(candidate);

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
	}
}
