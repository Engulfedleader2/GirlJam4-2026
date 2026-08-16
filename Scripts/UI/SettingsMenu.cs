using Godot;

public class SettingsMenu : Node2D
{
	private AudioManager audioManager;
	private SceneManager sceneManager;

	private CheckButton musicToggle;
	private CheckButton sfxToggle;
	private Button creditsButton;
	private Button backButton;

	private AudioStream[] clickSFX;
	private AudioStream[] backSFX;

	public override void _Ready()
	{
		audioManager = GetNode<AudioManager>("/root/AudioManager");
		sceneManager = GetNode<SceneManager>("/root/SceneManager");

		clickSFX = new[]
		{
			GD.Load<AudioStream>("res://Assets/Audio/SFX/UI/SFX_UI_Click_01.wav"),
			GD.Load<AudioStream>("res://Assets/Audio/SFX/UI/SFX_UI_Click_02.wav"),
			GD.Load<AudioStream>("res://Assets/Audio/SFX/UI/SFX_UI_Click_03.wav"),
			GD.Load<AudioStream>("res://Assets/Audio/SFX/UI/SFX_UI_Click_04.wav")
		};

		backSFX = new[]
		{
			GD.Load<AudioStream>("res://Assets/Audio/SFX/UI/SFX_UI_Back_01.wav"),
			GD.Load<AudioStream>("res://Assets/Audio/SFX/UI/SFX_UI_Back_02.wav"),
			GD.Load<AudioStream>("res://Assets/Audio/SFX/UI/SFX_UI_Back_03.wav")
		};

		musicToggle = GetNode<CheckButton>(
			"UI/CenterContainer/VBoxContainer/MusicToggle"
		);

		sfxToggle = GetNode<CheckButton>(
			"UI/CenterContainer/VBoxContainer/SFXToggle"
		);

		creditsButton = GetNode<Button>(
			"UI/CenterContainer/VBoxContainer/CreditsButton"
		);

		backButton = GetNode<Button>(
			"UI/CenterContainer/VBoxContainer/BackButton"
		);

		musicToggle.Connect(
			"toggled",
			this,
			nameof(OnMusicToggled)
		);

		sfxToggle.Connect(
			"toggled",
			this,
			nameof(OnSFXToggled)
		);

		creditsButton.Connect(
			"pressed",
			this,
			nameof(OnCreditsPressed)
		);

		backButton.Connect(
			"pressed",
			this,
			nameof(OnBackPressed)
		);
	}

	private void OnMusicToggled(bool enabled)
	{
		AudioManager.Instance.PlayRandomSFX(clickSFX);
	}

	private void OnSFXToggled(bool enabled)
	{
		AudioManager.Instance.PlayRandomSFX(clickSFX);
	}

	private void OnCreditsPressed()
	{
		AudioManager.Instance.PlayRandomSFX(clickSFX);
		sceneManager.GoToCreditsMenu();
	}

	private void OnBackPressed()
	{
		AudioManager.Instance.PlayRandomSFX(backSFX);
		sceneManager.GoToMainMenu();
	}
}
