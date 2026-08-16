using Godot;
using System;

public class LettersUI : Control
{
	private Sprite[] pages;
	private Button prevButton;
	private Button nextButton;
	private ColorRect backDrop;
	
	private int currentDay = 1;
	private int unlockedDay;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		pages = new[]
		{
			GetNode<Sprite>("Day1"),
			GetNode<Sprite>("Day2"),
			GetNode<Sprite>("Day3"),
			GetNode<Sprite>("Day4"),
			GetNode<Sprite>("Day5"),
			GetNode<Sprite>("Day6"),
			GetNode<Sprite>("Day7")
		};
		
		prevButton = GetNode<Button>("PrevButton");
		nextButton = GetNode<Button>("NextButton");
		backDrop = GetNode<ColorRect>("ColorRect");
		
		backDrop.Connect("gui_input", this, nameof(OnBackdropInput));
		
		prevButton.Connect("pressed", this, nameof(OnPrevPressed));
		nextButton.Connect("pressed", this, nameof(OnNextPressed));
		
		
		Visible = false;
	}
	
	public void ShowDay(int day)
	{
		unlockedDay = day;
		ShowPage(day);
	}
	
	public void ShowArchive (int currentGameDay)
	{
		unlockedDay = currentGameDay;
		ShowPage(currentGameDay);
	}
	
	public void ShowPage(int day)
	{
		currentDay = Mathf.Clamp(day, 1, pages.Length);
		
		for(int i = 0; i < pages.Length; i++)
		{
			pages[i].Visible = i == currentDay - 1;
		}
		
		if (currentDay <= 1) 
		{
			prevButton.Disabled = true;
		}
		if (currentDay >= unlockedDay) {
			nextButton.Disabled = true;
		}
		
		Visible = true;
	}
	private void OnBackdropInput(InputEvent @event) {
		
		var mouseEvent = @event as InputEventMouseButton;
		if(mouseEvent != null && mouseEvent.Pressed && mouseEvent.ButtonIndex == (int)ButtonList.Left)
		{
			Visible = false;
		}
	}
	private void OnPrevPressed() => ShowPage(currentDay - 1);
	private void OnNextPressed() => ShowPage(currentDay + 1);
}
