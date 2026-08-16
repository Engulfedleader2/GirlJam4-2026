using Godot;

public class MainMenu : Node2D
{
	private GameManager gameManager;
	private SceneManager sceneManager;

	private Button playButton;
	private Button settingsButton;
	private Button quitButton;
	private TextureButton settingsIcon;
	private AudioStream startGameSFX;

	public override void _Ready()
	{
		gameManager = GetNode<GameManager>("/root/GameManager");
		sceneManager = GetNode<SceneManager>("/root/SceneManager");
		startGameSFX = GD.Load<AudioStream>("res://Assets/Audio/SFX/SFX_UI_Start_Game.wav");
		AudioManager.Instance.PlayMusic(GD.Load<AudioStream>("res://Assets/Audio/Music/SLORP.mp3"));
		playButton = GetNode<Button>(
			"UI/CenterContainer/VBoxContainer/PlayButton"
		);

		settingsButton = GetNode<Button>(
			"UI/CenterContainer/VBoxContainer/SettingsButton"
		);

		quitButton = GetNode<Button>(
			"UI/CenterContainer/VBoxContainer/QuitButton"
		);

		settingsIcon = GetNode<TextureButton>("Setting_Icon");

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

		settingsIcon.Connect(
			"pressed",
			this,
			nameof(OnSettingsPressed)
		);
	}

	private void OnPlayPressed()
	{
		AudioManager.Instance.PlaySFX(startGameSFX);
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
