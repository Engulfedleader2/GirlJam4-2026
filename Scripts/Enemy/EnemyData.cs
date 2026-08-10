using Godot;

// Enemy data types

public class EnemyData : Resource
{
	[Export] public string EnemyName;
	[Export] public int MaxHP = 1;
	[Export] public int Attack = 1;
	[Export] public Texture Sprite;
}
