using Godot;
using System;

public class Menu : Control
{
    enum CurrentMenu
    {
        Main,
        Options,
        HowToPlay
    }

    [Export]
    public PackedScene GameScene;


    private int helpindex;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ChangeMenu(CurrentMenu.Main);

        GetNode<Button>("MenuAligner/OptionsPanel/ToggleMusic").Pressed = !AudioServer.IsBusMute(AudioServer.GetBusIndex("Music"));
        GetNode<Button>("MenuAligner/OptionsPanel/ToggleSound").Pressed = !AudioServer.IsBusMute(AudioServer.GetBusIndex("Sound"));
    
        //AudioServer.SetBusMute(AudioServer.GetBusIndex("Sound"), !GetNode<Button>("MenuAligner/OptionsPanel/ToggleSound").Pressed);

    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

    private void ChangeMenu(CurrentMenu newMenu)
    {
        if(newMenu == CurrentMenu.Main)
        {
            GetNode<VBoxContainer>("MenuAligner/OptionsPanel").Hide();
            GetNode<VBoxContainer>("MenuAligner/HowToPlayPanel").Hide();
            //GetNode<HBoxContainer>("MenuAligner").Alignment = BoxContainer.AlignMode.Center;
        }
        else if(newMenu == CurrentMenu.Options)
        {
            //GetNode<HBoxContainer>("MenuAligner").Alignment = BoxContainer.AlignMode.Begin;
            GetNode<VBoxContainer>("MenuAligner/HowToPlayPanel").Hide();
            GetNode<VBoxContainer>("MenuAligner/OptionsPanel").Show();
        }
        else if(newMenu == CurrentMenu.HowToPlay)
        {
            //GetNode<HBoxContainer>("MenuAligner").Alignment = BoxContainer.AlignMode.Begin;
            GetNode<VBoxContainer>("MenuAligner/OptionsPanel").Hide();
            GetNode<VBoxContainer>("MenuAligner/HowToPlayPanel").Show();
        }
    }

    private void OnStartButtonDown()
    {
        GD.Print("Do transition to game");
        GetTree().ChangeSceneTo(GameScene);
    }

    private void OnOptionsButtonDown()
    {
        ChangeMenu(CurrentMenu.Options);
    }

    private void OnHowToPlayButtonDown()
    {
        ChangeMenu(CurrentMenu.HowToPlay);
    }

    private void OnMusicToggled(bool toggle)
    {
        AudioServer.SetBusMute(AudioServer.GetBusIndex("Music"), !toggle);
    }

    private void OnSoundToggled(bool toggle)
    {
        AudioServer.SetBusMute(AudioServer.GetBusIndex("Sound"), !toggle);
    }

    private void OnHelpLeftDown()
    {
        helpindex = helpindex > 1 ? helpindex - 1 : 4;
        GetNode<TextureRect>("MenuAligner/HowToPlayPanel/Help").Texture = GD.Load<Texture>("res://sprites/help"+helpindex+".png");
    }

    private void OnHelpRightDown()
    {
        helpindex = helpindex < 4 ? helpindex + 1 : 1;
        GetNode<TextureRect>("MenuAligner/HowToPlayPanel/Help").Texture = GD.Load<Texture>("res://sprites/help"+helpindex+".png");
    }
}
