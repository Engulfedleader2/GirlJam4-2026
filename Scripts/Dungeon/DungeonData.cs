using Godot;

// The potential dungeon config stuff

public class DungeonData : Resource
{
	[Export] public string DungeonName;
	[Export] public int Floors = 1;
	[Export] public EnemyData Enemy;
}
