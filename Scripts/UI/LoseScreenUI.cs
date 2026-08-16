using Godot;

public class LoseScreenUI : Control
{
	private SceneManager sceneManager;
	private AudioStream[] backSFX;

	public override void _Ready()
	{
		sceneManager = GetNode<SceneManager>("/root/SceneManager");

		backSFX = new[]
		{
			GD.Load<AudioStream>("res://Assets/Audio/SFX/UI/SFX_UI_Back_01.wav"),
			GD.Load<AudioStream>("res://Assets/Audio/SFX/UI/SFX_UI_Back_02.wav"),
			GD.Load<AudioStream>("res://Assets/Audio/SFX/UI/SFX_UI_Back_03.wav")
		};

		GetNode<Button>("MainMenuButton").Connect(
			"pressed",
			this,
			nameof(OnMainMenuPressed)
		);
	}

	private void OnMainMenuPressed()
	{
		AudioManager.Instance.PlayRandomSFX(backSFX);
		GameManager.Instance.ResetGame();
		sceneManager.GoToMainMenu();
	}
}
