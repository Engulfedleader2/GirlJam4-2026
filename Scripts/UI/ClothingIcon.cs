using Godot;

// A clickable/draggable icon for one clothing item.

public class ClothingIcon : TextureButton
{
	public ClothingData Item;

	public void SetItem(ClothingData item)
	{
		Item = item;

		if (item == null)
		{
			TextureNormal = null;
			return;
		}

		TextureNormal = item.Sprite;
	}

	public override object GetDragData(Vector2 position)
	{
		if (Item == null)
		{
			return null;
		}

		var preview = new TextureRect
		{
			Texture = Item.Sprite,
			RectMinSize = new Vector2(64, 64),
			Expand = true,
			StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered
		};

		SetDragPreview(preview);

		return Item;
	}
}
