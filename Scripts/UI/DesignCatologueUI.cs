using Godot;

// Shows the shop's current rotating stock as clickable icon tiles, plus a
// paid refresh to reroll the selection.

public class DesignCatologueUI : Control
{
	private SceneManager sceneManager;
	private Label headerLabel;
	private GridContainer stockGrid;

	private PackedScene iconScene;
	private Texture refreshIcon;
	private string statusMessage = "";

	private AudioStream[] catalogOpenSFX;
	private AudioStream[] catalogFlipSFX;
	private AudioStream[] catalogPurchaseSFX;
	private AudioStream[] backSFX;

	public override void _Ready()
	{
		sceneManager = GetNode<SceneManager>("/root/SceneManager");
		iconScene = GD.Load<PackedScene>("res://Scenes/UI/ClothingIcon.tscn");
		refreshIcon = GD.Load<Texture>("res://Assets/UI/UI/refresh.png");

		headerLabel = GetNode<Label>("VBox/HeaderLabel");
		stockGrid = GetNode<GridContainer>("VBox/StockGrid");
		stockGrid.Columns = 2;

		GetNode<Button>("BackButton").Connect(
			"pressed", this, nameof(OnBackPressed)
		);

		AudioManager.Instance.PlayLayeredMusic(new[]
		{
			GD.Load<AudioStream>("res://Assets/Audio/Music/ShopMusic/Layer 1 (Shop).mp3"),
			GD.Load<AudioStream>("res://Assets/Audio/Music/ShopMusic/Layer 2 (Shop).mp3"),
			GD.Load<AudioStream>("res://Assets/Audio/Music/ShopMusic/Layer 3 (Shop).mp3")
		});

		catalogOpenSFX = new[]
		{
			GD.Load<AudioStream>("res://Assets/Audio/SFX/MENU/SFX_MENU_Catalogue Open_01.wav"),
			GD.Load<AudioStream>("res://Assets/Audio/SFX/MENU/SFX_MENU_Catalogue Open_02.wav")
		};

		catalogFlipSFX = new[]
		{
			GD.Load<AudioStream>("res://Assets/Audio/SFX/MENU/SFX_MENU_Catalogue Flip_01.wav"),
			GD.Load<AudioStream>("res://Assets/Audio/SFX/MENU/SFX_MENU_Catalogue Flip_02.wav")
		};

		catalogPurchaseSFX = new[]
		{
			GD.Load<AudioStream>("res://Assets/Audio/SFX/MENU/SFX_MENU_Catalogue Purchase_01.wav"),
			GD.Load<AudioStream>("res://Assets/Audio/SFX/MENU/SFX_MENU_Catalogue Purchase_02.wav")
		};

		backSFX = new[]
		{
			GD.Load<AudioStream>("res://Assets/Audio/SFX/UI/SFX_UI_Back_01.wav"),
			GD.Load<AudioStream>("res://Assets/Audio/SFX/UI/SFX_UI_Back_02.wav"),
			GD.Load<AudioStream>("res://Assets/Audio/SFX/UI/SFX_UI_Back_03.wav")
		};

		AudioManager.Instance.PlayRandomSFX(catalogOpenSFX);

		Refresh();
	}

	private void Refresh()
	{
		headerLabel.Text = $"NEW - Treasure: {GameManager.Instance.Treasure}{statusMessage}";

		foreach (Node child in stockGrid.GetChildren())
		{
			child.QueueFree();
		}

		foreach (ClothingData item in DesignManager.Instance.Stock)
		{
			stockGrid.AddChild(BuildItemTile(item));
		}

		stockGrid.AddChild(BuildRefreshTile());
	}

	// Every grid cell (items and the refresh button) uses this exact size so
	// GridContainer can't stretch one row/column to fit an oversized cell.
	private static readonly Vector2 TileSize = new Vector2(104, 132);

	private VBoxContainer BuildItemTile(ClothingData item)
	{
		var tile = new VBoxContainer
		{
			Alignment = BoxContainer.AlignMode.Center,
			RectMinSize = TileSize
		};

		var icon = (ClothingIcon)iconScene.Instance();
		icon.SetItem(item);
		icon.RectMinSize = new Vector2(84, 84);
		icon.SizeFlagsHorizontal = (int)Control.SizeFlags.ShrinkCenter;
		icon.Connect("pressed", this, nameof(OnBuyDesignPressed), new Godot.Collections.Array { icon });
		tile.AddChild(icon);

		var priceLabel = new Label
		{
			Text = $"{item.ItemName}\n{DesignManager.Instance.GetPrice(item)}g",
			Align = Label.AlignEnum.Center,
			Autowrap = true,
			RectMinSize = new Vector2(104, 0)
		};
		priceLabel.AddColorOverride("font_color", new Color(0.1f, 0.1f, 0.1f));
		tile.AddChild(priceLabel);

		return tile;
	}

	private VBoxContainer BuildRefreshTile()
	{
		var tile = new VBoxContainer
		{
			Alignment = BoxContainer.AlignMode.Center,
			RectMinSize = TileSize
		};

		var button = new Button
		{
			RectMinSize = new Vector2(84, 84),
			SizeFlagsHorizontal = (int)Control.SizeFlags.ShrinkCenter,
			Icon = refreshIcon,
			Flat = true,
			ExpandIcon = true,
			IconAlign = Button.TextAlign.Center
		};
		button.Connect("pressed", this, nameof(OnRefreshPressed));
		tile.AddChild(button);

		var label = new Label
		{
			Text = $"Refresh\n{DesignManager.Instance.RefreshCost}g",
			Align = Label.AlignEnum.Center,
			Autowrap = true,
			RectMinSize = new Vector2(104, 0)
		};
		label.AddColorOverride("font_color", new Color(0.1f, 0.1f, 0.1f));
		tile.AddChild(label);

		return tile;
	}

	private void OnBuyDesignPressed(ClothingIcon icon)
	{
		bool bought = DesignManager.Instance.BuyDesign(icon.Item);
		statusMessage = bought ? "" : " (can't afford that)";

		if (bought)
		{
			AudioManager.Instance.PlayRandomSFX(catalogPurchaseSFX);
		}

		Refresh();
	}

	private void OnRefreshPressed()
	{
		bool refreshed = DesignManager.Instance.RefreshStockPaid();
		statusMessage = refreshed ? "" : " (can't afford refresh)";

		if (refreshed)
		{
			AudioManager.Instance.PlayRandomSFX(catalogFlipSFX);
		}

		Refresh();
	}

	private void OnBackPressed()
	{
		AudioManager.Instance.PlayRandomSFX(backSFX);
		sceneManager.GoToMainGame();
	}
}
