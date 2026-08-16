using Godot;

public class LoseScreenUI : Control
{
	private SceneManager sceneManager;

	public override void _Ready()
	{
		sceneManager = GetNode<SceneManager>("/root/SceneManager");

		GetNode<Button>("MainMenuButton").Connect(
			"pressed",
			this,
			nameof(OnMainMenuPressed)
		);
	}

	private void OnMainMenuPressed()
	{
		GameManager.Instance.ResetGame();
		sceneManager.GoToMainMenu();
	}
}
