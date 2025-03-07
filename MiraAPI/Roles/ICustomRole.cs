using System.Text;
using BepInEx.Configuration;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.PluginLoading;
using MiraAPI.Utilities;
using Reactor.Utilities;
using UnityEngine;

namespace MiraAPI.Roles;

/// <summary>
/// Interface for custom roles.
/// </summary>
public interface ICustomRole : IOptionable
{
    /// <summary>
    /// Gets the name of the role.
    /// </summary>
    string RoleName { get; }

    /// <summary>
    /// Gets the description of the role. Used in the Intro Cutscene.
    /// </summary>
    string RoleDescription { get; }

    /// <summary>
    /// Gets the long description of the role. Used in the Role Tab and Role Options.
    /// </summary>
    string RoleLongDescription { get; }

    /// <summary>
    /// Gets the color of the role.
    /// </summary>
    Color RoleColor { get; }

    /// <summary>
    /// Gets the team of the role.
    /// </summary>
    ModdedRoleTeams Team { get; }

    /// <summary>
    /// Speed multiplier, the normal player speed chosen by the host will be multiplied by this value.
    /// <summary>
    public Vector2 Speed
    {
        get => new Vector2(1f, 1f);
        set => Speed = value;
    }


    /// <summary>
    /// Gets advanced settings of the role.
    /// </summary>
    CustomRoleConfiguration Configuration { get; }

    /// <summary>
    /// Gets the role options group.
    /// </summary>
    RoleOptionsGroup RoleOptionsGroup => Team switch
    {
        ModdedRoleTeams.Crewmate => RoleOptionsGroup.Crewmate,
        ModdedRoleTeams.Impostor => RoleOptionsGroup.Impostor,
        ModdedRoleTeams.Custom => RoleOptionsGroup.Neutral,
        _ => new RoleOptionsGroup(RoleName, RoleColor),
    };

    /// <summary>
    /// Gets the role's TeamIntroCutscene configuration.
    /// </summary>
    TeamIntroConfiguration? IntroConfiguration => Team switch
    {
        ModdedRoleTeams.Custom => TeamIntroConfiguration.Neutral,
        _ => null,
    };

    /// <summary>
    /// Gets the parent mod of this role.
    /// </summary>
    MiraPluginInfo ParentMod => CustomRoleManager.FindParentMod(this);

    /// <summary>
    /// This method runs on the PlayerControl.FixedUpdate method for ALL players with this role.
    /// </summary>
    /// <param name="playerControl">The PlayerControl that has this role.</param>
    void PlayerControlFixedUpdate(PlayerControl playerControl)
    {
    }

    internal ConfigDefinition NumConfigDefinition => new("Roles", $"Num {GetType().FullName}");
    internal ConfigDefinition ChanceConfigDefinition => new("Roles", $"Chance {GetType().FullName}");

    /// <summary>
    /// Gets the role chance option.
    /// </summary>
    /// <returns>The role chance option.</returns>
    public virtual int? GetChance()
    {
        if (!Configuration.CanModifyChance)
        {
            return Configuration.DefaultChance;
        }

        if (ParentMod.PluginConfig.TryGetEntry(ChanceConfigDefinition, out ConfigEntry<int> entry))
        {
            return Mathf.Clamp(entry.Value, 0, 100);
        }

        return null;
    }

    /// <summary>
    /// Gets the role count option.
    /// </summary>
    /// <returns>The role count option.</returns>
    public virtual int? GetCount()
    {
        if (ParentMod.PluginConfig.TryGetEntry(NumConfigDefinition, out ConfigEntry<int> entry))
        {
            return Mathf.Clamp(entry.Value, 0, Configuration.MaxRoleCount);
        }

        return null;
    }

    /// <summary>
    /// Sets the role chance option.
    /// </summary>
    /// <param name="chance">The chance between 0 and 100.</param>
    public virtual void SetChance(int chance)
    {
        if (!Configuration.CanModifyChance)
        {
            Logger<MiraApiPlugin>.Error($"Cannot modify chance for role: {RoleName}");
            return;
        }

        if (ParentMod.PluginConfig.TryGetEntry(ChanceConfigDefinition, out ConfigEntry<int> entry))
        {
            entry.Value = Mathf.Clamp(chance, 0, 100);
            return;
        }

        Logger<MiraApiPlugin>.Error($"Error getting chance configuration for role: {RoleName}");
    }

    /// <summary>
    /// Sets the role count option.
    /// </summary>
    /// <param name="count">The amount of this role between zero and its MaxRoleCount in the Configuration.</param>
    public virtual void SetCount(int count)
    {
        if (ParentMod.PluginConfig.TryGetEntry(NumConfigDefinition, out ConfigEntry<int> entry))
        {
            entry.Value = Mathf.Clamp(count, 0, Configuration.MaxRoleCount);
            return;
        }

        Logger<MiraApiPlugin>.Error($"Error getting count configuration for role: {RoleName}");
    }

    /// <summary>
    /// Whether the local player can see this role.
    /// </summary>
    /// <param name="player">The player with the role.</param>
    /// <returns>Whether they can see the role (name color) or not.</returns>
    public virtual bool CanLocalPlayerSeeRole(PlayerControl player)
    {
        return (PlayerControl.LocalPlayer.Data.Role.IsImpostor && player.Data.Role.IsImpostor) || PlayerControl.LocalPlayer.Data.IsDead;
    }

    /// <summary>
    /// Allows the role to specify who is shown on the intro team screen.
    /// </summary>
    /// <param name="instance">The intro cutscene instance.</param>
    /// <param name="yourTeam">The reference to the list of player in the team.</param>
    /// <returns>True to use the original team intro code, false to skip.</returns>
    public virtual bool SetupIntroTeam(IntroCutscene instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam)
    {
        if (Team == ModdedRoleTeams.Custom)
        {
            var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();

            team.Add(PlayerControl.LocalPlayer);

            yourTeam = team;
        }

        return true;
    }

    /// <summary>
    /// This method runs on the HudManager.Update method ONLY when the LOCAL player has this role.
    /// </summary>
    /// <param name="hudManager">Reference to HudManager instance.</param>
    void HudUpdate(HudManager hudManager)
    {
    }

    /// <summary>
    /// Gets a custom ejection message for the role. Return null to use the default message.
    /// </summary>
    /// <param name="player">The NetworkedPlayerInfo object for this player.</param>
    /// <returns>A string with a custom ejection message or null.</returns>
    string? GetCustomEjectionMessage(NetworkedPlayerInfo player)
    {
        return Team == ModdedRoleTeams.Impostor ? $"{player.PlayerName} was The {RoleName}" : null;
    }

    /// <summary>
    /// Get the custom Role Tab text for this role.
    /// </summary>
    /// <returns>A StringBuilder with the role tab text.</returns>
    StringBuilder SetTabText() => Helpers.CreateForRole(this);

    /// <summary>
    /// Determine whether a given modifier can be applied to this role.
    /// </summary>
    /// <param name="modifier">The modifier to be tested.</param>
    /// <returns>True if the modifier is valid on this role, false otherwise.</returns>
    bool IsModifierApplicable(BaseModifier modifier)
    {
        return true;
    }
    public virtual void ChangeSpeed(Vector2 newspeed)
    {
        Speed = newspeed;
    }
    public virtual void DecreaseSpeed(float amountX, float amountY)
    {
        Speed = new Vector2(Speed.x - amountX, Speed.y - amountY);
    }
    public virtual void IncreaseSpeed(float amountX, float amountY)
    {
        Speed = new Vector2(Speed.x + amountX, Speed.y + amountY);
    }
}
