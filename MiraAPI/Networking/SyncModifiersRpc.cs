﻿using Hazel;
using MiraAPI.Modifiers;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;

namespace MiraAPI.Networking;

[RegisterCustomRpc((uint)MiraRpc.SyncModifiers)]
internal class SyncModifiersRpc(MiraApiPlugin plugin, uint id) : PlayerCustomRpc<MiraApiPlugin, NetData[]>(plugin, id)
{
    public override RpcLocalHandling LocalHandling => RpcLocalHandling.None;

    public override void Write(MessageWriter writer, NetData[]? data)
    {
        if (data == null)
        {
            writer.WritePacked(0U);
            return;
        }

        writer.WritePacked((uint)data.Length);
        foreach (var netData in data)
        {
            writer.WritePacked(netData.Id);
            writer.WriteBytesAndSize(netData.Data);
        }
    }

    public override NetData[] Read(MessageReader reader)
    {
        var length = reader.ReadPackedUInt32();
        var data = new NetData[length];
        for (var i = 0; i < length; i++)
        {
            var dataId = reader.ReadPackedUInt32();
            var bytes = reader.ReadBytesAndSize();
            data[i] = new NetData(dataId, bytes);
        }

        return data;
    }

    public override void Handle(PlayerControl playerControl, NetData[]? data)
    {
        if (AmongUsClient.Instance.HostId != playerControl.OwnerId)
        {
            return;
        }

        ModifierManager.HandleSyncModifiers(data ?? []);
    }
}
