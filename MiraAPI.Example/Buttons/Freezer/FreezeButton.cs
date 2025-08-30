﻿using MiraAPI.Example.Modifiers.Freezer;
using MiraAPI.Example.Options.Roles;
using MiraAPI.Example.Roles;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Rewired;
using UnityEngine;

namespace MiraAPI.Example.Buttons.Freezer;

public class FreezeButton : CustomActionButton<PlayerControl>
{
    public override string Name => "Freeze";

    public override float Cooldown => OptionGroupSingleton<FreezerRoleSettings>.Instance.FreezeDuration;

    public override LoadableAsset<Sprite> Sprite => ExampleAssets.ExampleButton;
    public override KeyboardKeyCode? DefaultKeybind => KeyboardKeyCode.T;
    public override ModifierKey Modifier1 => ModifierKey.Control;

    protected override void OnClick()
    {
        Target?.RpcAddModifier<FreezeModifier>();
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestPlayer(true, Distance);
    }

    public override void SetOutline(bool active)
    {
        Target?.cosmetics.SetOutline(active, new Il2CppSystem.Nullable<Color>(Palette.Blue));
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        return true;
    }

    public override bool Enabled(RoleBehaviour? role)
    {
        return role is FreezerRole;
    }
}
