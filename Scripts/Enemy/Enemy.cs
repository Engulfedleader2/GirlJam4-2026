// Enemy setup

public class Enemy
{
	public string Name;
	public int MaxHP;
	public int CurrentHP;
	public int Attack;

	public bool IsAlive => CurrentHP > 0;

	public void TakeDamage(int amount)
	{
		CurrentHP = System.Math.Max(CurrentHP - amount, 0);
	}
}
