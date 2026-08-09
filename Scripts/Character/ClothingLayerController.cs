using Godot;
using System.Collections.Generic;

// Swaps textures on a fixed set of Sprite layers, one per slot.

public class ClothingLayerController : Node2D
{
	private readonly Dictionary<ClothingSlot, Sprite> slotSprites = new Dictionary<ClothingSlot, Sprite>();
	private readonly Dictionary<ClothingSlot, ClothingData> equippedItems = new Dictionary<ClothingSlot, ClothingData>();

	public override void _Ready()
	{
		// Matches the child node names in PaperDoll.tscn.
		slotSprites[ClothingSlot.Body] = GetNode<Sprite>("Body");
		slotSprites[ClothingSlot.Head] = GetNode<Sprite>("Head");
		slotSprites[ClothingSlot.Legs] = GetNode<Sprite>("Legs");
		slotSprites[ClothingSlot.Feet] = GetNode<Sprite>("Feet");
		slotSprites[ClothingSlot.Hair] = GetNode<Sprite>("Hair");
		slotSprites[ClothingSlot.Accessory] = GetNode<Sprite>("Accessory");
	}

	public void EquipItem(ClothingData item)
	{
		if (item == null || item.Sprite == null)
		{
			return;
		}

		slotSprites[item.Slot].Texture = item.Sprite;
		equippedItems[item.Slot] = item;
	}

	public void UnequipSlot(ClothingSlot slot)
	{
		slotSprites[slot].Texture = null;
		equippedItems.Remove(slot);
	}

	public ClothingData GetEquipped(ClothingSlot slot)
	{
		equippedItems.TryGetValue(slot, out ClothingData item);
		return item;
	}

	public IEnumerable<ClothingData> GetAllEquipped()
	{
		return equippedItems.Values;
	}
}
