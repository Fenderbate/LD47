using Godot;
using System;

public class Paddle : Node2D
{
    [Export]
    public float MovementSensitivity = 0.1f;

    [Export]
    public float MinMovementSpeed = 0.05f;

    [Export]
    public float DistanceFromCenter = 240f;

    [Export]
    public float MaxPaddleAngle = 30f;

    [Export]
    public Color CircleColor;

    private Node2D mouseFollower;
    private Node2D paddleHolder;
    private Area2D paddleArea;
    private SignalManager signalManager;

    private bool move = true;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        signalManager = GetNode<SignalManager>("/root/SignalManager");
        signalManager.Connect(nameof(SignalManager.Defeat),this,"OnDefeat");
        
        mouseFollower = GetNode<Node2D>("MouseFollower");
        paddleHolder = GetNode<Node2D>("PaddleHolder");
        paddleArea = paddleHolder.GetNode<Area2D>("PaddleArea");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        mouseFollower.LookAt(GetGlobalMousePosition());

        GetNode<Sprite>("PaddleHolder/PaddleArea/Position2D/PlayerSprite").LookAt(GlobalPosition);
        
        paddleArea.LookAt(GetGlobalMousePosition());

        //paddleArea.Rotation = (GetGlobalMousePosition() - paddleArea.GlobalPosition).Normalized().Angle();

        //Mathf.Clamp(paddleArea.Rotation,-MaxPaddleAngle,MaxPaddleAngle);

        if(move)
        {
            var direction = (mouseFollower.Rotation - paddleHolder.Rotation) * MovementSensitivity;

            paddleHolder.Rotation += direction ;//> MinMovementSpeed ? direction : MinMovementSpeed;

            if(paddleHolder.RotationDegrees > 360)
            {
                paddleHolder.RotationDegrees -= 360;
                mouseFollower.RotationDegrees -= 360;
            }
            else if (paddleHolder.RotationDegrees < 0)
            {
                paddleHolder.RotationDegrees += 360;
                mouseFollower.RotationDegrees += 360;
            }
        }
    }

    public override void _Input(InputEvent input)
    {
        if(input.IsActionPressed("left_click"))
        {
            move = false;
        }
        else if(input.IsActionReleased("left_click"))
        {
            move = true;
        }
    }

    private void OnPaddleAreaEntered(Area2D area)
    {
        if(area.IsInGroup("Ball"))
        {
            (area.GetParent() as Ball).Redirect(new Vector2(Godot.Mathf.Cos(paddleArea.GlobalRotation),Godot.Mathf.Sin(paddleArea.GlobalRotation)));
            (area.GetParent() as Ball).Hit();
            GetNode<Global>("/root/Global").SpwanHit(area.GlobalPosition);

            
        }
        else if(area.IsInGroup("Enemy"))
        {
            if(!GetNode<Global>("/root/Global").defeat && !GetNode<Global>("/root/Global").victory)
            {
                GetNode<Global>("/root/Global").Defeat();
                signalManager.EmitSignal(nameof(SignalManager.Defeat));
            }
        }
    }

    private void OnDefeat()
    {
        GD.Print("------------------------Defeat!------------------------");
    }
}
