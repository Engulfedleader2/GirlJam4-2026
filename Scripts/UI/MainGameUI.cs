using Godot;
using System.Collections.Generic;

// Runs the Tailor Shop loop: hire from the catalog, pick your active party,
// go dress them in the Closet, then send them out and see what happened.

public class MainGameUI : Control
{
	private const int HireCost = 10;

	private DungeonData testDungeon;

	private SceneManager sceneManager;

	private Label headerLabel;
	private VBoxContainer catalogList;
	private VBoxContainer rosterList;
	private Label resultLabel;

	public override void _Ready()
	{
		testDungeon = GD.Load<DungeonData>("res://Resources/Dungeons/BaseDungeon.tres");

		sceneManager = GetNode<SceneManager>("/root/SceneManager");

		headerLabel = GetNode<Label>("VBox/HeaderLabel");
		catalogList = GetNode<VBoxContainer>("VBox/HBox/CatalogPanel/CatalogList");
		rosterList = GetNode<VBoxContainer>("VBox/HBox/RosterPanel/RosterList");
		resultLabel = GetNode<Label>("VBox/ResultLabel");

		GetNode<Button>("VBox/ButtonRow/DressPartyButton").Connect(
			"pressed", this, nameof(OnDressPartyPressed)
		);

		GetNode<Button>("VBox/ButtonRow/VentureForthButton").Connect(
			"pressed", this, nameof(OnVentureForthPressed)
		);

		GetNode<Button>("VBox/ButtonRow/DesignsButton").Connect(
			"pressed", this, nameof(OnDesignsPressed)
		);

		GetNode<Button>("CurtainButton").Connect(
			"pressed", this, nameof(OnDressPartyPressed)
		);
		
		if (GameManager.Instance.PendingVentureForth) 
		{
			GameManager.Instance.PendingVentureForth = false;
			OnVentureForthPressed();
		}
		Refresh();
	}

	private void Refresh()
	{
		headerLabel.Text = $"Day {GameManager.Instance.CurrentDay} - Treasure: {GameManager.Instance.Treasure}";

		RefreshCatalog();
		RefreshRoster();
	}

	private void RefreshCatalog()
	{
		foreach (Node child in catalogList.GetChildren())
		{
			child.QueueFree();
		}

		List<Adventurer> catalog = AdventurerManager.Instance.Catalog;

		for (int i = 0; i < catalog.Count; i++)
		{
			var button = new Button { Text = $"Hire for {HireCost}g" };
			button.Connect("pressed", this, nameof(OnHirePressed), new Godot.Collections.Array { i });
			catalogList.AddChild(button);
		}
	}

	private void RefreshRoster()
	{
		foreach (Node child in rosterList.GetChildren())
		{
			child.QueueFree();
		}

		List<Adventurer> roster = AdventurerManager.Instance.Roster;

		for (int i = 0; i < roster.Count; i++)
		{
			Adventurer adventurer = roster[i];
			bool isActive = AdventurerManager.Instance.ActiveParty.Contains(adventurer);
			string name = string.IsNullOrEmpty(adventurer.Name) ? "Adventurer" : adventurer.Name;

			var button = new Button
			{
				Text = $"{name} (HP {adventurer.CurrentHP}/{adventurer.MaxHP}) - {(isActive ? "Active" : "Inactive")}"
			};

			button.Connect("pressed", this, nameof(OnRosterMemberPressed), new Godot.Collections.Array { i });
			rosterList.AddChild(button);
		}
	}

	private void OnHirePressed(int catalogIndex)
	{
		List<Adventurer> catalog = AdventurerManager.Instance.Catalog;

		if (catalogIndex < catalog.Count)
		{
			AdventurerManager.Instance.HireAdventurer(catalog[catalogIndex]);
		}

		Refresh();
	}

	private void OnRosterMemberPressed(int rosterIndex)
	{
		List<Adventurer> roster = AdventurerManager.Instance.Roster;

		if (rosterIndex < roster.Count)
		{
			Adventurer adventurer = roster[rosterIndex];
			bool isActive = AdventurerManager.Instance.ActiveParty.Contains(adventurer);
			AdventurerManager.Instance.SetActive(adventurer, !isActive);
		}

		Refresh();
	}

	private void OnDressPartyPressed()
	{
		sceneManager.GoToCloset();
	}

	private void OnDesignsPressed()
	{
		sceneManager.GoToDesignShop();
	}

	private void OnVentureForthPressed()
	{
		List<Adventurer> activeParty = AdventurerManager.Instance.ActiveParty;

		if (activeParty.Count == 0)
		{
			resultLabel.Text = "Send at least one adventurer into the dungeon first.";
			return;
		}

		DungeonManager.Instance.StartRun(testDungeon, activeParty);

		while (DungeonManager.Instance.IsRunActive)
		{
			DungeonManager.Instance.AdvanceFloor();
		}

		ShowReceipt();
		GameManager.Instance.AdvanceDay();
		Refresh();
	}

	private void ShowReceipt()
	{
		var lines = new List<string>
		{
			$"{testDungeon.DungeonName} - reached floor {DungeonManager.Instance.CurrentFloor}"
		};

		foreach (Adventurer adventurer in DungeonManager.Instance.Party)
		{
			string name = string.IsNullOrEmpty(adventurer.Name) ? "Adventurer" : adventurer.Name;
			lines.Add(adventurer.IsAlive ? $"{name} returned." : $"{name} did not return.");
		}

		resultLabel.Text = string.Join("\n", lines);
	}
}
