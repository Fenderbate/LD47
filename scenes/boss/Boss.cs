using Godot;
using System;
using Godot.Collections;
using System.Reflection;

public class Boss : Node2D
{
    private class Pattern
    {
        public string name;
        public object [] methodParams;

        public Pattern(string _name, object [] _methodParams)
        {
            name  = _name;
            methodParams = _methodParams;
        }
    }

    private enum Stage
    {
        Easy,
        Medium,
        Hard
    }
    private Stage currentStage = Stage.Easy;
    private Stage nextStage = Stage.Easy;


    [Export]
    public PackedScene ball;

    [Export]
    public int health = 30;

    [Export]
    public NodePath paddleArea;

    private Timer attackTimer;

    private Godot.Collections.Array shootSoundPlayers;
    private int PlayerIndex
    {
        set
        {
            if(value > shootSoundPlayers.Count - 1)
            {
                _soundplayerIndex = 0;
            }
            else
            {
                _soundplayerIndex = value;
            }
        }
        get
        {
            return _soundplayerIndex;
        }
    }
    private int _soundplayerIndex = 0;

    private int AttackIndex
    {
        set
        {
            if(value < 0)
            {
                index = 0;
            }
            else if(value >= currentAttackPattern.Length)
            {
                GD.Print("rached pattern end!");
                index = 0;
            }
            else
            {
                index = value;
            }
        }
        get
        {
            return index;
        }
    }
    private int index = 0;

    private object [] currentAttackPattern;

    public object [] easyAttackPattern = new object [] {
            new Pattern(nameof(Boss.SprayBallsPattern), new object [] {Vector2.Right,4,1f,1f}),
            new Pattern(nameof(Boss.CircleSprayBalls), new object [] {Vector2.Down,10, 1,1f}),
            new Pattern(nameof(Boss.SprayBallsPattern), new object [] {Vector2.Left,4,1f,1f}),
            new Pattern(nameof(Boss.Charge), new object [] {Vector2.Zero,1f, 90f, 1, 1f}),
            
            //new Pattern(nameof(Boss.SprayBalls), new object [] {Vector2.Right,3,0.4f}),
            //new Pattern(nameof(Boss.SprayBalls), new object [] {Vector2.Down,3,0.4f}),
            //new Pattern(nameof(Boss.SprayBalls), new object [] {Vector2.Left,3,0.4f}),
            //new Pattern(nameof(Boss.SprayBalls), new object [] {Vector2.Up,3,1f})
            
    };
    
    public object [] mediumAttackPattern = new object [] {
            new Pattern(nameof(Boss.Charge), new object [] {Vector2.Zero,3f, 180f, 1, 0.5f}),
            new Pattern(nameof(Boss.Charge), new object [] {Vector2.Zero,3f, 180f, -1, 0.5f}),
            new Pattern(nameof(Boss.CircleSprayBalls), new object [] {Vector2.Down, 10, 1, 0.01f}),
            new Pattern(nameof(Boss.CircleSprayBalls), new object [] {Vector2.Up, 10, -1, 0.5f}),
            new Pattern(nameof(Boss.SprayBallsPattern), new object [] {Vector2.Left,3,0.5f,0.5f}),
            new Pattern(nameof(Boss.SprayBallsPattern), new object [] {new Vector2(1,1).Normalized(),3,0.5f,0.5f}),

    };

    public object [] hardAttackPattern = new object [] {
            new Pattern(nameof(Boss.CircleSprayBalls), new object [] {Vector2.Down, 10, 1, 0.01f}),
            new Pattern(nameof(Boss.CircleSprayBalls), new object [] {Vector2.Right, 10, 1, 0.01f}),
            new Pattern(nameof(Boss.CircleSprayBalls), new object [] {Vector2.Up, 10, 1, 0.01f}),
            new Pattern(nameof(Boss.CircleSprayBalls), new object [] {Vector2.Left, 10, 1, 1f}),
            new Pattern(nameof(Boss.Charge), new object [] {Vector2.Zero,6f, 270f, 1, 0.25f}),
            new Pattern(nameof(Boss.Charge), new object [] {Vector2.Zero,6f, 270f, -1, 0.25f}),
            new Pattern(nameof(Boss.SprayBallsPattern), new object [] {Vector2.Down,5,0.01f,0.25f}),
            new Pattern(nameof(Boss.SprayBallsPattern), new object [] {Vector2.Up,5,0.01f,1f}),
            new Pattern(nameof(Boss.Charge), new object [] {Vector2.Zero,6f, 360f, 1, 1f}),



    };
    
