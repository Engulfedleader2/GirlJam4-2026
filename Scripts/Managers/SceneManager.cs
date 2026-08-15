using Godot;
using System;

public class SceneManager : Node
{

	private const string MainMenuScenePath = "res://Scenes/Screens/MainMenu.tscn";
	private const string MainGameScenePath = "res://Scenes/Screens/MainGame.tscn";
	private const string SettingsMenuScenePath = "res://Scenes/Screens/SettingsMenu.tscn";
	private const string PauseMenuScenePath = "res://Scenes/Screens/PauseMenu.tscn";
	private const string LeaderboardScenePath = "res://Scenes/Screens/Leaderboard.tscn";
	private const string CreditsMenuScenePath = "res://Scenes/Screens/CreditsMenu.tscn";
	private const string GraveyardScenePath = "res://Scenes/Screens/Graveyard.tscn";
	private const string ClosetScenePath = "res://Scenes/Screens/Closet.tscn";
	private const string DesignShopScenePath = "res://Scenes/Screens/DesignShop.tscn";
	private const string ReceiptScenePath = "res://Scenes/Screens/Receipt.tscn";
	private const string HeroSelectionScenePath = "res://Scenes/Screens/HeroSelection.tscn";
	private const string HireShopScenePath = "res://Scenes/Screens/HireShop.tscn";
	private const string DesignCatologueScenePath = "res://Scenes/Screens/DesignCatologue.tscn";


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

	public void GoToGraveyard()
	{
		ChangeScene(GraveyardScenePath);
	}

	public void GoToCloset()
	{
		ChangeScene(ClosetScenePath);
	}

	public void GoToDesignShop()
	{
		ChangeScene(DesignShopScenePath);
	}

	public void GoToReceipt()
	{
		ChangeScene(ReceiptScenePath);
	}

	public void GoToHeroSelection()
	{
		ChangeScene(HeroSelectionScenePath);
	}

	public void GoToHireShop()
	{
		ChangeScene(HireShopScenePath);
	}

	public void GoToDesignCatologue()
	{
		ChangeScene(DesignCatologueScenePath);
	}

	public void QuitGame()
	{
		GetTree().Quit();
	}


}
