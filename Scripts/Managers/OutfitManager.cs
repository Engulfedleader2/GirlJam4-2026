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


		Adventurer adventurer = AdventurerManager.Instance.Selected;
		
		
		foreach (Adventurer temp in AdventurerManager.Instance.Roster)
		{
			if(temp == adventurer) continue;
			if (temp.EquippedItems.TryGetValue(item.Slot, out ClothingData worn) && worn == item)
			{
				temp.EquippedItems.Remove(item.Slot);
				ApplyOutfitToAdventurer(temp);
			}
		}
		
		activeDoll.EquipItem(item);
		
		if (adventurer != null)
		{
			adventurer.EquippedItems[item.Slot] = item;
			ApplyOutfitToAdventurer(adventurer);
		}

		EmitSignal(nameof(OutfitChanged));
	}

	public void UnequipSlot(ClothingSlot slot)
	{
		if (activeDoll == null)
		{
			return;
		}

		activeDoll.UnequipSlot(slot);

		Adventurer adventurer = AdventurerManager.Instance.Selected;

		if (adventurer != null)
		{
			adventurer.EquippedItems.Remove(slot);
			ApplyOutfitToAdventurer(adventurer);
		}

		EmitSignal(nameof(OutfitChanged));
	}
	
	public void UnequipAll()
	{
		if (activeDoll == null)
		{
			return;
		}
		Adventurer adventurer = AdventurerManager.Instance.Selected;
		
		if(adventurer == null) return;
		
		foreach (ClothingSlot slot in System.Enum.GetValues(typeof(ClothingSlot)))
		{
			activeDoll.UnequipSlot(slot);
			adventurer.EquippedItems.Remove(slot);
		}
		ApplyOutfitToAdventurer(adventurer);
		EmitSignal(nameof(OutfitChanged));
	}

	// Resets the doll to show exactly what this adventurer has equipped -
	// call this whenever the selected adventurer changes.
	public void LoadAdventurerOutfit(Adventurer adventurer)
	{
		if (activeDoll == null)
		{
			return;
		}

		foreach (ClothingSlot slot in System.Enum.GetValues(typeof(ClothingSlot)))
		{
			if (adventurer.EquippedItems.TryGetValue(slot, out ClothingData item))
			{
				activeDoll.EquipItem(item);
			}
			else
			{
				activeDoll.UnequipSlot(slot);
			}
		}

		EmitSignal(nameof(OutfitChanged));
	}

	// Recomputes an adventurer's real stats from their base stats + equipped items.
	private void ApplyOutfitToAdventurer(Adventurer adventurer)
	{
		int bonus = adventurer.EquippedItems.Values
			.Where(item => item != null)
			.Sum(item => item.StatBonus);

		adventurer.Class = ClassResolver.Resolve(adventurer.EquippedItems.Values);
		adventurer.MaxHP = adventurer.BaseMaxHP + bonus;
		adventurer.CurrentHP = adventurer.MaxHP;
		adventurer.Attack = adventurer.BaseAttack + bonus;
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
