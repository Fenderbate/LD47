using Godot;
using System;

public class EndGameScreen : Node2D
{
    [Export]
    public Color WinColor;

    [Export]
    public Color LoseColor;

    private float radius = 0f;

    private bool victory = false;

    private bool defeat = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<SignalManager>("/root/SignalManager").Connect(nameof(SignalManager.Victory),this,"OnVictory");
        GetNode<SignalManager>("/root/SignalManager").Connect(nameof(SignalManager.Defeat),this,"OnDefeat");
    }

    public override void _Draw()
    {
        base._Draw();

        if(victory)
        {
            DrawCircle(Vector2.Zero,radius,WinColor);
        }
        else if(defeat)
        {
            DrawCircle(Vector2.Zero,radius,LoseColor);
        }

    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Update();
    }
         
    private void OnRetryButtonDown()
    {
        if(GetNode<Sprite>("LoseSprite").Modulate.a > 0)
        {
            GetNode<SignalManager>("/root/SignalManager").EmitSignal(nameof(SignalManager.Retry));
        }
    }

    private void OnVictory()
    {
        victory = true;
        GetNode<Tween>("Tween").InterpolateProperty(this,"radius",radius,300,4f,Tween.TransitionType.Expo,Tween.EaseType.Out);
        GetNode<Tween>("Tween").InterpolateProperty(GetNode<Sprite>("WinSprite"),"self_modulate",new Color(1,1,1,0),new Color(1,1,1,1),1f,Tween.TransitionType.Expo,Tween.EaseType.Out,0.65f);
        GetNode<Tween>("Tween").InterpolateProperty(GetNode<Sprite>("WinSprite"),"position",new Vector2(0,-10),new Vector2(0,0),1f,Tween.TransitionType.Expo,Tween.EaseType.Out,0.65f);
        GetNode<Tween>("Tween").Start();
    }

    private void OnDefeat()
    {
        defeat = true;
        GetNode<Tween>("Tween").InterpolateProperty(this,"radius",radius,300,4f,Tween.TransitionType.Expo,Tween.EaseType.Out);
        GetNode<Tween>("Tween").InterpolateProperty(GetNode<Sprite>("LoseSprite"),"modulate",new Color(1,1,1,0),new Color(1,1,1,1),1f,Tween.TransitionType.Expo,Tween.EaseType.Out,0.65f);
        GetNode<Tween>("Tween").InterpolateProperty(GetNode<Sprite>("LoseSprite"),"position",new Vector2(0,-10),new Vector2(0,0),1f,Tween.TransitionType.Expo,Tween.EaseType.Out,0.65f);
        GetNode<Tween>("Tween").Start();
    }
}
