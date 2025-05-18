using MiraAPI.Example.Extension.Options;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers.Types;

namespace MiraAPI.Example.Extension.Modifiers;

public class ExtensionModifier : GameModifier
{
    public override string ModifierName => "Extension Modifier";

    public override string GetDescription() => "This is from the extension mod!";

    public override int GetAssignmentChance() => (int)OptionGroupSingleton<ExtensionModifierOptions>.Instance.ModifierChance;

    public override int GetAmountPerGame() => (int)OptionGroupSingleton<ExtensionModifierOptions>.Instance.ModifierLimit;
}
