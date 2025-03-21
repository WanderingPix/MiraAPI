using System;
using System.Collections.Generic;
using System.Linq;

namespace MiraAPI.Utilities;

/// <summary>
/// Enum serializer for converting enums to and from byte arrays.
/// </summary>
public static class EnumSerializer
{
    /// <summary>
    /// Serializes an enum to an array of bytes.
    /// </summary>
    /// <param name="value">The enum value to serialize.</param>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <returns>A byte array representing the enum value.</returns>
    /// <exception cref="ArgumentException">Thrown when the underlying type is not recognized.</exception>
    public static byte[] EnumToBytes<T>(T value) where T : struct, Enum
    {
        var underlyingType = typeof(T).GetEnumUnderlyingType();

        return underlyingType switch
        {
            not null when underlyingType == typeof(byte) => [(byte)(object)value],
            not null when underlyingType == typeof(sbyte) => [(byte)(sbyte)(object)value],
            not null when underlyingType == typeof(short) => BitConverter.GetBytes((short)(object)value),
            not null when underlyingType == typeof(ushort) => BitConverter.GetBytes((ushort)(object)value),
            not null when underlyingType == typeof(int) => BitConverter.GetBytes((int)(object)value),
            not null when underlyingType == typeof(uint) => BitConverter.GetBytes((uint)(object)value),
            not null when underlyingType == typeof(long) => BitConverter.GetBytes((long)(object)value),
            not null when underlyingType == typeof(ulong) => BitConverter.GetBytes((ulong)(object)value),
            _ => throw new ArgumentException("Unknown underlying type for " + typeof(T).Name),
        };
    }

    /// <summary>
    /// Converts bytes to an enum using the underlying type.
    /// </summary>
    /// <param name="bytes">The byte array to convert.</param>
    /// <param name="offset">The offset in the byte array to start reading from.</param>
    /// <param name="underlyingType">The underlying type of the enum.</param>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <returns>The enum value represented by the bytes.</returns>
    /// <exception cref="ArgumentException">Thrown when the underlying type is not recognized.</exception>
    public static T EnumFromBytes<T>(byte[] bytes, int offset, Type underlyingType) where T : struct, Enum
    {
        return underlyingType switch
        {
            { } t when t == typeof(byte) || t == typeof(sbyte) => (T)(object)bytes[offset],
            { } t when t == typeof(short) => (T)(object)BitConverter.ToInt16(bytes, offset),
            { } t when t == typeof(ushort) => (T)(object)BitConverter.ToUInt16(bytes, offset),
            { } t when t == typeof(int) => (T)(object)BitConverter.ToInt32(bytes, offset),
            { } t when t == typeof(uint) => (T)(object)BitConverter.ToUInt32(bytes, offset),
            { } t when t == typeof(long) => (T)(object)BitConverter.ToInt64(bytes, offset),
            { } t when t == typeof(ulong) => (T)(object)BitConverter.ToUInt64(bytes, offset),
            _ => throw new ArgumentException("Unknown underlying type for " + typeof(T).Name),
        };
    }

    /// <inheritdoc cref="EnumFromBytes{T}(byte[], int, Type)"/>
    public static T EnumFromBytes<T>(byte[] bytes, int offset) where T : struct, Enum => EnumFromBytes<T>(bytes, offset, typeof(T).GetEnumUnderlyingType());

    /// <summary>
    /// Converts a byte array into a collection of enum values.
    /// </summary>
    /// <param name="bytes">The byte array to convert.</param>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <returns>An enumerable collection of enum values.</returns>
    /// <exception cref="ArgumentException">Thrown when the byte array length is not a multiple of the enum's underlying type size.</exception>
    public static IEnumerable<T> EnumsFromBytes<T>(byte[] bytes) where T : struct, Enum
    {
        var underlyingType = typeof(T).GetEnumUnderlyingType();
        var size = GetManagedSize(underlyingType);

        if (bytes.Length % size != 0)
        {
            throw new ArgumentException("Byte array length must be a multiple of " + size + ".");
        }

        return EnumsFromBytesIterator<T>(bytes, underlyingType, size);
    }

    private static IEnumerable<T> EnumsFromBytesIterator<T>(byte[] bytes, Type underlyingType, int size) where T : struct, Enum
    {
        var count = bytes.Length / size;
        var underlyingArray = Array.CreateInstance(underlyingType, count);
        Buffer.BlockCopy(bytes, 0, underlyingArray, 0, bytes.Length);

        foreach (var value in underlyingArray)
            yield return (T)value;
    }

    /// <summary>
    /// Converts a collection of enum values to a byte array.
    /// </summary>
    /// <param name="values"> The collection of enum values to convert.</param>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <returns>A byte array representing the collection of enum values.</returns>
    public static byte[] EnumsToBytes<T>(IEnumerable<T> values) where T : struct, Enum
    {
        var underlyingType = typeof(T).GetEnumUnderlyingType();
        var size = GetManagedSize(underlyingType);
        var valuesArray = values.ToArray();
        var count = valuesArray.Length;

        var underlyingArray = Array.CreateInstance(underlyingType, count);
        for (var i = 0; i < count; i++)
        {
            underlyingArray.SetValue(ConvertToUnderlyingType(valuesArray[i], underlyingType), i);
        }

        var bytes = new byte[count * size];
        Buffer.BlockCopy(underlyingArray, 0, bytes, 0, bytes.Length);
        return bytes;
    }

    private static object ConvertToUnderlyingType<T>(T value, Type underlyingType) where T : struct, Enum
    {
        return underlyingType switch
        {
            not null when underlyingType == typeof(byte) => (byte)(object)value,
            not null when underlyingType == typeof(sbyte) => (sbyte)(object)value,
            not null when underlyingType == typeof(short) => (short)(object)value,
            not null when underlyingType == typeof(ushort) => (ushort)(object)value,
            not null when underlyingType == typeof(int) => (int)(object)value,
            not null when underlyingType == typeof(uint) => (uint)(object)value,
            not null when underlyingType == typeof(long) => (long)(object)value,
            not null when underlyingType == typeof(ulong) => (ulong)(object)value,
            _ => throw new InvalidOperationException("Unsupported underlying type."),
        };
    }

    private static int GetManagedSize(Type type)
    {
        return Type.GetTypeCode(type) switch
        {
            TypeCode.Byte or TypeCode.SByte => 1,
            TypeCode.Int16 or TypeCode.UInt16 => 2,
            TypeCode.Int32 or TypeCode.UInt32 => 4,
            TypeCode.Int64 or TypeCode.UInt64 => 8,
            _ => throw new ArgumentException("Unsupported underlying type: " + type.Name),
        };
    }
}
