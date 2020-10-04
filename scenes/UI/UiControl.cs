using Godot;
using System;

public class UiControl : Control
{
    [Export]
    public String Menu;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

    private void OnPauseButtonDown()
    {
        GetTree().Paused = true;
        GetNode<ColorRect>("Cover").Show();
    }

    private void OnResumeButtonDown()
    {
        GetNode<ColorRect>("Cover").Hide();
        GetTree().Paused = false;
    }

    private void OnExitButtonDown()
    {
        GetTree().ChangeSceneTo(GD.Load<PackedScene>(Menu));
        GetTree().Paused = false;
    }
}
