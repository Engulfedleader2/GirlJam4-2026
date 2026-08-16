using Godot;
using System.Collections.Generic;

public class HeroSelectionUI : Control
{
	private const int PageSize = 3;

	private SceneManager sceneManager;
	private CheckBox[] rowCheckboxes;
	private Button prevPageButton;
	private Button nextPageButton;

	private int pageIndex;

	private AudioStream[] clipboardOpenSFX;
	private AudioStream[] adventurerSelectSFX;
	private AudioStream[] clickSFX;
	private AudioStream[] backSFX;

	public override void _Ready()
	{
		sceneManager = GetNode<SceneManager>("/root/SceneManager");

		rowCheckboxes = new[]
		{
			GetNode<CheckBox>("Row0"),
			GetNode<CheckBox>("Row1"),
			GetNode<CheckBox>("Row2")
		};

		for (int i = 0; i < rowCheckboxes.Length; i++)
		{
			rowCheckboxes[i].Connect(
				"toggled", this, nameof(OnRowToggled), new Godot.Collections.Array { i }
			);
		}

		prevPageButton = GetNode<Button>("PrevPageButton");
		nextPageButton = GetNode<Button>("NextPageButton");

		prevPageButton.Connect("pressed", this, nameof(OnPrevPagePressed));
		nextPageButton.Connect("pressed", this, nameof(OnNextPagePressed));

		GetNode<Button>("BackButton").Connect(
			"pressed", this, nameof(OnBackPressed)
		);

		clipboardOpenSFX = new[]
		{
			GD.Load<AudioStream>("res://Assets/Audio/SFX/MENU/SFX_MENU_Clipboard Open_01.wav"),
			GD.Load<AudioStream>("res://Assets/Audio/SFX/MENU/SFX_MENU_Clipboard Open_02.wav")
		};

		adventurerSelectSFX = new[]
		{
			GD.Load<AudioStream>("res://Assets/Audio/SFX/ADV/SFX_ADV_Grunt_Female_01.wav"),
			GD.Load<AudioStream>("res://Assets/Audio/SFX/ADV/SFX_ADV_Grunt_Female_02.wav"),
			GD.Load<AudioStream>("res://Assets/Audio/SFX/ADV/SFX_ADV_Grunt_Male_01.wav"),
			GD.Load<AudioStream>("res://Assets/Audio/SFX/ADV/SFX_ADV_Grunt_Male_02.wav")
		};

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

		AudioManager.Instance.PlayRandomSFX(clipboardOpenSFX);

		Refresh();
	}

	private void Refresh()
	{
		List<Adventurer> roster = AdventurerManager.Instance.Roster;
		int pageCount = Mathf.Max(1, Mathf.CeilToInt((float)roster.Count / PageSize));
		pageIndex = Mathf.Clamp(pageIndex, 0, pageCount - 1);

		for (int row = 0; row < rowCheckboxes.Length; row++)
		{
			int rosterIndex = pageIndex * PageSize + row;
			CheckBox checkbox = rowCheckboxes[row];

			if (rosterIndex < roster.Count)
			{
				Adventurer adventurer = roster[rosterIndex];
				string name = string.IsNullOrEmpty(adventurer.Name) ? "Adventurer" : adventurer.Name;

				checkbox.Visible = true;
				checkbox.Text = $"{name} (HP {adventurer.CurrentHP}/{adventurer.MaxHP})";
				checkbox.Pressed = AdventurerManager.Instance.ActiveParty.Contains(adventurer);
			}
			else
			{
				checkbox.Visible = false;
			}
		}

		prevPageButton.Disabled = pageIndex <= 0;
		nextPageButton.Disabled = pageIndex >= pageCount - 1;
	}

	private void OnRowToggled(bool pressed, int row)
	{
		List<Adventurer> roster = AdventurerManager.Instance.Roster;
		int rosterIndex = pageIndex * PageSize + row;

		if (rosterIndex < roster.Count)
		{
			AdventurerManager.Instance.SetActive(roster[rosterIndex], pressed);
			AudioManager.Instance.PlayRandomSFX(adventurerSelectSFX);
		}

		Refresh();
	}

	private void OnPrevPagePressed()
	{
		AudioManager.Instance.PlayRandomSFX(clickSFX);
		pageIndex--;
		Refresh();
	}

	private void OnNextPagePressed()
	{
		AudioManager.Instance.PlayRandomSFX(clickSFX);
		pageIndex++;
		Refresh();
	}

	private void OnBackPressed()
	{
		AudioManager.Instance.PlayRandomSFX(backSFX);
		sceneManager.GoToMainGame();
	}
}
