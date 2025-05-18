using MiraAPI.Example.Extension.Modifiers;
using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;

namespace MiraAPI.Example.Extension.Options;

public class ExtensionModifierOptions : AbstractOptionGroup<ExtensionModifier>
{
    public override string GroupName => "Extension Modifier Options";

    [ModdedNumberOption("Modifier Chance", 0, 100, 10, MiraNumberSuffixes.Percent)]
    public float ModifierChance { get; set; } = 100f;

    [ModdedNumberOption("Modifier Limit", 0, 10)]
    public float ModifierLimit { get; set; } = 0;
}
