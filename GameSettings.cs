public class GameSettings
{
    public static float MouseSensitivity { get; set; }

    public static float CameraSensitivity { get; set; }

    public GameSettings()
    {
        MouseSensitivity = 1f;
        CameraSensitivity = 2.5f;
    }
}