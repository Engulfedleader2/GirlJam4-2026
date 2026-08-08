using Godot;

public class SettingsMenu : Node2D
{
	private AudioManager audioManager;
	private SceneManager sceneManager;

	private CheckButton musicToggle;
	private CheckButton sfxToggle;
	private Button creditsButton;
	private Button backButton;

	public override void _Ready()
	{
		audioManager = GetNode<AudioManager>("/root/AudioManager");
		sceneManager = GetNode<SceneManager>("/root/SceneManager");

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
		
	}

	private void OnSFXToggled(bool enabled)
	{

	}

	private void OnCreditsPressed()
	{
		sceneManager.GoToCreditsMenu();
	}

	private void OnBackPressed()
	{
		sceneManager.GoToMainMenu();
	}
}
