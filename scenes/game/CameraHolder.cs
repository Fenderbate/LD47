using Godot;
using System;

public class CameraHolder : Node2D
{
    private Camera2D camera;

    int Shakes = 0;
    float Intensity = 1;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<SignalManager>("/root/SignalManager").Connect(nameof(SignalManager.Shake),this,"Shake");
        GetNode<SignalManager>("/root/SignalManager").Connect(nameof(SignalManager.BigShake),this,"BigShake");
        
        camera = GetNode<Camera2D>("Camera2D");
    }

    

    public void AddShake(int shakes = 5)
    {
        Shakes += shakes;
    }

    public void AddBigShake(int shakes = 200, float intensity = 3)
    {
        Shakes += shakes;
        Intensity = intensity;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if(Shakes > 0)
        {
            camera.Position = new Vector2(Intensity,Intensity).Rotated(Godot.Mathf.Deg2Rad((float)GD.RandRange(0,360)));
            if(Intensity > 2)
            {
                Intensity -= delta * 3;
            }
            Shakes--;
        }
        else
        {
            camera.Position = Vector2.Zero;
            Intensity = 2;
        }
    }

    public void Shake()
    {
        AddShake();
    }

    public void BigShake()
    {
        AddBigShake();
    }
}
