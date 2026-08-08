using Godot;
using System;

public class SceneManager : Node
{

	private const string MainMenuScenePath = "res://Scenes/MainMenu.tscn";
	private const string MainGameScenePath = "res://Scenes/MainGame.tscn";
	private const string SettingsMenuScenePath = "res://Scenes/SettingsMenu.tscn";
	private const string PauseMenuScenePath = "res://Scenes/PauseMenu.tscn";
	private const string LeaderboardScenePath = "res://Scenes/Leaderboard.tscn";
	private const string CreditsMenuScenePath = "res://Scenes/CreditsMenu.tscn";


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

	public void GoToMainGame()
	{
		ChangeScene(MainGameScenePath);
	}
	
	public void GoToPauseMenu()
	{
		ChangeScene(PauseMenuScenePath);
	}

	public void GoToSettingsMenu()
	{
		ChangeScene(SettingsMenuScenePath);
	}
	public void GoToCreditsMenu()
	{
		ChangeScene(CreditsMenuScenePath);
	}

	public void GoToLeaderboard()
	{
		//TODO: This might need a DB hook up if we decide to go this route
		ChangeScene(LeaderboardScenePath);
	}

	public void QuitGame()
	{
		GetTree().Quit();
	}


}
