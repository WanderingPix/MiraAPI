namespace MiraAPI.Events.Vanilla.Chat;

/// <summary>
/// Event that is invoked after a new chat message is sent in chat.
/// </summary>
public class AfterSendChatMessageEvent : MiraEvent
{
    /// <summary>
    /// Gets the <see cref="ChatController"/> Instance.
    /// </summary>
    public ChatBubble Bubble { get; }

    /// <summary>
    /// Gets the <see cref="PlayerControl"/> of the sender.
    /// </summary>
    public PlayerControl Sender { get; }

    /// <summary>
    /// Gets the contents of the message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AfterSendChatMessageEvent"/> class.
    /// </summary>
    /// <param name="bubble">The <see cref="ChatBubble"/>  for the message.</param>
    public AfterSendChatMessageEvent(ChatBubble bubble, PlayerControl sender, string message)
    {
        Message = message;
        Sender = sender;
        Bubble = bubble;
    }
}
