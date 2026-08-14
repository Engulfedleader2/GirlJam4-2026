using Godot;
using System.Collections.Generic;

public class EnemyLibrary : Node
{
	public static EnemyLibrary Instance { get; private set; }
	private const string EnemyDir = "res://Resources/Enemies";
		
	private const string ConfigPath = "res://Resources/Config/GameConfig.cfg";
	private const string EnemyConfigSection = "Enemies";

	private readonly List<EnemyData> _enemies = new List<EnemyData>();
	private readonly System.Random _rng = new System.Random();
	
	public override void _Ready()
	{
		Instance = this;
		LoadFolder();
	}
	
	private void LoadFolder()
	{
		var config = new ConfigFile();
		config.Load(ConfigPath);

		var dir = new Directory();
		if (dir.Open(EnemyDir) != Error.Ok) return;
		
		dir.ListDirBegin(true, true);
		string file = dir.GetNext();
		while (!string.IsNullOrEmpty(file))
		{
			if (file.EndsWith(".tres"))
			{
				EnemyData enemy = GD.Load<EnemyData>($"{EnemyDir}/{file}");
				ApplyConfigOverrides(enemy, System.IO.Path.GetFileNameWithoutExtension(file), config);
				_enemies.Add(enemy);
			}
			file = dir.GetNext();
		}
		GD.Print($"[EnemyLibrary] Loaded {_enemies.Count} enemies");
	}

	// Lets GameConfig.cfg tas the final say of each value
	private void ApplyConfigOverrides(EnemyData enemy, string key, ConfigFile config)
	{
		if (config.HasSectionKey(EnemyConfigSection, $"{key}_HP"))
		{
			enemy.MaxHP = (int)config.GetValue(EnemyConfigSection, $"{key}_HP");
		}

		if (config.HasSectionKey(EnemyConfigSection, $"{key}_Attack"))
		{
			enemy.Attack = (int)config.GetValue(EnemyConfigSection, $"{key}_Attack");
		}

		if (config.HasSectionKey(EnemyConfigSection, $"{key}_Gold"))
		{
			enemy.Gold = (int)config.GetValue(EnemyConfigSection, $"{key}_Gold");
		}
	}
	
	public EnemyData RandomForLevel(int level, bool boss = false)
	{
		var pool = _enemies.FindAll(e=> e.Level == level && e.IsBoss == boss);
		return pool.Count == 0 ? null : pool[_rng.Next(pool.Count)];
	}
	
	public EnemyData BossForLevel(int level) => RandomForLevel(level, true);
}
