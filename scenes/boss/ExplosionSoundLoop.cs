using Godot;
using System;

public class ExplosionSoundLoop : AudioStreamPlayer
{
    public bool PlayLoop = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

    public void StartLoop()
    {
        PlayLoop = true;
        Play();
    }

    public void StopLoop()
    {
        PlayLoop = false;
        Stop();
    }

    public void PlayBigExplosion()
    {
        PlayLoop = false;
        Stop();
        Stream = GD.Load<AudioStreamSample>("res://audio/bigexplosion.wav");
        Play();
    }

    private void OnExplosionSoundLoopFinished()
    {
        if(PlayLoop)
        {
            Stream = GetNode<Global>("/root/Global").ExplosonSounds[(int)GD.RandRange(0,3)];
            Play();
        }
    }
}
