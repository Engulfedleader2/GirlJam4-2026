using Godot;
using System.Collections.Generic;

// Handles the closet UI, clothing options, and outfit stats.

public class ClosetUI : Node
{
	private const string ClothingDataRoot = "res://Resources/Clothing";

	private PackedScene iconScene;
	
	private PackedScene spriteDollScene;
	private readonly SpriteDoll[] partyDolls = new SpriteDoll[3];
	private Adventurer _selected;

	private ClothingLayerController paperDoll;
	private Label statsLabel;
	private Button[] partyButtons;

	private SceneManager sceneManager;
	private AudioStream[] closetEquipSFX;
	private AudioStream[] adventurerSelectSFX;

	public override void _Ready()
	{
		iconScene = GD.Load<PackedScene>("res://Scenes/UI/ClothingIcon.tscn");
		sceneManager = GetNode<SceneManager>("/root/SceneManager");

		closetEquipSFX = new[]
		{
			GD.Load<AudioStream>("res://Assets/Audio/SFX/MENU/SFX_MENU_Closet Equip_01.wav"),
			GD.Load<AudioStream>("res://Assets/Audio/SFX/MENU/SFX_MENU_Closet Equip_02.wav"),
			GD.Load<AudioStream>("res://Assets/Audio/SFX/MENU/SFX_MENU_Closet Equip_03.wav"),
			GD.Load<AudioStream>("res://Assets/Audio/SFX/MENU/SFX_MENU_Closet Equip_04.wav"),
			GD.Load<AudioStream>("res://Assets/Audio/SFX/MENU/SFX_MENU_Closet Equip_05.wav"),
			GD.Load<AudioStream>("res://Assets/Audio/SFX/MENU/SFX_MENU_Closet Equip_06.wav")
		};

		adventurerSelectSFX = new[]
		{
			GD.Load<AudioStream>("res://Assets/Audio/SFX/ADV/SFX_ADV_Grunt_Female_01.wav"),
			GD.Load<AudioStream>("res://Assets/Audio/SFX/ADV/SFX_ADV_Grunt_Female_02.wav"),
			GD.Load<AudioStream>("res://Assets/Audio/SFX/ADV/SFX_ADV_Grunt_Male_01.wav"),
			GD.Load<AudioStream>("res://Assets/Audio/SFX/ADV/SFX_ADV_Grunt_Male_02.wav")
		};

		AudioManager.Instance.PlaySFX(GD.Load<AudioStream>("res://Assets/Audio/SFX/MENU/SFX_MENU_Closet Open.wav"));

		GetNode<Button>("BackButton").Connect(
			"pressed",
			this,
			nameof(OnBackPressed)
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
		spriteDollScene = GD.Load<PackedScene>("res://Scenes/Character/SpriteDoll.tscn");
		for (int i = 0; i < partyButtons.Length; i++)
		{
			var doll = (SpriteDoll)spriteDollScene.Instance();
			partyButtons[i].AddChild(doll);
			partyButtons[i].RectClipContent = false;
			partyDolls[i] = doll;
			doll.Position = partyButtons[i].RectMinSize / 2;
		}
		// Set this doll as the one currently being dressed.
		OutfitManager.Instance.SetActiveDoll(paperDoll);

		// Update the stats whenever the outfit changes.
		OutfitManager.Instance.Connect(
			nameof(OutfitManager.OutfitChanged),
			this,
			nameof(OnOutfitChanged)
		);

		// Temporary: makes sure there's someone to dress when testing this
		// scene on its own, before the Tailor Shop hiring flow exists.
		/* Removing since hiring is set up
		if (AdventurerManager.Instance.Roster.Count == 0 && AdventurerManager.Instance.Catalog.Count > 0)
		{
			Adventurer freeHire = AdventurerManager.Instance.Catalog[0];
			AdventurerManager.Instance.Catalog.Remove(freeHire);
			AdventurerManager.Instance.Roster.Add(freeHire);
		}
		*/
		RefreshPartyButtons();

		if (AdventurerManager.Instance.ActiveParty.Count > 0)
		{
			SelectAdventurer(AdventurerManager.Instance.ActiveParty[0]);
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
		AudioManager.Instance.PlayLayeredMusic(new[]
		{
			GD.Load<AudioStream>("res://Assets/Audio/Music/ShopMusic/Layer 1 (Shop).mp3"),
			GD.Load<AudioStream>("res://Assets/Audio/Music/ShopMusic/Layer 2 (Shop).mp3"),
			GD.Load<AudioStream>("res://Assets/Audio/Music/ShopMusic/Layer 3 (Shop).mp3"),
			GD.Load<AudioStream>("res://Assets/Audio/Music/ShopMusic/Layer 4 (Shop).mp3")
		});
		UpdateStatsLabel();
	}

	// Shows up to 3 roster members as buttons; click one to dress them.
	private void RefreshPartyButtons()
	{
		List<Adventurer> roster = AdventurerManager.Instance.ActiveParty;

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
				button.Visible = true;
				//button.Text = string.IsNullOrEmpty(adventurer.Name) ? "Adventurer" : adventurer.Name;
				button.Disabled = false;
				partyDolls[i].Visible = true;
				DressDoll(partyDolls[i], adventurer);
				button.Connect("pressed", this, nameof(OnPartyMemberPressed), new Godot.Collections.Array { i });
			}
			else
			{
				button.Text = "-";
				button.Disabled = true;
				button.Visible = false;
			}
		}
	}

