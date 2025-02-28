﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Reactor.Utilities;

namespace MiraAPI.Events;

/// <summary>
/// Mira Event manager.
/// </summary>
public static class MiraEventManager
{
    private static readonly Dictionary<Type, List<MiraEventWrapper>> EventWrappers = [];

    /// <summary>
    /// Invoke an event.
    /// </summary>
    /// <param name="eventInstance">The event instance.</param>
    /// <typeparam name="T">Type of Event.</typeparam>
    /// <returns>If there was an event handler invoked for this event, return true. Otherwise, return false.</returns>
    public static bool InvokeEvent<T>(T eventInstance) where T : MiraEvent
    {
        EventWrappers.TryGetValue(typeof(T), out var handlers);
        if (handlers == null)
        {
            return false;
        }

        foreach (var handler in handlers)
        {
            ((Action<T>)handler.EventHandler).Invoke(eventInstance);
        }

        return true;
    }

    /// <summary>
    /// Invoke an event and use a specific type to find the handlers.
    /// </summary>
    /// <param name="eventInstance">The event instance.</param>
    /// <param name="type">The type to use for handler lookup.</param>
    /// <returns>If there was an event handler invoked for this event, return true. Otherwise, return false.</returns>
    public static bool InvokeEvent(MiraEvent eventInstance, Type type)
    {
        EventWrappers.TryGetValue(type, out var handlers);
        if (handlers == null)
        {
            return false;
        }

        foreach (var handler in handlers)
        {
            handler.EventHandler.DynamicInvoke(eventInstance);
        }

        return true;
    }

    /// <summary>
    /// Register an event.
    /// </summary>
    /// <param name="type">The type of event.</param>
    /// <param name="methodInfo">The MethodInfo of the event handler.</param>
    /// <param name="priority">The priority of the event handler. Higher values are called first.</param>
    public static void RegisterEventHandler(Type type, MethodInfo methodInfo, int priority = 0)
    {
        if (!type.IsSubclassOf(typeof(MiraEvent)))
        {
            Logger<MiraApiPlugin>.Error($"Type must be a subclass of MiraEvent: {nameof(type)}");
            return;
        }

        if (!EventWrappers.TryGetValue(type, out var handlers))
        {
            handlers = [];
            EventWrappers.Add(type, handlers);
        }

        var @delegate = Delegate.CreateDelegate(typeof(Action<>).MakeGenericType(type), methodInfo);
        handlers.Add(new MiraEventWrapper(@delegate, priority));
    }

    /// <summary>
    /// Register an event.
    /// </summary>
    /// <param name="handler">The callback method/handler for the event.</param>
    /// <param name="priority">The priority of the event handler. Higher values are called first.</param>
    /// <typeparam name="T">Type of event.</typeparam>
    public static void RegisterEventHandler<T>(Action<T> handler, int priority = 0) where T : MiraEvent
    {
        if (!EventWrappers.ContainsKey(typeof(T)))
        {
            EventWrappers.Add(typeof(T), []);
        }

        var handlers = EventWrappers[typeof(T)];
        handlers.Add(new MiraEventWrapper(handler, priority));
    }

    internal static void SortAllHandlers()
    {
        foreach (var handlers in EventWrappers.Values)
        {
            handlers.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }
    }
}
