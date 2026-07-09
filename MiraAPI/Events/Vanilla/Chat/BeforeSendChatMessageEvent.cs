namespace MiraAPI.Events.Vanilla.Chat;

/// <summary>
/// Event that is invoked before a new chat message is sent in chat, this event is cancelable.
/// </summary>
public class BeforeSendChatMessageEvent : MiraCancelableEvent
{
    /// <summary>
    /// Gets the <see cref="ChatController"/> Instance.
    /// </summary>
    public ChatController Chat { get; }

    /// <summary>
    /// Gets the <see cref="PlayerControl"/> of the sender.
    /// </summary>
    public PlayerControl Sender { get; }

    /// <summary>
    /// Gets the contents of the message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BeforeSendChatMessageEvent"/> class.
    /// </summary>
    /// <param name="chat">The <see cref="ChatController"/> Instance.</param>
    public BeforeSendChatMessageEvent(ChatController chat, string message, PlayerControl sender)
    {
        Chat = chat;
        Message = message;
        Sender = sender;
    }
}
