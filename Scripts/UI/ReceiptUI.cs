using Godot;
using System.Collections.Generic;

public class ReceiptUI : Control
{
	private SceneManager sceneManager;
	private Label recapLabel;

	public override void _Ready()
	{
		sceneManager = GetNode<SceneManager>("/root/SceneManager");
		recapLabel = GetNode<Label>("HBoxContainer/VBoxContainer/RecapLabel");

		GetNode<Button>("HBoxContainer/VBoxContainer/ContinueButton").Connect(
			"pressed", this, nameof(OnContinuePressed)
		);
		recapLabel.AddColorOverride("font_color", new Color(0.1f, 0.1f, 0.1f));
		recapLabel.Text = BuildRecapText();
	}

	private string BuildRecapText()
	{
		/*
		DungeonManager dungeonManager = DungeonManager.Instance;

		// A guard for testing
		if (dungeonManager.CurrentDungeon == null)
		{
			return "No run to report yet.";
		}

		var lines = new List<string>
		{
			$"{dungeonManager.CurrentDungeon.DungeonName} - Floor {dungeonManager.CurrentFloor}",
			$"Recovered: {dungeonManager.GoldEarned}g"
		};

		foreach (Adventurer adventurer in dungeonManager.Party)
		{
			string name = string.IsNullOrEmpty(adventurer.Name) ? "Adventurer" : adventurer.Name;
			lines.Add(adventurer.IsAlive ? $"{name} returned." : $"{name} did not return.");
		}

		return string.Join("\n", lines);
		*/
		
		RunResult run =  DungeonManager.Instance.LastRun;
		if (run == null)
			return "No run to report yet.";
		
		var lines = new List<string>
		{
			run.Wiped ? "The party was lost..." : "The party returned!",
			$"Reached Floor {run.FloorReached}",
			$"Recovered: {run.GoldEarned}g",
		};
		lines.AddRange(run.AdventurerOutcomes);
		return string.Join("\n", lines);
	}

	private void OnContinuePressed()
	{
		GameManager.Instance.AdvanceDay();
		sceneManager.GoToMainGame();
	}
}
