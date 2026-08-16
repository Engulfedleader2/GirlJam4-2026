using Godot;
using System.Collections.Generic;

public class HireShopUI : Control
{
	private SceneManager sceneManager;
	private Label headerLabel;
	private VBoxContainer catalogList;
	private PackedScene heroCardScene;

	private AudioStream[] catalogOpenSFX;
	private AudioStream[] backSFX;
	private AudioStream[] hireSFX;

	public override void _Ready()
	{
		sceneManager = GetNode<SceneManager>("/root/SceneManager");

		headerLabel = GetNode<Label>("VBox/HeaderLabel");
		catalogList = GetNode<VBoxContainer>("VBox/CatalogList");
		catalogList.AddConstantOverride("separation", 5);

		heroCardScene = GD.Load<PackedScene>("res://Scenes/UI/HeroCard.tscn");
		GetNode<Button>("BackButton").Connect(
			"pressed", this, nameof(OnBackPressed)
		);

		AudioManager.Instance.PlayLayeredMusic(new[]
		{
			GD.Load<AudioStream>("res://Assets/Audio/Music/ShopMusic/Layer 1 (Shop).mp3"),
			GD.Load<AudioStream>("res://Assets/Audio/Music/ShopMusic/Layer 2 (Shop).mp3")
		});

		catalogOpenSFX = new[]
		{
			GD.Load<AudioStream>("res://Assets/Audio/SFX/MENU/SFX_MENU_Catalogue Open_01.wav"),
			GD.Load<AudioStream>("res://Assets/Audio/SFX/MENU/SFX_MENU_Catalogue Open_02.wav")
		};

		backSFX = new[]
		{
			GD.Load<AudioStream>("res://Assets/Audio/SFX/UI/SFX_UI_Back_01.wav"),
			GD.Load<AudioStream>("res://Assets/Audio/SFX/UI/SFX_UI_Back_02.wav"),
			GD.Load<AudioStream>("res://Assets/Audio/SFX/UI/SFX_UI_Back_03.wav")
		};

		hireSFX = new[]
		{
			GD.Load<AudioStream>("res://Assets/Audio/SFX/ADV/SFX_ADV_Hire_Female_01.wav"),
			GD.Load<AudioStream>("res://Assets/Audio/SFX/ADV/SFX_ADV_Hire_Female_02.wav"),
			GD.Load<AudioStream>("res://Assets/Audio/SFX/ADV/SFX_ADV_Hire_Male_01.wav"),
			GD.Load<AudioStream>("res://Assets/Audio/SFX/ADV/SFX_ADV_Hire_Male_02.wav")
		};

		AudioManager.Instance.PlayRandomSFX(catalogOpenSFX);

		Refresh();
	}

	private void Refresh()
	{
		headerLabel.Text = $"Mail Order Heroes - Treasure: {GameManager.Instance.Treasure}";

		RefreshCatalog();
	}

	private void RefreshCatalog()
	{
		foreach (Node child in catalogList.GetChildren())
		{
			child.QueueFree();
		}

		List<Adventurer> catalog = AdventurerManager.Instance.Catalog;

		for (int i = 0; i < catalog.Count; i++)
		{
			/*
			var button = new Button { Text = $"Hire for {AdventurerManager.Instance.HireCost}g" };
			button.Connect("pressed", this, nameof(OnHirePressed), new Godot.Collections.Array { i });
			catalogList.AddChild(button);
			*/
			var card = (HeroCard)heroCardScene.Instance();
			catalogList.AddChild(card);
			card.SetupCard(catalog[i], AdventurerManager.Instance.HireCost);
			card.Connect("pressed", this, nameof(OnHirePressed), new Godot.Collections.Array { i });
		}
	}
	
	private void OnHirePressed(int catalogIndex)
	{
		List<Adventurer> catalog = AdventurerManager.Instance.Catalog;

		if (catalogIndex < catalog.Count)
		{
			if (AdventurerManager.Instance.HireAdventurer(catalog[catalogIndex]))
			{
				AudioManager.Instance.PlayRandomSFX(hireSFX);
			}
		}

		Refresh();
	}

	private void OnBackPressed()
	{
		AudioManager.Instance.PlayRandomSFX(backSFX);
		sceneManager.GoToMainGame();
	}
}
