using Godot;

public class MainMenu : Node2D
{
	private GameManager gameManager;
	private SceneManager sceneManager;

	private Button playButton;
	private Button settingsButton;
	private Button quitButton;

	public override void _Ready()
	{
		gameManager = GetNode<GameManager>("/root/GameManager");
		sceneManager = GetNode<SceneManager>("/root/SceneManager");

		playButton = GetNode<Button>(
			"UI/CenterContainer/VBoxContainer/PlayButton"
		);

		settingsButton = GetNode<Button>(
			"UI/CenterContainer/VBoxContainer/SettingsButton"
		);

		quitButton = GetNode<Button>(
			"UI/CenterContainer/VBoxContainer/QuitButton"
		);

		playButton.Connect(
			"pressed",
			this,
			nameof(OnPlayPressed)
		);

		settingsButton.Connect(
			"pressed",
			this,
			nameof(OnSettingsPressed)
		);

		quitButton.Connect(
			"pressed",
			this,
			nameof(OnQuitPressed)
		);
	}

	private void OnPlayPressed()
	{
		gameManager.StartGame();
		sceneManager.GoToMainGame();
	}

	private void OnSettingsPressed()
	{
		sceneManager.GoToSettingsMenu();
	}

	private void OnQuitPressed()
	{
		sceneManager.QuitGame();
	}
}
