using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using MiraAPI.PluginLoading;
using Reactor;
using Reactor.Networking;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using UnityEngine;

namespace MiraAPI;

[BepInAutoPlugin("mira.api", "MiraAPI")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
[ReactorModFlags(ModFlags.RequireOnAllClients)]
public partial class MiraApiPlugin : BasePlugin, IMiraPlugin
{
    private static MiraPluginManager? PluginManager { get; set; }

    public Harmony Harmony { get; } = new(Id);

    public static Color MiraColor { get; } = new Color32(238, 154, 112, 255);

    public static MiraApiPlugin Instance { get; private set; }

    public string OptionsTitleText => "Mira API";

    public override void Load()
    {
        Instance = this;

        Harmony.PatchAll();

        ReactorCredits.Register("Mira API", Version, true, ReactorCredits.AlwaysShow);

        PluginManager = new MiraPluginManager();
        PluginManager.Initialize();
    }

    public ConfigFile GetConfigFile() => Config;
}
