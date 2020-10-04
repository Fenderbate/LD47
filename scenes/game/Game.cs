using Godot;
using System;

public class Game : Node2D
{
    [Export]
    public Color goodColor = new Color("0f1018");

    [Export]
    public Color badColor = new Color("561200");

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<SignalManager>("/root/SignalManager").Connect(nameof(SignalManager.TargetAreaLeft),this,"OnTargetAreaLeft");
        GetNode<SignalManager>("/root/SignalManager").Connect(nameof(SignalManager.TargetAreaEntered),this,"OnTargetAreaEntered");
        GetNode<SignalManager>("/root/SignalManager").Connect(nameof(SignalManager.Retry),this,"OnRetry");

        GetNode<Global>("/root/Global").Reset();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

    private void OnTargetAreaLeft()
    {
        GetNode<Tween>("Tween").StopAll();
        GetNode<Tween>("Tween").InterpolateProperty(GetNode<Polygon2D>("Background"),"color",GetNode<Polygon2D>("Background").Color,badColor,3f);
        GetNode<Tween>("Tween").Start();
        //GetNode<Polygon2D>("Background").Color = badColor;
    }

    private void OnTargetAreaEntered()
    {
        GetNode<Tween>("Tween").StopAll();
        GetNode<Tween>("Tween").InterpolateProperty(GetNode<Polygon2D>("Background"),"color",GetNode<Polygon2D>("Background").Color,goodColor,0.2f);
        GetNode<Tween>("Tween").Start();
        //GetNode<Polygon2D>("Background").Color = goodColor;
    }

    private void OnRetry()
    {
        GD.Print("maybe do transition");
        GetTree().ReloadCurrentScene();
    }
}
