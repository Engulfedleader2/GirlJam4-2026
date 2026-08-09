using Godot;
using System.Linq;

// Forwards equip calls to the active doll and signals when the outfit changes.

public class OutfitManager : Node
{
	public static OutfitManager Instance { get; private set; }

	[Signal]
	public delegate void OutfitChanged();

	private ClothingLayerController activeDoll;

	public override void _Ready()
	{
		Instance = this;
	}

	public void SetActiveDoll(ClothingLayerController doll)
	{
		activeDoll = doll;
	}

	public void ClearActiveDoll()
	{
		activeDoll = null;
	}

	public void EquipItem(ClothingData item)
	{
		if (activeDoll == null || item == null)
		{
			return;
		}

		activeDoll.EquipItem(item);
		EmitSignal(nameof(OutfitChanged));
	}

	public void UnequipSlot(ClothingSlot slot)
	{
		if (activeDoll == null)
		{
			return;
		}

		activeDoll.UnequipSlot(slot);
		EmitSignal(nameof(OutfitChanged));
	}

	public int GetTotalStatBonus()
	{
		if (activeDoll == null)
		{
			return 0;
		}

		return activeDoll
			.GetAllEquipped()
			.Where(item => item != null)
			.Sum(item => item.StatBonus);
	}

	public ClothingClass GetResolvedClass()
	{
		if (activeDoll == null)
		{
			return ClothingClass.None;
		}

		return ClassResolver.Resolve(
			activeDoll.GetAllEquipped()
		);
	}
}
