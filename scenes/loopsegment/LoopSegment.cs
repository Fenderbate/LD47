using Godot;
using System;

public class LoopSegment : Node2D
{
    [Export]
    public NodePath paddleHolderPath;
    private Node2D paddleHolder;

    [Export(PropertyHint.Range,"0,360")]
    public float AngleLimitRotation
    {
        set
        {
            if(value > 360)
            {
                _angleLimitRotation = value - 360;
            }
            else if(value < 0)
            {
                _angleLimitRotation = value + 360;
            }
            else
            {
                _angleLimitRotation = value;
            }
        }
        get
        {
            return _angleLimitRotation;
        }
    }
    private float _angleLimitRotation = 0;

    [Export(PropertyHint.Range,"20,180")]
    public float AngleLimitSize
    {
        set
        {
            if(value >= 20 && value <= 180)
            {
                _angleLimitSize = value;
            }
        }
        get
        {
            return _angleLimitSize;
        }
    }
    private float _angleLimitSize = 20;

    [Export]
    public Color AngleTargetColor;

    [Export]
    public Color AngleNoZoneColor;

    public float DistanceFromCenter = 120;

    private Global global;
    private SignalManager signalManager;

    private float deltasum = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        paddleHolder = GetNode<Node2D>(paddleHolderPath);
        signalManager = GetNode<SignalManager>("/root/SignalManager");
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Update();

        //AngleLimitRotation += ((Input.IsActionPressed("right") ? 150 : 0) - (Input.IsActionPressed("left") ? 150 : 0)) * delta;
        //AngleLimitSize += ((Input.IsActionPressed("up") ? 150 : 0) - (Input.IsActionPressed("down") ? 150 : 0)) * delta;

        //GD.Print(AngleLimitRotation + " - " + (CheckAngleLimit(paddleHolder.RotationDegrees, AngleLimitRotation - AngleLimitSize, AngleLimitRotation + AngleLimitSize) ? "Yep" : "Nope"));

        AngleLimitRotation += 10 * delta;

        deltasum += delta * 2;

        AngleLimitSize = 30 + (10 * Mathf.Sin(deltasum));

        if(!CheckAngleLimit(paddleHolder.RotationDegrees, AngleLimitRotation - AngleLimitSize, AngleLimitRotation + AngleLimitSize))
        {
            if(GetNode<Timer>("Timer").IsStopped())
            {
                GetNode<Timer>("Timer").Start();
                signalManager.EmitSignal(nameof(SignalManager.TargetAreaLeft));
            }
        }
        else
        {
            if(!GetNode<Timer>("Timer").IsStopped())
            {
                GetNode<Timer>("Timer").Stop();
                signalManager.EmitSignal(nameof(SignalManager.TargetAreaEntered));
            }
        }

    }

    public override void _Draw()
    {

        var steps = 360;

        foreach (var i in GD.Range(steps))
        {
            var currentColor = CheckAngleLimit(i,AngleLimitRotation - AngleLimitSize, AngleLimitRotation + AngleLimitSize) ? AngleTargetColor : AngleNoZoneColor;

            DrawLine(new Vector2(DistanceFromCenter,0).Rotated(Mathf.Deg2Rad(i * (360 / steps))),new Vector2(DistanceFromCenter,0).Rotated(Mathf.Deg2Rad((i + 1)* (360 / steps))),currentColor,4);
        }

    }

    public bool CheckAngleLimit(float angle, float minAngle, float maxAngle)
    {
        var result= (angle > minAngle && angle < maxAngle) ||
                    (angle > (minAngle) - 360f && angle <(maxAngle) - 360f) ||
                    (angle > (minAngle) + 360f && angle <(maxAngle) + 360f)
                    ? true : false;
        
        return result;
    }



    private void OnTimerTimeout()
    {
        if(!GetNode<Global>("/root/Global").defeat && !GetNode<Global>("/root/Global").victory)
        {
            GetNode<Global>("/root/Global").Defeat();
            signalManager.EmitSignal(nameof(SignalManager.Defeat));
        }
    }
}
