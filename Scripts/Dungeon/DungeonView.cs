using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

public class DungeonView : Node2D
{
	[Export] private float _scrollSpeed = 40f;
	[Export] private float _travelTime = 1.5f;
	[Export] private float _lungeTime = 0.2f;
	[Export] private float _lungeDistance =  40f;
	[Export] private PackedScene _partyDoll;
	[Export] private float _enemyEntranceTime = 0.8f;
	[Export] private float _enemyEntranceDistance = 90f;
	//[Export] private Vector2 _enemyScale = new Vector2(0.5f, 0.5f);
	
	private ParallaxBackground _parallax;
	private Node2D _partyRoot;
	private Tween _tween;
	private Position2D[] _adventurerSlots;
	private Position2D _enemySlot;
	private AnimatedSprite _fightScene;
	
	private enum DungeonState {Walking, Fighting, Idle}
	private DungeonState _state = DungeonState.Idle;
	private readonly List<SpriteDoll> _party = new List<SpriteDoll>();
	private Sprite _enemy;
	private Queue<DungeonEvent> _events;
	
	[Signal] public delegate void PlaybackFinished();
	
	public override void _Ready()
	{
		_parallax = GetNode<ParallaxBackground>("Background");
		_tween = GetNode<Tween>("Tween");
		_enemySlot = GetNode<Position2D>("Adventurers/Enemy");
		_fightScene = GetNode<AnimatedSprite>("FightScene");
		_partyRoot = GetNode<Node2D>("Adventurers");
		_adventurerSlots = new[]
		{
			GetNode<Position2D>("Adventurers/Adven1"),
			GetNode<Position2D>("Adventurers/Adven2"),
			GetNode<Position2D>("Adventurers/Adven3")
		};
		
		_fightScene.Visible = false;
	}
	
	public override void _Process(float delta)
	{
		if(_state == DungeonState.Walking)
			_parallax.ScrollOffset += new Vector2(-_scrollSpeed * delta, 0);
	}
	
	private void ClearParty()
	{
		foreach (SpriteDoll doll in _party)
			doll.QueueFree();
		
		_party.Clear();
		_enemy?.QueueFree();
		_enemy = null;
		_state = DungeonState.Idle;
	}
	public void StartPlayback(List<Adventurer> party, Queue<DungeonEvent> events)
	{
		ClearParty();
		SpawnParty(party);
		PlayTape(events);
	}
	
	public void PlayTape(Queue<DungeonEvent> events) 
	{
		_events = events;
		_ = ProcessEvents();
	}
	
	private async Task ProcessEvents()
	{
		while (_events.Count > 0)
		{
			DungeonEvent e = _events.Dequeue();
			switch (e.Type)
			{
				case DungeonEventType.EnterFloor:
					SetState(DungeonState.Walking);
					await Wait(_travelTime);
					break;
				case DungeonEventType.Fight:
					SetState(DungeonState.Walking);
					await Wait(_travelTime);
					SetState(DungeonState.Fighting);
					SpawnEnemy(e.EnemySprite);
					await WalkEnemyIn();
					await Lunge();
					
					await PlayFight();
					ApplyOutcome(e);
					await Recover();
					await Wait(0.2f);
					break;
					
				case DungeonEventType.FloorCleared:
					await Wait(0.3f);
					break;
				
				case DungeonEventType.RunEnded:
					SetState(DungeonState.Idle);
					break;
			}
		}
		EmitSignal(nameof(PlaybackFinished));
	}
	
	private void SetCombatantsVisible(bool visible) {
		foreach (SpriteDoll doll in _party)
			doll.Visible = visible;
		if  (_enemy != null)
			_enemy.Visible = visible;
	}
	private void SpawnEnemy(Texture sprite)
	{
		_enemy = new Sprite {Texture = sprite, Centered = true};
		_partyRoot.AddChild(_enemy);
		_enemy.Position = _enemySlot.Position + new Vector2(_enemyEntranceDistance, 0);
	}
	
	private async Task WalkEnemyIn()
	{
		_tween.InterpolateProperty(_enemy, "position",
			_enemy.Position, _enemySlot.Position,
			_enemyEntranceTime, Tween.TransitionType.Sine, Tween.EaseType.Out
		);
		_tween.Start();
		await ToSignal(_tween, "tween_all_completed");
	}
	private async Task Lunge()
	{
		for (int i = 0; i < _party.Count; i++)
		{
			if (!_party[i].Visible) continue;
			Vector2 home = _adventurerSlots[i].Position;
			_tween.InterpolateProperty(_party[i], "position", 
			home, home + new Vector2(_lungeDistance, 0), 
			_lungeTime, Tween.TransitionType.Back, Tween.EaseType.Out);
		}
		
		_tween.Start();
		await ToSignal(_tween, "tween_all_completed");
	}
	
	private async Task PlayFight()
	{
		SetCombatantsVisible(false);
		_fightScene.Visible = true;
		_fightScene.Frame = 0;
		_fightScene.Play("fight");
		await ToSignal(_fightScene, "animation_finished");
		_fightScene.Visible = false;
	}
	
	private void ApplyOutcome(DungeonEvent e)
	{
		for (int i = 0; i < _party.Count; i++)
		{
			bool dead = e.PartyDead != null && i < e.PartyDead.Length && e.PartyDead[i];
			_party[i].Visible = !dead;
		}
	}
	
	private async Task Recover()
	{
		_enemy?.QueueFree();
		_enemy = null;
		
		for (int i = 0; i < _party.Count; i++)
		{
			if (!_party[i].Visible) continue;
			_tween.InterpolateProperty(_party[i], "position", 
			_party[i].Position, _adventurerSlots[i].Position,
			_lungeTime, Tween.TransitionType.Sine, Tween.EaseType.Out);
		}
		
		_tween.Start();
		await ToSignal(_tween, "tween_all_completed");
	}
	
	private void SpawnParty(List<Adventurer> party)
	{
		for (int i = 0; i < party.Count && i < _adventurerSlots.Length; i++)
		{
			var doll = (SpriteDoll)_partyDoll.Instance();
			_partyRoot.AddChild(doll);
			doll.Position = _adventurerSlots[i].Position;
			DressDoll(doll, party[i]);
			_party.Add(doll);
		}
	}
	
	private void DressDoll(SpriteDoll doll, Adventurer adventurer)
	{
		foreach(KeyValuePair<ClothingSlot, ClothingData> kv in adventurer.EquippedItems)
			if(kv.Value != null)
				doll.Equip(kv.Value);
	}
	
	private void SetState(DungeonState next) {
		_state = next;
		bool walking = next == DungeonState.Walking;
		foreach (SpriteDoll doll in _party)
			if (doll.Visible)
				doll.SetWalking(walking);
				
	}
	
	private async Task Wait(float seconds)
		=> await ToSignal(GetTree().CreateTimer(seconds), "timeout");
}
