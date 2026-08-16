using Godot;

public class MainGameUI : Control
{
	private DungeonData testDungeon;
	private AudioStream shopMusic;
	private AudioStream dungeonMusic;
	private AudioStream tvOnSFX;

	private SceneManager sceneManager;
	private DungeonView _dungeonView;

	private Label headerLabel;
	private Label resultLabel;
	private Label treasureLabel;
	private Control floorChoice;
	private TextureRect calendarDay;
	private bool _runActive;

	public override void _Ready()
	{
		testDungeon = GD.Load<DungeonData>("res://Resources/Dungeons/BaseDungeon.tres");
		shopMusic = GD.Load<AudioStream>("res://Assets/Audio/Music/jauntyShop.mp3");
		dungeonMusic = GD.Load<AudioStream>("res://Assets/Audio/Music/dungeonSynth.mp3");
		tvOnSFX = GD.Load<AudioStream>("res://Assets/Audio/SFX/MENU/SFX_MENU_TV Static AND Turn On.wav");

		sceneManager = GetNode<SceneManager>("/root/SceneManager");

		//headerLabel = GetNode<Label>("VBox/HeaderLabel");
		resultLabel = GetNode<Label>("VBox/ResultLabel");
		treasureLabel = GetNode<Label>("VBoxContainer/Gold/MarginContainer/TreasureLabel");
		SetupDungeonViewport();
		GetNode<Button>("VBox/ButtonRow/DressPartyButton").Connect(
			"pressed", this, nameof(OnDressPartyPressed)
		);

		GetNode<Button>("VBox/ButtonRow/VentureForthButton").Connect(
			"pressed", this, nameof(OnVentureForthPressed)
		);

		GetNode<Button>("VBox/ButtonRow/DesignsButton").Connect(
			"pressed", this, nameof(OnDesignsPressed)
		);

		GetNode<TextureButton>("CurtainButton").Connect(
			"pressed", this, nameof(OnDressPartyPressed)
		);

		GetNode<TextureButton>("ClipboardButton").Connect(
			"pressed", this, nameof(OnHeroSelectionPressed)
		);

		GetNode<TextureButton>("TagButton").Connect(
			"pressed", this, nameof(OnHireShopPressed)
		);

		GetNode<TextureButton>("GreenBookButton").Connect(
			"pressed", this, nameof(OnDesignsPressed)
		);

		WireOutlineHover(GetNode<TextureButton>("TagButton"));
		WireOutlineHover(GetNode<TextureButton>("CurtainButton"));
		WireOutlineHover(GetNode<TextureButton>("ClipboardButton"));
		WireOutlineHover(GetNode<TextureButton>("GreenBookButton"));

		if (GameManager.Instance.PendingVentureForth)
		{
			GameManager.Instance.PendingVentureForth = false;
			OnVentureForthPressed();
		}
		else
		{
			AudioManager.Instance.PlayMusic(shopMusic);
		}

		Refresh();
	}
	
	private void SetupDungeonViewport()
	{
		var viewport = GetNode<Viewport>("Control/Viewport");
		_dungeonView = GetNode<DungeonView>("Control/Viewport/DungeonView");
		
		viewport.RenderTargetVFlip = true;
		viewport.RenderTargetUpdateMode = Viewport.UpdateMode.Always;
		
		var texture = viewport.GetTexture();
		texture.Flags = 0;
		GetNode<TextureRect>("Control/TVScreen").Texture = texture;
		
		floorChoice = GetNode<Control>("Control/FloorChoice");
		floorChoice.Visible = false;
		GetNode<Button>("Control/FloorChoice/VBoxContainer/ReturnButton").Connect("pressed", this, nameof(OnReturnPressed));
		GetNode<Button>("Control/FloorChoice/VBoxContainer/DelveButton").Connect("pressed", this, nameof(OnDelvePressed));
		_dungeonView.Connect(nameof(DungeonView.PlaybackFinished), this, nameof(OnFloorFinished));
	}
	
