using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using UnityEngine;

namespace MiraAPI.Example.Options;

public class ExampleOptions : AbstractOptionGroup
{
    public override string GroupName => "Example Options 1";
    public override Color GroupColor => Color.green;

    [ModdedToggleOption("Toggle Opt 1")]
    public bool ToggleOpt { get; set; } = false;

    [ModdedToggleOption("Toggle Opt 2")]
    public bool ToggleOpt2 { get; set; } = true;

    [ModdedNumberOption("Number Opt", min: 0, max: 10, increment: .25f, formatString: "0.00", suffixType: MiraNumberSuffixes.Percent)]
    public float NumberOpt { get; set; } = 4f;

    [ModdedEnumOption("Best API", typeof(AuApis), ["Mira API", "Mitochondria", "Reactor"])]
    public AuApis BestApiOpt { get; set; } = AuApis.MiraAPI;

    [ModdedEnumOption<AuApis>("Worst API", ["Mira API", "Mitochondria", "Reactor"])]
    public AuApis WorstApiOpt { get; set; } = AuApis.Mitochondria;
}

public enum AuApis
{
    MiraAPI,
    Mitochondria,
    Reactor,
}
