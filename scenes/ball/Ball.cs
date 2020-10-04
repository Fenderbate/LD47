using Godot;
using System;

public class Ball : Node2D
{
    public enum BehaviourType
    {
        Normal,
        Pierceing,
        Boucy
    }
    
    [Export]
    public bool canMove = true;

    [Export]
    public float Speed = 300f;

    [Export]
    public BehaviourType Type = BehaviourType.Normal;

    public Vector2 Direction = Vector2.Zero;

    private bool bounce = false;


    

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Shoot(Direction);
        GD.Randomize();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if(!canMove)
        {
            return;
        }

        Position += Direction * Speed * delta;
        Rotation = Direction.Angle();


    }

    public void Shoot(Vector2 shotDirection)
    {
        Direction = shotDirection;
    }

    public void Redirect(Vector2 newDirection)
    {
        bounce = true;
        Direction = newDirection;
    }

    public void Hit()
    {
        GetNode<AudioStreamPlayer>("Hit").Play();
    }

    private void OnLifeTimerTimeout()
    {
        QueueFree();
    }

    private async void OnArea2DAreaEntered(Area2D area)
    {
        if(!bounce)
        {
            return;
        }
        
        if(area.IsInGroup("Enemy"))
        {
            GetNode<SignalManager>("/root/SignalManager").EmitSignal(nameof(SignalManager.BossHurt));
            if(Type is BehaviourType.Normal)
            {
                GetNode<SignalManager>("/root/SignalManager").EmitSignal(nameof(SignalManager.Shake));
                GetNode<Area2D>("Area2D").QueueFree();
                GetNode<Particles2D>("Particles2D").Emitting = false;
                GetNode<Sprite>("Sprite").Hide();
                GetNode<Global>("/root/Global").SpawnExplosion(GlobalPosition,1,2f,1);

                GetNode<AudioStreamPlayer>("Explode").Stream = GetNode<Global>("/root/Global").ExplosonSounds[(int)GD.RandRange(0,3)];
                GetNode<AudioStreamPlayer>("Explode").Play();

                await ToSignal(GetTree().CreateTimer(4f,false),"timeout");

                QueueFree();
            }
        }
    }
}
