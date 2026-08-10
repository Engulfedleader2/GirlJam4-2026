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
	public bool IsRunActive { get; private set; }

	public override void _Ready()
	{
		Instance = this;
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
		GameManager.Instance.AddTreasure(GoldPerFloor);
	}

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

	private bool HasSurvivors()
	{
		return Party.Exists(adventurer => adventurer.IsAlive);
	}
}
