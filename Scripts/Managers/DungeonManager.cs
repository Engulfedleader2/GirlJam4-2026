using Godot;
using System;
using System.Collections.Generic;

public class DungeonManager : Node
{
	public static DungeonManager Instance { get; private set; }

	private const int MaxRounds = 100;
	private const int GoldPerFloor = 20;

	private readonly Random random = new Random();

	public DungeonData CurrentDungeon { get; private set; }
	public List<Adventurer> Party { get; private set; } = new List<Adventurer>();

	public int CurrentFloor { get; private set; }
	public int GoldEarned { get; private set; }
	public bool IsRunActive { get; private set; }


	private int _runGold;
	public int GoldEarned => _runGold;
	public override void _Ready()
	{
		Instance = this;
	}
	public Queue<DungeonEvent> BuildRun(List<Adventurer> party, int maxFloors = 2)
	{
		_runGold = 0;
		var tape = new Queue<DungeonEvent>();
		var rng = new System.Random();
		
		for (int floor = 1; floor <= maxFloors; floor++)
		{
			EnemyData data = EnemyLibrary.Instance.RandomForLevel(floor);
			if (data == null) continue;
			
			bool wiped = ResolveEncounter(party, data, rng);
			tape.Enqueue(MakeFightEvent(party, data, false));
			if (wiped) {tape.Enqueue(new DungeonEvent {Type = DungeonEventType.RunEnded}); return tape; }
		}
		
		tape.Enqueue(new DungeonEvent {Type = DungeonEventType.RunEnded});
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
		GoldEarned = 0;
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
		GameManager.Instance.AddTreasure(GoldPerFloor);
		GoldEarned += GoldPerFloor;
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
		
		foreach (Adventurer a in party)
		{
			if (a.IsAlive)
			{
				AdventurerManager.Instance.HandleReturningAdventurer(a);
				survivors ++;
			}
			else {
				AdventurerManager.Instance.HandleDeadAdventurer(a);
				fallen++;
			}
		}
		
		GameManager.Instance.AddTreasure(_runGold);
		return wiped
			? $"The party fell. Earned {_runGold}g."
			: $"Returned! {survivors} survived, {fallen} lost. Earned {_runGold}g.";
	}
	private bool HasSurvivors()
	{
		return Party.Exists(adventurer => adventurer.IsAlive);
	}
}
