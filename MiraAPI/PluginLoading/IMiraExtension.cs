using System;

namespace MiraAPI.PluginLoading;

public interface IMiraExtension
{
    public Type GetBasePluginType();
}
