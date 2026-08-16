using Godot;
using System;

public class GameManager : Node
{
	private const string ConfigPath = "res://Resources/Config/GameConfig.cfg";
	private bool _transition;
	private bool _tutorialSeen;
	public static GameManager Instance {get; private set; }

	public enum GamePhase
	{
		Store,
		Dressing,
		Dungeon,
		Results
	}

	public enum GameOverReason
	{
		None,
		Won,
		Bankrupt,
		MissedDeadline,
		PartyWiped
	}

	// Cursor was too big so we scaled it down a bit
	private const int CursorSize = 48;

	// This is in the game config now
	private int startingTreasure;
	private int debtGoal;
	private int dayLimit;

	public GamePhase CurrentPhase { get; private set; }
	public int CurrentDay { get; private set; }
	public int Treasure { get; private set; }
	public bool PendingVentureForth { get; set;}
	public bool IsGameOver { get; private set; }
	public GameOverReason Reason { get; private set; }
	public bool HasSeenTutorial() {
		return _tutorialSeen;
	}
	public void MarkTutorialSeen()
	{
		_tutorialSeen = true;
	}
	public override void _Ready()
	{
		Instance = this;

		var config = new ConfigFile();
		config.Load(ConfigPath);
		startingTreasure = (int)config.GetValue("Game", "starting_treasure", 30);
		debtGoal = (int)config.GetValue("Game", "debt_goal", 500);
		dayLimit = (int)config.GetValue("Game", "day_limit", 10);

		Texture cursorSource = GD.Load<Texture>("res://Assets/UI/Cursor.png");
		Image cursorImage = cursorSource.GetData();
		cursorImage.Resize(CursorSize, CursorSize, Image.Interpolation.Lanczos);

		var cursor = new ImageTexture();
		cursor.CreateFromImage(cursorImage);

		// Potating source
		Vector2 hotspot = new Vector2(78, 65) * CursorSize / cursorSource.GetWidth();
		Input.SetCustomMouseCursor(cursor, Input.CursorShape.Arrow, hotspot);
	}

	public void StartGame()
	{
		CurrentDay = 1;
		CurrentPhase = GamePhase.Dressing;
		Treasure = startingTreasure;
		IsGameOver = false;
		Reason = GameOverReason.None;
		ActivateTransition();
		
		AdventurerManager.Instance.Reset();
		DesignManager.Instance.Reset();
	}

	public void AdvanceDay()
	{
		CurrentDay++;
		CurrentPhase = GamePhase.Dressing;
		ActivateTransition();

		AdventurerManager.Instance.RefreshCatalog();
		DesignManager.Instance.RefreshStock();

		CheckGameOverConditions();
	}

	public void CheckGameOverConditions()
	{
		if (Treasure >= debtGoal)
		{
			TriggerGameOver(GameOverReason.Won);
		}
		else if (Treasure <= 0 && AdventurerManager.Instance.Roster.Count == 0)
		{
			TriggerGameOver(GameOverReason.Bankrupt);
		}
		else if (CurrentDay > dayLimit)
		{
			TriggerGameOver(GameOverReason.MissedDeadline);
		}
	}

	public void TriggerGameOver(GameOverReason reason)
	{
		if (IsGameOver)
		{
			return;
		}

		IsGameOver = true;
		Reason = reason;
	}
	public void SetGamePhases(GamePhase phase)
	{
		CurrentPhase = phase;
	}

	public void AddTreasure(int amount)
	{
		Treasure += amount;
	}

	// Returns false (and spends nothing) if there isn't enough treasure.
	public bool SpendTreasure(int amount)
	{
		if (amount > Treasure)
		{
			return false;
		}

		Treasure -= amount;
		return true;
	}

	public void ResetGame()
	{
		CurrentDay = 0;
		CurrentPhase = GamePhase.Dressing;
		Treasure = 0;
		IsGameOver = false;
		Reason = GameOverReason.None;
	}
	
	public void ActivateTransition()
	{
		_transition = true;
	}
	
	public bool ConsumeTransitionFlag()
	{
		bool temp = _transition;
		_transition = false;
		return temp;
	}
}
