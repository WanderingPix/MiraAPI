﻿using System;
using System.Reflection;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace MiraAPI.Utilities.Assets;

/// <summary>
/// A utility class for various sprite-related operations.
/// </summary>
public static class SpriteTools
{
    /// <summary>
    /// Load a sprite from a resource path.
    /// </summary>
    /// <param name="resourcePath">The path to the resource.</param>
    /// <returns>A sprite made from the resource.</returns>
    /// <exception cref="Exception">The resource cannot be found.</exception>
    public static Sprite LoadSpriteFromPath(string resourcePath, Assembly assembly, float pixelsPerUnit)
    {
        var tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        var myStream = assembly.GetManifestResourceStream(resourcePath);
        if (myStream != null)
        {
            var buttonTexture = myStream.ReadFully();
            tex.LoadImage(buttonTexture, false);
        }
        else
        {
            Logger<MiraApiPlugin>.Error($"Resource not found: {resourcePath}\nReturning empty sprite!");
        }

        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
    }
}
