using Godot;
using System.Collections.Generic;

public class HireShopUI : Control
{
	private SceneManager sceneManager;
	private Label headerLabel;
	private VBoxContainer catalogList;
	private PackedScene heroCardScene;

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
			AdventurerManager.Instance.HireAdventurer(catalog[catalogIndex]);
		}

		Refresh();
	}
	
	private void OnBackPressed()
	{
		sceneManager.GoToMainGame();
	}
}
