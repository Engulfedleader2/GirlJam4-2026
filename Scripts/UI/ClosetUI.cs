using Godot;

// Handles the closet UI, clothing options, and outfit stats.

public class ClosetUI : Node
{
	private const string ClothingDataRoot = "res://Resources/Clothing";

	private static readonly PackedScene IconScene =
		GD.Load<PackedScene>("res://Scenes/UI/ClothingIcon.tscn");

	private ClothingLayerController paperDoll;
	private Label statsLabel;

	public override void _Ready()
	{
		// Get the paper doll and stats UI.
		paperDoll = GetNode<ClothingLayerController>(
			"HBox/CharacterPreview/PaperDoll"
		);

		statsLabel = GetNode<Label>(
			"HBox/StatsPanel/StatsLabel"
		);

		// Set this doll as the one currently being dressed.
		OutfitManager.Instance.SetActiveDoll(paperDoll);

		// Update the stats whenever the outfit changes.
		OutfitManager.Instance.Connect(
			nameof(OutfitManager.OutfitChanged),
			this,
			nameof(UpdateStatsLabel)
		);

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
				// Create an icon for each clothing item found.
				var icon = (ClothingIcon)IconScene.Instance();

				container.AddChild(icon);

				icon.SetItem(
					GD.Load<ClothingData>(
						$"{folder}/{fileName}"
					)
				);

				icon.Connect(
					"pressed",
					this,
					nameof(OnItemPressed),
					new Godot.Collections.Array { icon }
				);
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
