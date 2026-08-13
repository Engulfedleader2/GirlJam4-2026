using Godot;
using System.Collections.Generic;

public class DungeonViewTest : Control
{
	
	private Viewport _viewport;
	private DungeonView _dungeonView;
	private TextureRect _display;
	
	public override void _Ready()
	{
		_viewport = GetNode<Viewport>("Viewport");
		_dungeonView = GetNode<DungeonView>("Viewport/DungeonView");
		_display = GetNode<TextureRect>("TextureRect");
		
		ConfigureViewport();
		ConfigureDisplay();
		_dungeonView.StartPlayback(BuildDemoParty(), BuildDemoTape());
	}
	
	private void ConfigureViewport()
	{
		_viewport.RenderTargetVFlip = true;
		_viewport.RenderTargetUpdateMode = Viewport.UpdateMode.Always;
	}
	
	private void ConfigureDisplay()
	{

		ViewportTexture tex = _viewport.GetTexture();
		tex.Flags = 0; 
		_display.Texture = tex;
	}

	private List<Adventurer> BuildDemoParty() 
	{
		return new List<Adventurer>
		{
			new Adventurer { Name = "Tank", MaxHP = 14, CurrentHP = 14, Attack = 3},
			new Adventurer { Name = "DPS", MaxHP = 10, CurrentHP = 10, Attack = 4},
		};
	}
	
	private Queue<DungeonEvent> BuildDemoTape()
	{
		Texture slime = GD.Load<Texture>("res://AssetDump/Monsters/Slime.png");
		var tape = new Queue<DungeonEvent>();
		
		tape.Enqueue(new DungeonEvent {Type = DungeonEventType.EnterFloor});
		tape.Enqueue(Fight(slime, new[] {14,8}, new[] {false, false}));
		tape.Enqueue(Fight(slime, new[] {11,5}, new[] {false, false}));
		tape.Enqueue(new DungeonEvent {Type = DungeonEventType.FloorCleared});
		
		tape.Enqueue(Fight(slime, new[] {9,0}, new[] {false, true}, boss: true));
		tape.Enqueue(new DungeonEvent {Type = DungeonEventType.FloorCleared});
		tape.Enqueue(new DungeonEvent {Type = DungeonEventType.RunEnded});
		return tape;
	}
	
	private DungeonEvent Fight(Texture sprite, int[] hp, bool[] dead, bool boss = false)
	{
		return new DungeonEvent
		{
			Type = DungeonEventType.Fight,
			EnemyName = "Slime",
			EnemySprite = sprite,
			IsBoss = boss,
			PartyHp = hp,
			PartyDead = dead,
		};
	}
}
