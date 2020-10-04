using Godot;
using System;

public class Explosion : Node2D
{
    public int ParticleCount = 4;
    public float Lifetime = 2f;
    public bool IsOneShot = true;
    public float Size = 12;

    // Called when the node enters the scene tree for the first time.
    public async override void _Ready()
    {
        //GetNode<Particles2D>("Particles2D").Amount = ParticleCount;
        //GetNode<Particles2D>("Particles2D").OneShot = IsOneShot;
        //GetNode<Particles2D>("Particles2D").ProcessMaterial.Set("emission_sphere_radius",Size);
        //GetNode<Timer>("Timer").WaitTime = Lifetime;
        //GetNode<Particles2D>("Particles2D").Emitting = true;
        //GetNode<Particles2D>("Particles2D").Explosiveness = IsOneShot ? 1 : 0;
        //GetNode<Timer>("Timer").Start();

        await ToSignal(GetTree(),"idle_frame");
        GD.Randomize();
        EmitParticles();
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

    public async void EmitParticles()
    {
        foreach (var praticle in GD.Range(ParticleCount))
        {
            AnimatedSprite p = (GetNode<AnimatedSprite>("Particle").Duplicate() as AnimatedSprite);

            p.Position = new Vector2(Size,Size).Rotated(Godot.Mathf.Deg2Rad((float)GD.RandRange(0,360)));
            p.Show();
            AddChild(p);
            p.Animation = "default";
            p.Playing = true;

            GD.Print("Poof");

            await ToSignal(GetTree().CreateTimer(Lifetime / (float)ParticleCount,false),"timeout");
        }

        await ToSignal(GetTree().CreateTimer(2f,false),"timeout");
        QueueFree();
    }

    private async void OnTimerTimeout()
    {
        if(!GetNode<Particles2D>("Particles2D").OneShot)
        {
            GetNode<Particles2D>("Particles2D").Emitting = false;
            await ToSignal(GetTree().CreateTimer(1f,false),"timeout");
        }
        
        QueueFree();
    }
}
