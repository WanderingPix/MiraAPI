using System;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using MiraAPI.PluginLoading;
using Reactor;
using Reactor.Networking;
using Reactor.Networking.Attributes;

namespace MiraAPI.Example.Extension;

[BepInAutoPlugin("mira.example.extension", "MiraExampleExtensionMod")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
[BepInDependency(MiraApiPlugin.Id)]
[BepInDependency(ExamplePlugin.Id)]
[ReactorModFlags(ModFlags.RequireOnAllClients)]
public partial class ExampleExtensionPlugin : BasePlugin, IMiraExtension
{
    public Harmony Harmony { get; } = new(Id);

    public override void Load()
    {
        Harmony.PatchAll();
    }

    public Type GetBasePluginType()
    {
        return typeof(ExamplePlugin);
    }
}
