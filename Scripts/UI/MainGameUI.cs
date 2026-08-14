using Godot;

public class MainGameUI : Control
{
	private DungeonData testDungeon;
	private AudioStream shopMusic;
	private AudioStream dungeonMusic;

	private SceneManager sceneManager;
	private DungeonView _dungeonView;

	private Label headerLabel;
	private Label resultLabel;
	private Control floorChoice;

	public override void _Ready()
	{
		testDungeon = GD.Load<DungeonData>("res://Resources/Dungeons/BaseDungeon.tres");
		shopMusic = GD.Load<AudioStream>("res://Assets/Audio/Music/jauntyShop.mp3");
		dungeonMusic = GD.Load<AudioStream>("res://Assets/Audio/Music/dungeonSynth.mp3");

		sceneManager = GetNode<SceneManager>("/root/SceneManager");

		headerLabel = GetNode<Label>("VBox/HeaderLabel");
		resultLabel = GetNode<Label>("VBox/ResultLabel");
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

		GetNode<Button>("CurtainButton").Connect(
			"pressed", this, nameof(OnDressPartyPressed)
		);

		GetNode<Button>("ClipboardButton").Connect(
			"pressed", this, nameof(OnHeroSelectionPressed)
		);

		GetNode<Button>("TagButton").Connect(
			"pressed", this, nameof(OnHireShopPressed)
		);

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
		
		floorChoice = GetNode<Control>("FloorChoice");
		floorChoice.Visible = false;
		GetNode<Button>("FloorChoice/VBoxContainer/ReturnButton").Connect("pressed", this, nameof(OnReturnPressed));
		GetNode<Button>("FloorChoice/VBoxContainer/DelveButton").Connect("pressed", this, nameof(OnDelvePressed));
		_dungeonView.Connect(nameof(DungeonView.PlaybackFinished), this, nameof(OnFloorFinished));
	}

	private void Refresh()
	{
		headerLabel.Text = $"Day {GameManager.Instance.CurrentDay} - Treasure: {GameManager.Instance.Treasure}";
	}

	private void OnDressPartyPressed()
	{
		sceneManager.GoToCloset();
	}

	private void OnDesignsPressed()
	{
		sceneManager.GoToDesignShop();
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
	/*
		DungeonManager.Instance.StartRun(testDungeon, activeParty);

		while (DungeonManager.Instance.IsRunActive)
		{
			DungeonManager.Instance.AdvanceFloor();
		}
	*/
	
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
		AudioManager.Instance.PlayMusic(shopMusic);
		DungeonManager.Instance.EndRun();
		sceneManager.GoToReceipt();
	}
}
