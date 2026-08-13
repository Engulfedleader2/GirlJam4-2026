using Godot;
using System;

public enum DungeonEventType { EnterFloor, Fight, FloorCleared, RunEnded}
public class DungeonEvent
{
	public DungeonEventType Type;
	
	public string EnemyName;
	public Texture EnemySprite;
	public bool IsBoss;
	
	public int[] PartyHp;
	public bool[] PartyDead;
}
