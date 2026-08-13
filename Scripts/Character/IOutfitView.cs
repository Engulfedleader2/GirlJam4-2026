using Godot;
using System;

public interface  IOutfitView
{
	void Equip(ClothingData item);
	void UnequipSlot(ClothingSlot slot);
}
