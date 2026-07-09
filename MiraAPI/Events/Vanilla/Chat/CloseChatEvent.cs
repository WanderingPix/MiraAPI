namespace MiraAPI.Events.Vanilla.Chat;

/// <summary>
/// Event that is invoked when the chat screen is closed.
/// </summary>
public class CloseChatEvent : MiraEvent
{
    /// <summary>
    /// Gets the <see cref="ChatController"/> Instance.
    /// </summary>
    public ChatController Chat { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CloseChatEvent"/> class.
    /// </summary>
    /// <param name="chat">The <see cref="ChatController"/> Instance.</param>
    public CloseChatEvent(ChatController chat)
    {
        Chat = chat;
    }
}