    public async override void _Ready()
    {
        currentAttackPattern = easyAttackPattern;
        
        GD.Randomize();
        GetNode<SignalManager>("/root/SignalManager").Connect(nameof(SignalManager.BossHurt),this,"OnBossHurt");
        shootSoundPlayers = GetNode<Node>("ShootPlayers").GetChildren();

        await ToSignal(GetTree(), "idle_frame");
        attackTimer = GetNode<Timer>("AttackTimer");
        attackTimer.Start();

        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        
    }

    public void ShootBall(Vector2 shootDirection = new Vector2(), float ballSpeed = 100)
    {
        Ball b = (ball.Instance() as Ball);
        b.Position = shootDirection * 30;
        b.Direction = shootDirection;
        b.Speed = ballSpeed;

        GetParent().AddChild(b);
    }

    public void SprayBalls(Vector2 direction, int ballcount = 3, float nextAtackDelay = 1f)
    {
        var spread = 30;
        
        foreach (float i in GD.Range(ballcount))
        {
            ShootBall(direction.Rotated(Godot.Mathf.Deg2Rad((float)(spread / ballcount) * (-1f + i))));
        }
        PlayShootSound();
        //attackTimer.WaitTime = nextAtackDelay;
        //attackTimer.Start();
    }

    public async void SprayBallsPattern(Vector2 direction = new Vector2() ,int sprayCount = 4, float sprayDelay = 1f, float nextAtackDelay = 1f )
    {
        float angle = Godot.Mathf.Deg2Rad(360f / (float)sprayCount);
        
        foreach (var i in GD.Range(sprayCount))
        {
            SprayBalls(direction.Rotated(angle * i), 3, sprayDelay);
            await ToSignal(GetTree().CreateTimer(sprayDelay,false),"timeout");
        }

        attackTimer.WaitTime = nextAtackDelay;
        attackTimer.Start();
    }

    public async void CircleSprayBalls(Vector2 startDirection, int ballcount = 36, int direction = 1, float nextAtackDelay = 1f)
    {
        foreach (var i in GD.Range(ballcount))
        {
            ShootBall(startDirection.Rotated(Godot.Mathf.Deg2Rad((360 / ballcount) * i) * direction));
            PlayShootSound();
            await ToSignal(GetTree().CreateTimer(0.1f,false),"timeout");
        }
        attackTimer.WaitTime = nextAtackDelay;
        attackTimer.Start();
    }

    public async void Charge(Vector2 startNormalVector, float speed, float distance, int direction, float nextAtackDelay)
    {
        
        Vector2 posNormal = (GetNode<Area2D>(paddleArea).GlobalPosition - GlobalPosition).Normalized().Rotated(Godot.Mathf.Deg2Rad((distance / 2) * -direction));


        GetNode<Sprite>("DirArrow").Show();
        GetNode<Sprite>("DirArrow").Rotation = posNormal.Angle();
        
        
        GetNode<Tween>("Tween").InterpolateProperty(this,"position",Position,Position + posNormal * 120,1f,Tween.TransitionType.Expo,Tween.EaseType.InOut,0.5f);
        GetNode<Tween>("Tween").Start();
        
        GetNode<AudioStreamPlayer>("Fly").Play();
        
        await ToSignal(GetNode<Tween>("Tween"),"tween_all_completed");

        GetNode<Sprite>("DirArrow").Rotation = direction < 0 ? posNormal.Perpendicular().Angle() : posNormal.Perpendicular().Rotated(Godot.Mathf.Deg2Rad(180)).Angle();
        await ToSignal(GetTree().CreateTimer(0.5f,false),"timeout");

        int steps = 0;

        while(steps < distance * 2)
        {
            posNormal = posNormal.Rotated(Godot.Mathf.Deg2Rad(speed / 2 * direction));
            Position = posNormal * 120;
            steps += (int)speed;

            GetNode<Sprite>("DirArrow").Rotation = direction < 0 ? posNormal.Perpendicular().Angle() : posNormal.Perpendicular().Rotated(Godot.Mathf.Deg2Rad(180)).Angle();

            await ToSignal(GetTree().CreateTimer(GetPhysicsProcessDeltaTime() / 2,false),"timeout");
        }

        

        GetNode<Sprite>("DirArrow").Rotation = (Vector2.Zero - Position).Normalized().Angle();

        GetNode<Tween>("Tween").InterpolateProperty(this,"position",Position,Vector2.Zero,1f,Tween.TransitionType.Expo,Tween.EaseType.InOut,0.5f);
        GetNode<Tween>("Tween").Start();
        await ToSignal(GetNode<Tween>("Tween"),"tween_all_completed");

        GetNode<AudioStreamPlayer>("Fly").Stop();
        GetNode<Sprite>("DirArrow").Hide();

        attackTimer.WaitTime = nextAtackDelay;
        attackTimer.Start();
        
    }

