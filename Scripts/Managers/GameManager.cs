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

	public GamePhase CurrentPhase { get; private set; }
	public int CurrentDay { get; private set; }

	public override void _Ready()
	{
		Instance = this;
		
	}

	public void StartGame()
	{
		CurrentDay = 1;
		CurrentPhase = GamePhase.Dressing;
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

	public void ResetGame()
	{
		CurrentDay = 0;
		CurrentPhase = GamePhase.Dressing;
	}
}
