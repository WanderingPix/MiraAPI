using System;

namespace MiraAPI.LocalSettings.Attributes;

/// <summary>
/// Base class for all local settings attributes.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public abstract class LocalSettingAttribute : Attribute
{
}
