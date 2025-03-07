using MiraAPI.Example.Options.Modifiers;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers.Types;
using UnityEngine;

namespace MiraAPI.Example.Modifiers;

public class FlashModifier : GameModifier
{
    public override string ModifierName => "Flash";

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<CaptainModifierSettings>.Instance.Chance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<CaptainModifierSettings>.Instance.Amount;
    }
    public override Vector2 Speed => new Vector2(3f, 3f);
}
