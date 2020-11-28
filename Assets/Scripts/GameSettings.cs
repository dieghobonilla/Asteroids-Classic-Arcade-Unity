using UnityEngine;

public static class GameSettings
{
    public static Vector2 ScreenLimits { get; set; }

    public static void SetScreenLimits(Camera camera)
    {
        ScreenLimits = camera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
    }
}