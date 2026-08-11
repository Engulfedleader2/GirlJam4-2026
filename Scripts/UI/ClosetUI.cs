using Godot;
using System.Collections.Generic;

// Handles the closet UI, clothing options, and outfit stats.

public class ClosetUI : Node
{
	private const string ClothingDataRoot = "res://Resources/Clothing";

	private static readonly PackedScene IconScene =
		GD.Load<PackedScene>("res://Scenes/UI/ClothingIcon.tscn");

	private ClothingLayerController paperDoll;
	private Label statsLabel;
	private Button[] partyButtons;

	public override void _Ready()
	{
		GetNode<Button>("BackButton").Connect(
			"pressed",
			GetNode<SceneManager>("/root/SceneManager"),
			nameof(SceneManager.GoToMainGame)
		);

		// Get the paper doll and stats UI.
		paperDoll = GetNode<ClothingLayerController>(
			"HBox/CharacterPreview/PaperDoll"
		);

		statsLabel = GetNode<Label>(
			"HBox/StatsPanel/StatsLabel"
		);

		partyButtons = new[]
		{
			GetNode<Button>("HBox/StatsPanel/PartyRow/Member0"),
			GetNode<Button>("HBox/StatsPanel/PartyRow/Member1"),
			GetNode<Button>("HBox/StatsPanel/PartyRow/Member2")
		};

		// Set this doll as the one currently being dressed.
		OutfitManager.Instance.SetActiveDoll(paperDoll);

		// Update the stats whenever the outfit changes.
		OutfitManager.Instance.Connect(
			nameof(OutfitManager.OutfitChanged),
			this,
			nameof(UpdateStatsLabel)
		);

		// Temporary: makes sure there's someone to dress when testing this
		// scene on its own, before the Tailor Shop hiring flow exists.
		if (AdventurerManager.Instance.Roster.Count == 0 && AdventurerManager.Instance.Catalog.Count > 0)
		{
			Adventurer freeHire = AdventurerManager.Instance.Catalog[0];
			AdventurerManager.Instance.Catalog.Remove(freeHire);
			AdventurerManager.Instance.Roster.Add(freeHire);
		}

		RefreshPartyButtons();

		if (AdventurerManager.Instance.Roster.Count > 0)
		{
			SelectAdventurer(AdventurerManager.Instance.Roster[0]);
		}

		// Setup each clothing category.
		SetupRow(
			"HBox/OutfitOptions/Categories/HeadRow",
			ClothingSlot.Head
		);

		SetupRow(
			"HBox/OutfitOptions/Categories/BodyRow",
			ClothingSlot.Body
		);

		SetupRow(
			"HBox/OutfitOptions/Categories/LegsRow",
			ClothingSlot.Legs
		);

		SetupRow(
			"HBox/OutfitOptions/Categories/FeetRow",
			ClothingSlot.Feet
		);

		SetupRow(
			"HBox/OutfitOptions/Categories/HairRow",
			ClothingSlot.Hair
		);

		SetupRow(
			"HBox/OutfitOptions/Categories/AccessoryRow",
			ClothingSlot.Accessory
		);

		UpdateStatsLabel();
	}

	// Shows up to 3 roster members as buttons; click one to dress them.
	private void RefreshPartyButtons()
	{
		List<Adventurer> roster = AdventurerManager.Instance.Roster;

		for (int i = 0; i < partyButtons.Length; i++)
		{
			Button button = partyButtons[i];

			if (button.IsConnected("pressed", this, nameof(OnPartyMemberPressed)))
			{
				button.Disconnect("pressed", this, nameof(OnPartyMemberPressed));
			}

			if (i < roster.Count)
			{
				Adventurer adventurer = roster[i];
				button.Text = string.IsNullOrEmpty(adventurer.Name) ? "Adventurer" : adventurer.Name;
				button.Disabled = false;
				button.Connect("pressed", this, nameof(OnPartyMemberPressed), new Godot.Collections.Array { i });
			}
			else
			{
				button.Text = "-";
				button.Disabled = true;
			}
		}
	}

	private void OnPartyMemberPressed(int rosterIndex)
	{
		List<Adventurer> roster = AdventurerManager.Instance.Roster;

		if (rosterIndex < roster.Count)
		{
			SelectAdventurer(roster[rosterIndex]);
		}
	}

	// Selects who's being dressed and loads their current outfit onto the doll.
	private void SelectAdventurer(Adventurer adventurer)
	{
		AdventurerManager.Instance.SelectAdventurer(adventurer);
		OutfitManager.Instance.LoadAdventurerOutfit(adventurer);
	}

	// Setup the category button and load its clothing.
	private void SetupRow(string rowPath, ClothingSlot slot)
	{
		Button header = GetNode<Button>($"{rowPath}/Header");
		GridContainer items = GetNode<GridContainer>($"{rowPath}/Items");

		header.Connect(
			"pressed",
			this,
			nameof(OnHeaderPressed),
			new Godot.Collections.Array { items }
		);

		PopulateSlot(items, slot);
	}

	// Show or hide the clothing options for a category.
	private void OnHeaderPressed(GridContainer items)
	{
		items.Visible = !items.Visible;
	}

	// Load all clothing resources for the given slot.
	private void PopulateSlot(
		GridContainer container,
		ClothingSlot slot
	)
	{
		string folder = $"{ClothingDataRoot}/{slot}";

		var dir = new Directory();

		if (dir.Open(folder) != Error.Ok)
		{
			return;
		}

		dir.ListDirBegin(true, true);
		string fileName = dir.GetNext();

		while (!string.IsNullOrEmpty(fileName))
		{
			if (fileName.EndsWith(".tres"))
			{
				ClothingData item = GD.Load<ClothingData>($"{folder}/{fileName}");

				// Only unlocked (purchased) items show up in the closet.
				if (DesignManager.Instance.IsUnlocked(item))
				{
					var icon = (ClothingIcon)IconScene.Instance();

					container.AddChild(icon);
					icon.SetItem(item);

					icon.Connect(
						"pressed",
						this,
						nameof(OnItemPressed),
						new Godot.Collections.Array { icon }
					);
				}
			}

			fileName = dir.GetNext();
		}
	}

	// Equip clothing when its icon is clicked.
	private void OnItemPressed(ClothingIcon icon)
	{
		OutfitManager.Instance.EquipItem(icon.Item);
	}

	// Refresh the current outfit bonus and class.
	private void UpdateStatsLabel()
	{
		int totalBonus =
			OutfitManager.Instance.GetTotalStatBonus();

		ClothingClass resolvedClass =
			OutfitManager.Instance.GetResolvedClass();

		statsLabel.Text =
			$"Total Bonus: {totalBonus}\nClass: {resolvedClass}";
	}
}
