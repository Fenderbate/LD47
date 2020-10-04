using Godot;
using System;

public class SignalManager : Node
{
    [Signal]
    public delegate void Defeat();

    [Signal]
    public delegate void Victory();

    [Signal]
    public delegate void Retry();

    [Signal]
    public delegate void TargetAreaLeft();

    [Signal]
    public delegate void TargetAreaEntered();

    [Signal]
    public delegate void BossHurt();

    [Signal]
    public delegate void Shake();

    [Signal]
    public delegate void BigShake();

    [Signal]
    public delegate void ShakeDone();

    
}
