using UnityEngine;

public class TestNotification : MonoBehaviour
{
    public NotificationLogManager logManager;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            logManager.SendFormattedNotification("One In The Chamber", "Your", logManager.customBlue, "Backfired", logManager.customYellow);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            logManager.SendFormattedNotification("Deep Cut", "Opponent's", logManager.customRed, "Backfired", logManager.customYellow);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            logManager.SendFormattedNotification("Turn Was", "Your", logManager.customBlue, "Skipped", logManager.customYellow);
        }
    }
}