    public void PlayShootSound()
    {
        (shootSoundPlayers[PlayerIndex] as AudioStreamPlayer).Play();
        PlayerIndex++;
    }

    private void OnAttackTimerTimeout()
    {
        if(health <= 0)
        {
            return;
        }
        //SprayBalls(Vector2.Right);
        //SprayBalls(Vector2.Down);
        //SprayBalls(Vector2.Left);
        //SprayBalls(Vector2.Up);

        //CircleSprayBalls(Vector2.Up, 36);

        //GD.Print("ee - ",this.Call(nameof(this.ShootBall).ToString()));
        //GD.Print("attack result:  " + Call((attackPattern[AttackIndex] as Pattern).name, (attackPattern[AttackIndex] as Pattern).methodParams));

        if(currentStage != nextStage)
        {
            currentStage = nextStage;
            AttackIndex = 0;
            if(currentStage == Stage.Medium)
            {
                currentAttackPattern = mediumAttackPattern;
            }
            else if(currentStage == Stage.Hard)
            {
                currentAttackPattern = hardAttackPattern;
            }
            GD.Print("Stage changed! play anim!");
        }


        this.GetType().GetMethod((currentAttackPattern[AttackIndex] as Pattern).name).Invoke(this, (currentAttackPattern[AttackIndex] as Pattern).methodParams);

        AttackIndex++;

    }

    private async void OnBossHurt()
    {
        health -= 1;

        GetNode<TextureProgress>("Healthbar").Value = health;

        GD.Print(health);
        
        if(health <= 20 && health > 10)
        {
            nextStage = Stage.Medium;
            GetNode<Sprite>("Sprite").Frame = 1;
            GetNode<Global>("/root/Global").SpawnExplosion(GlobalPosition,4,0.5f,12);
        }
        else if(health <= 10)
        {
            nextStage = Stage.Hard;
            GetNode<Sprite>("Sprite").Frame = 2;
        }
        
        if(health <= 0)
        {
            attackTimer.Stop();
            GetNode<Tween>("Tween").StopAll();

            GD.Print("Victory!! Do boss defeat anim :D");

            // emit during/after boss defeat anim
            if(!GetNode<Global>("/root/Global").defeat && !GetNode<Global>("/root/Global").victory)
            {
                
                GetNode<Global>("/root/Global").Victory();

                GetNode<Global>("/root/Global").SpawnExplosion(GlobalPosition,64,5f,12f,false);
                GetNode<SignalManager>("/root/SignalManager").EmitSignal(nameof(SignalManager.BigShake));
                GetNode<ExplosionSoundLoop>("ExplosionSoundLoop").StartLoop();
                await ToSignal(GetTree().CreateTimer(4,false),"timeout");
                GetNode<ExplosionSoundLoop>("ExplosionSoundLoop").StopLoop();
                GetNode<ExplosionSoundLoop>("ExplosionSoundLoop").PlayBigExplosion();
                GetNode<SignalManager>("/root/SignalManager").EmitSignal(nameof(SignalManager.Victory));
            }
        }

    }
}
