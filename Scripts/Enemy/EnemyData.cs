using Godot;

// Enemy data types

public class EnemyData : Resource
{
	[Export] public string EnemyName;
	[Export] public int MaxHP = 1;
	[Export] public int Attack = 1;
	[Export] public Texture Sprite;
	[Export] public int Level = 1;
	[Export] public int Gold = 5;
	[Export] public bool IsBoss = false;
}
