using Godot;

// Drop target for equipping items onto the character preview.

public class ClothingDropZone : Control
{
	public override bool CanDropData(Vector2 position, object data)
	{
		return data is ClothingData;
	}

	public override void DropData(Vector2 position, object data)
	{
		if (data is ClothingData item)
		{
			OutfitManager.Instance.EquipItem(item);
		}
	}
}
