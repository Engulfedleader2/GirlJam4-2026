using Godot;
using System;

public class DungeonViewTest : Control
{
	
	private Viewport _viewport;
	private DungeonView _dungeonView;
	private TextureRect _display;
	
	public override void _Ready()
	{
		_viewport = GetNode<Viewport>("Viewport");
		_dungeonView = GetNode<DungeonView>("Viewport/DungeonView");
		_display = GetNode<TextureRect>("TextureRect");
		
		ConfigureViewport();
		ConfigureDisplay();
	}
	
	private void ConfigureViewport()
	{
		_viewport.RenderTargetVFlip = true;
		_viewport.RenderTargetUpdateMode = Viewport.UpdateMode.Always;
	}
	
	private void ConfigureDisplay()
	{

		ViewportTexture tex = _viewport.GetTexture();
		tex.Flags = 0; 
		_display.Texture = tex;
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
