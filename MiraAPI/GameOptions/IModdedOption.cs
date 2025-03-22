using System;
using BepInEx.Configuration;
using MiraAPI.Networking;
using MiraAPI.PluginLoading;
using UnityEngine;

namespace MiraAPI.GameOptions;

/// <summary>
/// Represents a modded game option interface that can be used to create custom game settings.
/// </summary>
public interface IModdedOption
{
    /// <summary>
    /// Gets the unique identifier of the option.
    /// </summary>
    uint Id { get; }

    /// <summary>
    /// Gets the title of the option.
    /// </summary>
    string Title { get; }

    /// <summary>
    /// Gets the StringName object of the option.
    /// </summary>
    StringNames StringName { get; }

    /// <summary>
    /// Gets or sets the parent mod of the option.
    /// </summary>
    IMiraPlugin? ParentMod { get; set; }

    /// <summary>
    /// Gets the BaseGameSetting data of the option.
    /// </summary>
    BaseGameSetting? Data { get; }

    /// <summary>
    /// Gets the option behaviour of the option.
    /// </summary>
    OptionBehaviour? OptionBehaviour { get; }

    /// <summary>
    /// Gets or sets the visibility of the option.
    /// </summary>
    Func<bool> Visible { get; set; }

    /// <summary>
    /// Gets or sets the config definition of the option.
    /// </summary>
    ConfigDefinition? ConfigDefinition { get; set; }

    /// <summary>
    /// Creates the option behaviour.
    /// </summary>
    /// <param name="toggleOpt">The ToggleOption prefab.</param>
    /// <param name="numberOpt">The NumberOption prefab.</param>
    /// <param name="stringOpt">The StringOption prefab.</param>
    /// <param name="container">The options container.</param>
    /// <returns>A new OptionBehaviour for this modded option.</returns>
    OptionBehaviour CreateOption(ToggleOption toggleOpt, NumberOption numberOpt, StringOption stringOpt, Transform container);

    /// <summary>
    /// Gets the float data of the option.
    /// </summary>
    /// <returns>A float object representing the option's value.</returns>
    float GetFloatData();

    /// <summary>
    /// Gets the net data of the option.
    /// </summary>
    /// <returns>A NetData object representing this option's data.</returns>
    NetData GetNetData();

    /// <summary>
    /// Handles incoming net data.
    /// </summary>
    /// <param name="data">The NetData's byte array.</param>
    void HandleNetData(byte[] data);

    /// <summary>
    /// Changes the scales and positions of the option if it's attached to the game options menu.
    /// </summary>
    void ChangeGameSetting();

    /// <summary>
    /// Changes the scales and positions of the option if it's attached to the roles settings menu.
    /// </summary>
    void ChangeRoleSetting();
}
