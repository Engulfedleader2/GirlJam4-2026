using Godot;
using System.Collections.Generic;

public class SpriteDoll : Node2D, IOutfitView
{
	private static readonly Dictionary<ClothingSlot, string> SlotNodes = 
		new Dictionary<ClothingSlot, string>
		{
			{ClothingSlot.Head, "HeadOutfit"},
			{ClothingSlot.Body, "BodyOutfit"},
			{ClothingSlot.Legs, "LegsOutfit"},
			{ClothingSlot.Feet, "FeetOutfit"},
		};
	
	public void Equip(ClothingData item)
	{
		if (item == null) return;
		SetClothing(item.Slot, item.PixelSprite);
	}
	
	public void UnequipSlot(ClothingSlot slot)
	{
		SetClothing(slot, null);
	}
	
	public void SetWalking(bool walking) {}
	
	private void SetClothing(ClothingSlot slot, Texture tex)
	{
		if(!SlotNodes.TryGetValue(slot, out string nodeName))
			return;
		var sprite = FindNode(nodeName, true, false) as Sprite;
		if(sprite != null)
			sprite.Texture = tex;
	}
}
