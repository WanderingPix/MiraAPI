using MiraAPI.Example.Roles;
using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;

namespace MiraAPI.Example.Options.Roles;

public class FreezerRoleSettings : AbstractOptionGroup<FreezerRole>
{
    public override string GroupName => "Custom Role";

    [ModdedNumberOption("Freeze Duration", 1, 15, 1, MiraNumberSuffixes.Seconds)]
    public float FreezeDuration { get; set; } = 5;
}
