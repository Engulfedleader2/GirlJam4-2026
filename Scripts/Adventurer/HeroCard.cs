using Godot;
using System;

public class HeroCard : Button
{
	private TextureRect portraitRect;
	private Label priceLabel;
	private Label nameLabel;
	private Label statsLabel;
	private Label  dreamLabel;
	
	public override void _Ready()
	{
		portraitRect = GetNode<TextureRect>("Row/Portrait/PortraitBox");
		priceLabel = GetNode<Label>("Row/Portrait/PriceLabel");
		nameLabel = GetNode<Label>("Row/Info/NameLabel");
		statsLabel = GetNode<Label>("Row/Info/StatsLabel");
		dreamLabel = GetNode<Label>("Row/Info/DreamLabel");
	}

	public void SetupCard(Adventurer adv, int price) 
	{
		portraitRect.Texture = GD.Load<Texture>("res://Assets/Characters/hero.png");
		priceLabel.Text = $"{price}g";
		nameLabel.Text = string.IsNullOrEmpty(adv.Name) ? "Adventurer" : adv.Name;
		statsLabel.Text = $"HP {adv.MaxHP} \u00b7 ATK {adv.Attack}";
		dreamLabel.Text = adv.Dream;
	}
}
