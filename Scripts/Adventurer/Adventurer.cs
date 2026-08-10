public class Adventurer
{
	public string Name;
	public string SkinTone;
	public string Hair;
	public string Face;
	public string Dream;

	public ClothingClass Class;
	public int MaxHP;
	public int CurrentHP;
	public int Attack;

	public bool IsAlive => CurrentHP > 0;

	public void TakeDamage(int amount)
	{
		CurrentHP = System.Math.Max(CurrentHP - amount, 0);
	}
}
