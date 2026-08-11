using Godot;
using System;

public class GameManager : Node
{
	public static GameManager Instance {get; private set; }

	public enum GamePhase
	{
		Store,
		Dressing,
		Dungeon,
		Results
	}

	// Placeholder - enough to hire one adventurer and buy one design on day 1.
	private const int StartingTreasure = 30;

	// Cursor was too big so we scaled it down a bit
	private const int CursorSize = 48;

	public GamePhase CurrentPhase { get; private set; }
	public int CurrentDay { get; private set; }
	public int Treasure { get; private set; }

	public override void _Ready()
	{
		Instance = this;

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
		Treasure = StartingTreasure;
	}

	public void AdvanceDay()
	{
		CurrentDay++;
		CurrentPhase = GamePhase.Dressing;

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
	}
}
