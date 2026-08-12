using Godot;
using System;

public class DungeonView : Node2D
{
	[Export] private float _scrollSpeed = 40f;
	private ParallaxBackground _parallax;
	private enum DungeonState {Walking, Fighting, Stopped}
	private DungeonState _state = DungeonState.Walking;
	
	public override void _Ready()
	{
		_parallax = GetNode<ParallaxBackground>("Background");
		_state = DungeonState.Walking;
	}
	
	public override void _Process(float delta)
	{
		if(_state == DungeonState.Walking)
			_parallax.ScrollOffset += new Vector2(-_scrollSpeed * delta, 0);
	}
}
