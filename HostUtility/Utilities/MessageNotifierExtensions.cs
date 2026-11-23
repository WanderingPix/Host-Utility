using UnityEngine;

namespace HostUtility.Utilities;

public static class MessageNotifierExtensions
{
    public static void AddMessage(this NotificationPopper popper, string message)
    {
        LobbyNotificationMessage newMessage = UnityEngine.Object.Instantiate<LobbyNotificationMessage>(popper.notificationMessageOrigin, Vector3.zero, Quaternion.identity, popper.transform);
        newMessage.transform.localPosition = new Vector3(0.0f, 0.0f, -2f);
        newMessage.SetUp(message, popper.settingsChangeSprite, popper.settingsChangeColor, (System.Action) (() => popper.OnMessageDestroy(newMessage)));
        SoundManager.Instance.PlaySound(popper.settingsChangeSound, false);
        popper.ShiftMessages();
        popper.AddMessageToQueue(newMessage);
    }
}