using System;
using System.Reflection;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace MiraAPI.Utilities.Assets;

/// <summary>
/// A utility class for various sprite-related operations.
/// </summary>
public static class SpriteTools
{
    /// <summary>
    /// Loads and returns a texture from a resource path using the specified assembly.
    /// </summary>
    /// <param name="resourcePath">The path to the resource.</param>
    /// <param name="assembly">The assembly containing the resource.</param>
    /// <param name="pixelsPerUnit">The pixels per unit for the loaded sprite.</param>
    /// <returns>A sprite made from the resource.</returns>
    /// <exception cref="ArgumentException">The resource cannot be found.</exception>
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
            throw new ArgumentException($"Resource not found: {resourcePath}");
        }

        tex.name = resourcePath;
        return tex;
    }

    /// <summary>
    /// Loads and returns a <see cref="Sprite"/> from a resource path using the specified assembly.
    /// </summary>
    /// <param name="resourcePath">The path to the resource within the assembly.</param>
    /// <param name="assembly">The assembly from which to load the resource.</param>
    /// <param name="pixelsPerUnit">The number of pixels per unit for the sprite.</param>
    /// <returns>A <see cref="Sprite"/> object created from the texture loaded from the specified resource path.</returns>
    public static Sprite LoadSpriteFromPath(string resourcePath, Assembly assembly, float pixelsPerUnit)
    {
        var tex = LoadTextureFromResourcePath(resourcePath, assembly);
        var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
        sprite.name = resourcePath;
        return sprite;
    }
}
