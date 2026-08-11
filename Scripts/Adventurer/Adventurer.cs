using System.Collections.Generic;

public class Adventurer
{
	public string Name;
	public string SkinTone;
	public string Hair;
	public string Face;
	public string Dream;

	// Stats before any outfit is applied
	public int BaseMaxHP;
	public int BaseAttack;

	// What they're currently wearing
	public Dictionary<ClothingSlot, ClothingData> EquippedItems = new Dictionary<ClothingSlot, ClothingData>();

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
