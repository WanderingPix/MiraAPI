using System;

namespace MiraAPI.LocalSettings.Attributes;

/// <summary>
/// Creates a toggle and binds it with the config entry.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class LocalSettingToggleAttribute : LocalSettingAttribute
{
}
