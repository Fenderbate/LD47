using Godot;
using System;

public class Global : Node
{
    public bool defeat = false;
    public bool victory = false;

    public AudioStreamSample [] ExplosonSounds;

    private PackedScene explosion;
    private PackedScene hit;

    public override void _Ready()
    {
        GD.Print("Global Ready!");
        explosion = GD.Load<PackedScene>("res://scenes/explosion/Explosion.tscn");
        hit = GD.Load<PackedScene>("res://scenes/hit/Hit.tscn");

        ExplosonSounds = new AudioStreamSample [] {GD.Load<AudioStreamSample>("res://audio/explosion1.wav"),GD.Load<AudioStreamSample>("res://audio/explosion2.wav"),GD.Load<AudioStreamSample>("res://audio/explosion3.wav")};
    }

    public void Reset()
    {
        defeat = false;
        victory = false;
    }

    public void Defeat()
    {
        defeat = true;
    }

    public void Victory()
    {
        victory = true;
    }

    public void SpawnExplosion(Vector2 position = new Vector2(), int particleCount = 4, float lifetime = 2f, float size = 12, bool oneShot = true)
    {
        GD.Print("Spawn explosion");

        var e = (explosion.Instance() as Explosion);
        
        e.GlobalPosition = position;
        e.ParticleCount = particleCount;
        e.Lifetime = lifetime;
        e.Size = size;
        e.IsOneShot = oneShot;
        
        GetTree().Root.AddChild(e);


    }

    public void SpwanHit(Vector2 position = new Vector2())
    {
        var h = (hit.Instance() as Hit);

        h.GlobalPosition = position;

        GetTree().Root.AddChild(h);
    }
}
