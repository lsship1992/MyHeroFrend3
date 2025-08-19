using UnityEngine;

public static class DebugLogger
{
    public static void Log(string message, UnityEngine.Object context = null)
    {
        Debug.Log(message, context);
    }

    public static void LogError(string message, UnityEngine.Object context = null)
    {
        Debug.LogError(message, context);
    }

    public static void LogWarning(string message, UnityEngine.Object context = null)
    {
        Debug.LogWarning(message, context);
    }
}