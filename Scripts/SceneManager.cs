using Godot;
using System;

public class SceneManager : Node
{
	public static SceneManager Instance { get; private set; }

	private const string MainMenuScenePath = "res://Scenes/MainMenu.tscn";
	private const string SettingsMenuScenePath = "res://Scenes/SettingsMenu.tscn";
	private const string PauseMenuScenePath = "res://Scenes/PauseMenu.tscn";
	private const string LeaderboardScenePath = "res://Scenes/Leaderboard.tscn";


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
	}

	public void ChangeScene(string scenePath)
	{
		if (string.IsNullOrEmpty(scenePath))
		{
			GD.PrintErr("Scene path is null or empty.");
			return;
		}

		GetTree().ChangeScene(scenePath);
	}

	public void GoToMainMenu()
	{
		ChangeScene(MainMenuScenePath);
	}
	
	public void GoToPauseMenu()
	{
		ChangeScene(PauseMenuScenePath);
	}

	public void GoToSettingsMenu()
	{
		ChangeScene(SettingsMenuScenePath);
	}

	public void goToLeaderboard()
	{
		//TODO: This might need a DB hook up if we decide to go this route
		ChangeScene(LeaderboardScenePath);
	}

	public void quitGame()
	{
		GetTree().Quit();
	}


}
