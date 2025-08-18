using UnityEngine;

public static class DebugLogger
{
    public static void Log(string message, Object context = null)
    {
        Debug.Log($"[DEBUG] {message}", context);
    }

    public static void LogWarning(string message, Object context = null)
    {
        Debug.LogWarning($"[WARNING] {message}", context);
    }

    public static void LogError(string message, Object context = null)
    {
        Debug.LogError($"[ERROR] {message}", context);
    }
}