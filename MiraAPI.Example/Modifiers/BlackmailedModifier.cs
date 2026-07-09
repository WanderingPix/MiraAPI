using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Chat;
using MiraAPI.Modifiers;
using MiraAPI.Modifiers.Types;

namespace MiraAPI.Example.Modifiers;

public class BlackmailedModifier : GameModifier
{
    public override string ModifierName => "Blackmailed";

    public override string GetDescription()
    {
        return "You can't talk!";
    }

    public override int GetAssignmentChance()
    {
        return 0;
    }

    public override int GetAmountPerGame()
    {
        return 0;
    }

    [RegisterEvent]
    public static void BeforeChatMessageSend(BeforeSendChatMessageEvent @event)
    {
        if (@event.Sender.HasModifier<BlackmailedModifier>()) @event.Cancel();
    }
}