	private void SetMenusEnabled(bool enabled)
	{
		//add buttons that shouldnt be clickable during dungeon run
		GetNode<TextureButton>("CurtainButton").Disabled = !enabled;
		GetNode<TextureButton>("ClipboardButton").Disabled = !enabled;
		GetNode<TextureButton>("TagButton").Disabled = !enabled;
		GetNode<TextureButton>("GreenBookButton").Disabled = !enabled;
		
		
	}
	private void Refresh()
	{
		//headerLabel.Text = $"Day {GameManager.Instance.CurrentDay} - Treasure: {GameManager.Instance.Treasure}";
		treasureLabel.Text = $"{GameManager.Instance.Treasure}";
		var currentDay = GameManager.Instance.CurrentDay;
		var previousDay = currentDay - 1;
		if(previousDay > 0)
		{
			GetNode<TextureRect>($"VBoxContainer/HBoxContainer/Day{previousDay}").Visible = false;
		}
		GetNode<TextureRect>($"VBoxContainer/HBoxContainer/Day{currentDay}").Visible = true;
		
	}

	private void OnDressPartyPressed()
	{
		sceneManager.GoToCloset();
	}

	private void OnDesignsPressed()
	{
		sceneManager.GoToDesignCatologue();
	}

	private void OnHeroSelectionPressed()
	{
		sceneManager.GoToHeroSelection();
	}

	private void OnHireShopPressed()
	{
		sceneManager.GoToHireShop();
	}

	private void OnVentureForthPressed()
	{
		var activeParty = AdventurerManager.Instance.ActiveParty;

		if (activeParty.Count == 0)
		{
			resultLabel.Text = "Send at least one adventurer into the dungeon first.";
			return;
		}

		AudioManager.Instance.PlayMusic(dungeonMusic);
		AudioManager.Instance.PlaySFX(tvOnSFX);
	/*
		DungeonManager.Instance.StartRun(testDungeon, activeParty);

		while (DungeonManager.Instance.IsRunActive)
		{
			DungeonManager.Instance.AdvanceFloor();
		}
	*/
		_runActive = true;
		SetMenusEnabled(false);
	
		DungeonManager.Instance.BeginRun();
		_dungeonView.StartPlayback(activeParty, DungeonManager.Instance.BuildCurrentFloor(activeParty));
		//sceneManager.GoToReceipt();
	}
	
	private void OnFloorFinished() 
	{
		var dm = DungeonManager.Instance;
		if (dm.PartyWiped || !dm.HasNextFloor) 
			FinishRun();
		else
			floorChoice.Visible = true;
	}
	
	private void OnDelvePressed()
	{
		floorChoice.Visible = false;
		DungeonManager.Instance.AdvanceToNextFloor();
		var activeParty = AdventurerManager.Instance.ActiveParty;
		_dungeonView.PlayTape(DungeonManager.Instance.BuildCurrentFloor(activeParty));
	}
	
	private void OnReturnPressed()
	{
		floorChoice.Visible = false;
		FinishRun();
	}
	
	private void FinishRun() {
		_runActive = false;
		SetMenusEnabled(true);
		AudioManager.Instance.PlayMusic(shopMusic);
		DungeonManager.Instance.EndRun();
		sceneManager.GoToReceipt();
	}
	
	private void WireOutlineHover(BaseButton button) 
	{
		if (button.Material is ShaderMaterial whiteOutline)	
		{
			var temp = (ShaderMaterial)whiteOutline.Duplicate();
			temp.SetShaderParam("line_thickness", 0f);
			button.Material =  temp;
		}
		
		button.Connect("mouse_entered", this, nameof(OnOutlineHover),
			new Godot.Collections.Array {button, true});
		button.Connect("mouse_exited", this, nameof(OnOutlineHover),
			new Godot.Collections.Array {button, false});
	}
	
	private void OnOutlineHover(Godot.Object obj, bool hovering)
	{
		var item = (CanvasItem)obj;
		if (item.Material is ShaderMaterial mat)
			mat.SetShaderParam("line_thickness", hovering ? 4f : 0f);
	}
}
