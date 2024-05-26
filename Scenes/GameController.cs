using Godot;
using System;

public partial class GameController : Node
{
    public GameController Instance { get; private set; }

    public GameSettings GameSettings { get; private set; }

	public override void _Ready()
	{
        Instance = this;

        GameSettings = new GameSettings();
	}

	public override void _Process(double delta)
	{
        GatherInput();
	}

    private void GatherInput()
    {
        if (Input.IsActionJustPressed("escape"))
            ToggleMouseMode();
    }

    private void ToggleMouseMode()
    {
        if (Input.MouseMode == Input.MouseModeEnum.Captured)
            Input.MouseMode = Input.MouseModeEnum.Visible;
        else
            Input.MouseMode = Input.MouseModeEnum.Captured;
    }
}
