using System;
using BepInEx.Configuration;
using UnityEngine;

namespace MiraAPI.LocalSettings.Attributes;

/// <summary>
/// Base class for all local settings attributes.
/// </summary>
/// <typeparam name="T">The local setting type the attribute handles.</typeparam>
[AttributeUsage(AttributeTargets.Property)]
public abstract class LocalSettingAttribute(
    LocalSettingsTab tab,
    string? name = null,
    string? description = null) : Attribute
{
    /// <summary>
    /// Returns the created setting object.
    /// </summary>
    /// <returns>The created class.</returns>
    protected abstract GameObject CreateSetting();
}