﻿using MiraAPI.Events;
using MiraAPI.Events.Mira;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Map;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.Example.Buttons.Freezer;
using Reactor.Utilities;

namespace MiraAPI.Example;

public static class ExampleEventHandlers
{
    public static void Initialize()
    {
        // Register event handlers here
        MiraEventManager.RegisterEventHandler<MiraButtonClickEvent<FreezeButton>>(FreezeButtonClickHandler, 1);
        MiraEventManager.RegisterEventHandler<MiraButtonCancelledEvent<FreezeButton>>(FreezeButtonCancelledHandler);
        MiraEventManager.RegisterEventHandler<UpdateSystemEvent>(UpdateSystemEventHandler);

        MiraEventManager.RegisterEventHandler<BeforeMurderEvent>(@event =>
        {
            Logger<ExamplePlugin>.Info($"{@event.Source.Data.PlayerName} is about to kill {@event.Target.Data.PlayerName}");
        });

        MiraEventManager.RegisterEventHandler<AfterMurderEvent>(@event =>
        {
            Logger<ExamplePlugin>.Info($"{@event.Source.Data.PlayerName} has killed {@event.Target.Data.PlayerName}");
        });

        MiraEventManager.RegisterEventHandler<CompleteTaskEvent>(@event =>
        {
            Logger<ExamplePlugin>.Info($"{@event.Player.Data.PlayerName} completed {@event.Task.TaskType.ToString()}");
        });
    }

    public static void UpdateSystemEventHandler(UpdateSystemEvent @event)
    {
        Logger<ExamplePlugin>.Error(@event.SystemType.ToString());
    }

    // Example event handler
    public static void FreezeButtonClickHandler(MiraButtonClickEvent<FreezeButton> @event)
    {
        Logger<ExamplePlugin>.Warning("Freeze button clicked!");

        if (PlayerControl.LocalPlayer.Data.PlayerName == "stupid")
        {
            @event.Cancel();
            @event.Button.SetTimer(15f);
        }
    }

    // Example event handler
    public static void FreezeButtonCancelledHandler(MiraButtonCancelledEvent<FreezeButton> @event)
    {
        Logger<ExamplePlugin>.Warning("Freeze button cancelled!");
        @event.Button.OverrideName("Freeze Canceled");
    }
}
