using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using MiraAPI.Utilities;
using MonoMod.Utils;

namespace MiraAPI.Modules;

/// <summary>
/// Represents a collection of enum values that can be stored and manipulated as a comma-separated string.
/// </summary>
/// <typeparam name="T">The enum type that this collection will store.</typeparam>
/// <param name="values">One or more enum values to initialize the collection with.</param>
[Serializable]
public struct MultiSelectValue<T>(params T[] values) : IEnumerable<T>, IEquatable<MultiSelectValue<T>>, IDisposable where T : struct, Enum
{
    /// <summary>
    /// Gets or sets the underlying comma-separated string representation of enum values.
    /// </summary>
    public string Values
    {
        readonly get => Helpers.Join(',', values);
        set => values = [.. value.TrueSplit(',').Select(Enum.Parse<T>)];
    }
    private HashSet<T> values = [.. values];

    /// <summary>
    /// Gets the number of values contained within.
    /// </summary>
    public readonly int Count => values.Count;

    /// <summary>
    /// Adds a single enum value to the end of the collection.
    /// </summary>
    /// <param name="item">The enum value to add.</param>
    /// <returns><c>true</c> if successfully added; otherwise, <c>false</c>.</returns>
    public readonly bool Add(T item) => values.Add(item);

    /// <summary>
    /// Removes the first occurrence of the specified enum value.<br/>
    /// Returns <c>true</c> if the value was found and removed, <c>false</c> otherwise.
    /// </summary>
    /// <param name="item">The enum value to remove.</param>
    /// <returns><c>true</c> if successfully removed; otherwise, <c>false</c>.</returns>
    public readonly bool Remove(T item) => values.Remove(item);

    /// <summary>
    /// Adds multiple enum values to the end of the collection.
    /// </summary>
    /// <param name="items">The collection of enum values to add.</param>
    public readonly void AddRange(IEnumerable<T> items)
    {
        foreach (var item in items)
            values.Add(item);
    }

    /// <summary>
    /// Removes multiple enum values from the collection.
    /// </summary>
    /// <param name="items">The enum values to remove.</param>
    /// <returns>The number of items successfully removed.</returns>
    public readonly int RemoveRange(params T[] items) => RemoveRange((IEnumerable<T>)items);

    /// <summary>
    /// Removes multiple enum values from the collection.
    /// </summary>
    /// <param name="items">The collection of enum values to remove.</param>
    /// <returns>The number of items successfully removed.</returns>
    public readonly int RemoveRange(IEnumerable<T> items) => items.Count(Remove);

    /// <summary>
    /// Determines whether the collection contains the specified enum value.
    /// </summary>
    /// <param name="item">The enum value to locate.</param>
    /// <returns>true if the item is found in the collection; otherwise, false.</returns>
    public readonly bool Contains(T item) => values.Contains(item);

    /// <summary>
    /// Removes all enum values that match the specified predicate condition.<br/>
    /// Iterates through the collection and removes each matching value.
    /// </summary>
    /// <param name="predicate">A function that defines the condition for removing items.</param>
    /// <returns>The number of items removed from the collection.</returns>
    public readonly int RemoveAll(Func<T, bool> predicate)
    {
        var count = 0;

        foreach (var item in this)
        {
            if (predicate(item) && Remove(item))
                count++;
        }

        return count;
    }

    /// <summary>
    /// Removes all enum values from the collection.
    /// </summary>
    public readonly void Clear() => values.Clear();

    /// <summary>
    /// Gets the first value in the collection.
    /// </summary>
    /// <returns>Returns the first value.</returns>
    public readonly T First() => values.First();

    /// <summary>
    /// Serializes the current instance to an array of bytes.
    /// </summary>
    /// <returns>An array of bytes representing the current instance.</returns>
    public readonly byte[] ToBytes() => [.. values.Select(Helpers.EnumToBytes).SelectMany(x => x)];

    /// <summary>
    /// Converts the collection to its string representation.
    /// </summary>
    /// <returns>A comma-separated string of enum values.</returns>
    public override readonly string ToString() => Values;

    /// <inheritdoc/>
    public override readonly bool Equals(object? obj) => obj is MultiSelectValue<T> other && Equals(other);

    /// <inheritdoc/>
    public readonly bool Equals(MultiSelectValue<T> other) => values.SetEquals(other.values);

    /// <inheritdoc/>
    public override readonly int GetHashCode() => Values?.GetHashCode() ?? 0;

    /// <inheritdoc/>
    public readonly IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)values).GetEnumerator();

    /// <inheritdoc/>
    readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public readonly void Dispose() => values.Clear();

    /// <summary>
    /// Creates an instance of <see cref="MultiSelectValue{T}"/> from the provided byte array.
    /// </summary>
    /// <param name="bytes">The bytes to deserialize from.</param>
    /// <returns>An instance of <see cref="MultiSelectValue{T}"/>.</returns>
    public static MultiSelectValue<T> FromBytes(byte[] bytes) => new([.. Helpers.EnumsFromBytes<T>(bytes)]);

    /// <summary>
    /// Converts the current instance to its string representation.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator string(MultiSelectValue<T> value) => value.Values;

    /// <summary>
    /// Converts the current instance to its array representation of values.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator T[](MultiSelectValue<T> value) => [.. value.values];

    /// <summary>
    /// Converts the current instance to one singular value.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <exception cref="ConversionFailException">There were either no values, or more than one value.</exception>
#pragma warning disable S3877 // Exceptions should not be thrown from unexpected methods
    public static implicit operator T(MultiSelectValue<T> value) =>
        value.Count == 1
            ? value.values.First()
            : throw new ConversionFailException($"Tried to convert multiple or no values ({value.Values}) to a singular one");
#pragma warning restore S3877 // Exceptions should not be thrown from unexpected methods

    /// <summary>
    /// Converts the value to an instance of MultiSelectValue.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator MultiSelectValue<T>(T value) => new(value);

    /// <summary>
    /// Converts the array of values to an instance of MultiSelectValue.
    /// </summary>
    /// <param name="values">The values to convert.</param>
    public static implicit operator MultiSelectValue<T>(T[] values) => new(values);

    /// <summary>
    /// Converts the value string to an instance of MultiSelectValue.
    /// </summary>
    /// <param name="values">The values to convert.</param>
    public static implicit operator MultiSelectValue<T>(string values) => new([.. values.TrueSplit(',').Select(Enum.Parse<T>)]);

    /// <summary>
    /// Equality comparison.
    /// </summary>
    /// <param name="left">Left.</param>
    /// <param name="right">Right.</param>
    public static bool operator ==(MultiSelectValue<T> left, MultiSelectValue<T> right) => left.Equals(right);

    /// <summary>
    /// Inequality comparison.
    /// </summary>
    /// <param name="left">Left.</param>
    /// <param name="right">Right.</param>
    public static bool operator !=(MultiSelectValue<T> left, MultiSelectValue<T> right) => !left.Equals(right);
}
