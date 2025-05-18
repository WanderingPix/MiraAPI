using MiraAPI.Example.Roles;
using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;

namespace MiraAPI.Example.Extension.Options;

public class ExtendedFreezerOptions : AbstractOptionGroup<FreezerRole>
{
    public override string GroupName => "Extended Freezer Role Options";

    [ModdedNumberOption("Freeze Uses", 1, 5)]
    public float FreezeUses { get; set; } = 1;
}
