using Godot;
using System;
using System.Collections.Generic;

public class DungeonManager : Node
{
	private const string ConfigPath = "res://Resources/Config/GameConfig.cfg";
	private const int MaxRounds = 100;

	public static DungeonManager Instance { get; private set; }

	private int goldPerFloor;

	private readonly Random random = new Random();

	public DungeonData CurrentDungeon { get; private set; }
	public List<Adventurer> Party { get; private set; } = new List<Adventurer>();

	public int CurrentFloor { get; private set; }
	public bool IsRunActive { get; private set; }

	
	private int _runGold;
	public int GoldEarned => _runGold;
	public RunResult LastRun { get; private set; }
	
	private const int MaxFloors = 10;
	public bool PartyWiped {get; private set;}
	public bool HasNextFloor => CurrentFloor < MaxFloors;
	public override void _Ready()
	{
		Instance = this;

		var config = new ConfigFile();
		config.Load(ConfigPath);
		goldPerFloor = (int)config.GetValue("Dungeon", "gold_per_floor", 20);
	}
	
	public void BeginRun()
	{
		_runGold = 0;
		CurrentFloor = 1;
		PartyWiped = false;
	}
	
	public void AdvanceToNextFloor() => CurrentFloor++;
	
	public Queue<DungeonEvent> BuildCurrentFloor(List<Adventurer> party)
	{
		var tape = new Queue<DungeonEvent>();
		var rng = new System.Random();
		
		tape.Enqueue(new DungeonEvent {Type = DungeonEventType.EnterFloor});
		
		int commons = rng.Next(2,5);
		for (int i = 0; i < commons; i++) 
		{
			EnemyData data = EnemyLibrary.Instance.RandomForLevel(CurrentFloor);
			if (data == null) continue;
			if  (ResolveEncounter(party, data, rng))
			{
				return Wipe(tape, party, data, false);
			}
			tape.Enqueue(MakeFightEvent(party, data, false));
		}
		
		EnemyData boss = EnemyLibrary.Instance.BossForLevel(CurrentFloor);
		if (boss != null)
		{
			if(ResolveEncounter(party, boss, rng)) {
				return Wipe(tape, party, boss, true);
			}
			tape.Enqueue(MakeFightEvent(party, boss, true));
		}
		
		tape.Enqueue(new DungeonEvent {Type = DungeonEventType.FloorCleared});
		return tape;
	}
	
	private Queue<DungeonEvent> Wipe(Queue<DungeonEvent> tape, List<Adventurer> party, EnemyData data, bool boss)
	{
		PartyWiped = true;
		tape.Enqueue(MakeFightEvent(party, data, boss));
		tape.Enqueue(new DungeonEvent { Type = DungeonEventType.RunEnded});
		return tape;
	}
	private bool ResolveEncounter(List<Adventurer> party, EnemyData data, System.Random rng)
	{
		var enemy = new Enemy
		{
			Name = data.EnemyName,
			MaxHP = data.MaxHP,
			CurrentHP = data.MaxHP,
			Attack = System.Math.Max(data.Attack, 1)
		};
		
		int rounds = 0;
		while (enemy.IsAlive && party.Exists(a => a.IsAlive) && rounds < 100)
		{
			foreach (Adventurer a in party)
				if (a.IsAlive) enemy.TakeDamage(a.Attack);
			var survivors = party.FindAll(a => a.IsAlive);
			if (enemy.IsAlive && survivors.Count > 0)
				survivors[rng.Next(survivors.Count)].TakeDamage(enemy.Attack);
			
			rounds++;
		}
		if(!enemy.IsAlive) 
			_runGold += data.Gold;
		return !party.Exists(a => a.IsAlive);
	}
	
	private DungeonEvent MakeFightEvent(List<Adventurer> party, EnemyData data, bool boss)
	{
		int n = party.Count;
		int[] hp = new int[n];
		bool[] dead = new bool[n];
		for (int i = 0; i < n; i++)
		{
			hp[i] = party[i].CurrentHP;
			dead[i] = !party[i].IsAlive;
		}
		return new DungeonEvent
		{
			Type = DungeonEventType.Fight,
			EnemyName = data.EnemyName,
			EnemySprite = data.Sprite,
			IsBoss = boss,
			PartyHp = hp,
			PartyDead = dead,
		};
	}
	public void StartRun(DungeonData dungeon, List<Adventurer> party)
	{
		CurrentDungeon = dungeon;
		Party = new List<Adventurer>(party);

		CurrentFloor = 1;
		IsRunActive = true;
	}

	public void AdvanceFloor()
	{
		if (!IsRunActive)
		{
			return;
		}

		RunFight();

		if (!HasSurvivors())
		{
			EndRun();
			return;
		}

		GiveFloorReward();

		CurrentFloor++;

		if (CurrentFloor > CurrentDungeon.Floors)
		{
			EndRun();
		}
	}

	private void RunFight()
	{
		var enemy = new Enemy
		{
			Name = CurrentDungeon.Enemy.EnemyName,
			MaxHP = CurrentDungeon.Enemy.MaxHP,
			CurrentHP = CurrentDungeon.Enemy.MaxHP,
			Attack = Math.Max(CurrentDungeon.Enemy.Attack, 1)
		};

		int rounds = 0;

		while (enemy.IsAlive && HasSurvivors() && rounds < MaxRounds)
		{
			foreach (Adventurer adventurer in Party)
			{
				if (adventurer.IsAlive)
				{
					enemy.TakeDamage(adventurer.Attack);
				}
			}

			List<Adventurer> survivors =
				Party.FindAll(adventurer => adventurer.IsAlive);

			if (enemy.IsAlive && survivors.Count > 0)
			{
				Adventurer target =
					survivors[random.Next(survivors.Count)];

				target.TakeDamage(enemy.Attack);
			}

			rounds++;
		}
	}

	private void GiveFloorReward()
	{
		GameManager.Instance.AddTreasure(goldPerFloor);
	}
	/*
	public void EndRun()
	{
		IsRunActive = false;

		foreach (Adventurer adventurer in Party)
		{
			if (adventurer.IsAlive)
			{
				AdventurerManager.Instance
					.HandleReturningAdventurer(adventurer);
			}
			else
			{
				AdventurerManager.Instance
					.HandleDeadAdventurer(adventurer);
			}
		}
	}
	*/
	public string EndRun()
	{
		var party = new List<Adventurer>(AdventurerManager.Instance.ActiveParty);
		bool wiped = !party.Exists(a => a.IsAlive);
		int survivors = 0, fallen = 0;
		
		var result = new RunResult
		{
			FloorReached = CurrentFloor,
			GoldEarned = _runGold,
			Wiped = wiped,
		};
		
		foreach (Adventurer a in party)
		{
			
			string name = string.IsNullOrEmpty(a.Name) ? "Adventurer" : a.Name;
			if (a.IsAlive)
			{
				result.AdventurerOutcomes.Add($"{name} returned.");
				AdventurerManager.Instance.HandleReturningAdventurer(a);
				survivors ++;
			}
			else {
				result.AdventurerOutcomes.Add($"{name} did not return.");
				AdventurerManager.Instance.HandleDeadAdventurer(a);
				fallen++;
			}
		}
		
		GameManager.Instance.AddTreasure(_runGold);
		LastRun = result;
		return wiped
			? $"The party fell. Earned {_runGold}g."
			: $"Returned! {survivors} survived, {fallen} lost. Earned {_runGold}g.";
	}
	private bool HasSurvivors()
	{
		return Party.Exists(adventurer => adventurer.IsAlive);
	}
}
