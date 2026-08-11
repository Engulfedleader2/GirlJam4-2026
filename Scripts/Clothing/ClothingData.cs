using Godot;

// A single clothing item's data - no logic, just what it is and where it goes.

public enum ClothingSlot
{
	Body,
	Head,
	Legs,
	Feet,
	Hair,
	Accessory
}

public enum ClothingClass
{
	None,
	Tank,
	DPS,
	Healer
}

public class ClothingData : Resource
{
	[Export] public string Id;
	[Export] public string ItemName;
	[Export] public ClothingSlot Slot;
	[Export] public ClothingClass Class = ClothingClass.None;
	[Export] public Texture Sprite;
	[Export] public int StatBonus;
	[Export] public int Rarity;

	// Cost to unlock this item's design in the shop.
	[Export] public int Price = 15;
}
