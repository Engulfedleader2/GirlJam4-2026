using Godot;

public class MainMenu : Node2D
{
	private GameManager gameManager;
	private SceneManager sceneManager;

	private Button playButton;
	private Button settingsButton;
	private Button quitButton;
	private TextureButton settingsIcon;
	private AudioStream[] clickSFX;

	public override void _Ready()
	{
		gameManager = GetNode<GameManager>("/root/GameManager");
		sceneManager = GetNode<SceneManager>("/root/SceneManager");

		AudioManager.Instance.PlayMusic(GD.Load<AudioStream>("res://Assets/Audio/Music/SLORP.mp3"));
		clickSFX = new[]
		{
			GD.Load<AudioStream>("res://Assets/Audio/SFX/UI/SFX_UI_Click_01.wav"),
			GD.Load<AudioStream>("res://Assets/Audio/SFX/UI/SFX_UI_Click_02.wav"),
			GD.Load<AudioStream>("res://Assets/Audio/SFX/UI/SFX_UI_Click_03.wav"),
			GD.Load<AudioStream>("res://Assets/Audio/SFX/UI/SFX_UI_Click_04.wav")
		};

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
		gameManager.StartGame();
		sceneManager.GoToMainGame();
	}

	private void OnSettingsPressed()
	{
		AudioManager.Instance.PlayRandomSFX(clickSFX);
		sceneManager.GoToSettingsMenu();
	}

	private void OnQuitPressed()
	{
		AudioManager.Instance.PlayRandomSFX(clickSFX);
		sceneManager.QuitGame();
	}
}
