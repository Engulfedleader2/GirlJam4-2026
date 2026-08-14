using Godot;
using System.Collections.Generic;

public class HireShopUI : Control
{
	private SceneManager sceneManager;
	private Label headerLabel;
	private VBoxContainer catalogList;

	public override void _Ready()
	{
		sceneManager = GetNode<SceneManager>("/root/SceneManager");

		headerLabel = GetNode<Label>("VBox/HeaderLabel");
		catalogList = GetNode<VBoxContainer>("VBox/CatalogList");

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
			var button = new Button { Text = $"Hire for {AdventurerManager.Instance.HireCost}g" };
			button.Connect("pressed", this, nameof(OnHirePressed), new Godot.Collections.Array { i });
			catalogList.AddChild(button);
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
