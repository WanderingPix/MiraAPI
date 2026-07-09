namespace MiraAPI.Events.Vanilla.Chat;

/// <summary>
/// Event that is invoked when the chat screen is opened.
/// </summary>
public class OpenChatEvent : MiraEvent
{
    /// <summary>
    /// Gets the <see cref="ChatController"/> Instance.
    /// </summary>
    public ChatController Chat { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenChatEvent"/> class.
    /// </summary>
    /// <param name="chat">The <see cref="ChatController"/> Instance.</param>
    public OpenChatEvent(ChatController chat)
    {
        Chat = chat;
    }
}