	private void OnPartyMemberPressed(int rosterIndex)
	{
		List<Adventurer> roster = AdventurerManager.Instance.ActiveParty;

		if (rosterIndex < roster.Count)
		{
			AudioManager.Instance.PlayRandomSFX(adventurerSelectSFX);
			SelectAdventurer(roster[rosterIndex]);
		}
	}

	private void OnBackPressed()
	{
		AudioManager.Instance.PlaySFX(GD.Load<AudioStream>("res://Assets/Audio/SFX/MENU/SFX_MENU_Closet Close.wav"));
		sceneManager.GoToMainGame();
	}

	// Selects who's being dressed and loads their current outfit onto the doll.
	private void SelectAdventurer(Adventurer adventurer)
	{
		_selected = adventurer;
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

		int count = PopulateSlot(items, slot);
		bool hasItems = count > 0;
		items.Visible = hasItems;
		header.Disabled = !hasItems;
	}

	// Show or hide the clothing options for a category.
	private void OnHeaderPressed(GridContainer items)
	{
		items.Visible = !items.Visible;
	}

	// Load all clothing resources for the given slot.
	private int PopulateSlot(
		GridContainer container,
		ClothingSlot slot
	)
	{
		string folder = $"{ClothingDataRoot}/{slot}";

		var dir = new Directory();

		if (dir.Open(folder) != Error.Ok)
		{
			return 0;
		}
		int count = 0;
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
					var icon = (ClothingIcon)iconScene.Instance();

					container.AddChild(icon);
					icon.SetItem(item);

					icon.Connect(
						"pressed",
						this,
						nameof(OnItemPressed),
						new Godot.Collections.Array { icon }
					);
					
					count++;
				}
			}

			fileName = dir.GetNext();
		}
		return count;
	}

	// Equip clothing when its icon is clicked.
	private void OnItemPressed(ClothingIcon icon)
	{
		OutfitManager.Instance.EquipItem(icon.Item);
		AudioManager.Instance.PlayRandomSFX(closetEquipSFX);
	}

	// Refresh the current outfit bonus and class.
	private void UpdateStatsLabel()
	{
		/*
		int totalBonus =
			OutfitManager.Instance.GetTotalStatBonus();

		ClothingClass resolvedClass =
			OutfitManager.Instance.GetResolvedClass();

		statsLabel.Text =
			$"Total Bonus: {totalBonus}\nClass: {resolvedClass}";
			*/
			
		Adventurer adv = AdventurerManager.Instance.Selected;
		ClothingClass resolvedClass =
			OutfitManager.Instance.GetResolvedClass();
		if (adv == null) 
		{
			statsLabel.Text = "No adventurer hired or activated! ";
			return;
		}
		statsLabel.Text = $"Name: {adv.Name}\n" + $"HP: {adv.CurrentHP}/{adv.MaxHP}\n" + $"ATK: {adv.Attack}\n" + $"Class: {resolvedClass}";
	}
	
	private void OnVentureHover(bool hovering)
	{
		var mat = (ShaderMaterial)GetNode<TextureButton>("VentureForthButton").Material;
		mat.SetShaderParam("line_thickness", hovering ? 4.0f : 0.0f);
	}
	
	private void _on_VentureForthButton_mouse_entered()
	{
		this.OnVentureHover(true);
	}
	
	private void _on_VentureForthButton_mouse_exited()
	{
		this.OnVentureHover(false);
	}
	
	//currently just copied this over from maingameui, could figure out a better way to just call the same method so its not copy and pasted.
	private void _on_VentureForthButton_pressed()
	{
		GameManager.Instance.PendingVentureForth = true;
		GetNode<SceneManager>("/root/SceneManager").GoToMainGame();
	}
	
	//sprite doll methods
	private void BuildDollStrip()
	{
		var strip = GetNode<Container>("HBox/StatsPanel/PartyRow");
		
		
	}
	
	private void OnOutfitChanged() 
	{
		UpdateStatsLabel();
		/*
		int index = AdventurerManager.Instance.ActiveParty.IndexOf(_selected);
		if(index >= 0 && index < partyDolls.Length) 
			DressDoll(partyDolls[index], _selected);
		*/
		List<Adventurer> party = AdventurerManager.Instance.ActiveParty;
		for(int i = 0; i < partyDolls.Length; i++)
			if(partyDolls[i].Visible && i < party.Count)
				DressDoll(partyDolls[i], party[i]);
	}
	
	private void DressDoll(SpriteDoll doll, Adventurer adv) 
	{
		foreach (ClothingSlot slot in System.Enum.GetValues(typeof(ClothingSlot)))
		{
			if (adv.EquippedItems.TryGetValue(slot, out ClothingData item) && item != null)
				doll.Equip(item);
			else
				doll.UnequipSlot(slot);
		}
	}
	private void _on_UnequipButton_pressed()
	{
		OutfitManager.Instance.UnequipAll();
		AudioManager.Instance.PlayRandomSFX(closetEquipSFX);
	}
}
