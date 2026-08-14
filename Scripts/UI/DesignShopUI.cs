using Godot;

// Lists every locked clothing item across all slots so the player can buy
// designs to unlock them for the Closet.

public class DesignShopUI : Control
{
	private const string ClothingDataRoot = "res://Resources/Clothing";

	private SceneManager sceneManager;
	private Label headerLabel;
	private VBoxContainer designsList;

	public override void _Ready()
	{
		sceneManager = GetNode<SceneManager>("/root/SceneManager");

		headerLabel = GetNode<Label>("VBox/HeaderLabel");
		designsList = GetNode<VBoxContainer>("VBox/DesignsList");

		GetNode<Button>("BackButton").Connect(
			"pressed", this, nameof(OnBackPressed)
		);

		Refresh();
	}

	private void Refresh()
	{
		headerLabel.Text = $"Designs - Treasure: {GameManager.Instance.Treasure}";

		RefreshDesigns();
	}

	private void RefreshDesigns()
	{
		foreach (Node child in designsList.GetChildren())
		{
			child.QueueFree();
		}

		foreach (ClothingSlot slot in System.Enum.GetValues(typeof(ClothingSlot)))
		{
			string folder = $"{ClothingDataRoot}/{slot}";
			var dir = new Directory();

			if (dir.Open(folder) != Error.Ok)
			{
				continue;
			}

			dir.ListDirBegin(true, true);
			string fileName = dir.GetNext();

			while (!string.IsNullOrEmpty(fileName))
			{
				if (fileName.EndsWith(".tres"))
				{
					ClothingData item = GD.Load<ClothingData>($"{folder}/{fileName}");

					if (!DesignManager.Instance.IsUnlocked(item))
					{
						var button = new Button { Text = $"{item.ItemName} - {DesignManager.Instance.GetPrice(item)}g" };
						button.Connect("pressed", this, nameof(OnBuyDesignPressed), new Godot.Collections.Array { item });
						designsList.AddChild(button);
					}
				}

				fileName = dir.GetNext();
			}
		}
	}

	private void OnBuyDesignPressed(ClothingData item)
	{
		DesignManager.Instance.BuyDesign(item);
		Refresh();
	}

	private void OnBackPressed()
	{
		sceneManager.GoToMainGame();
	}
}
