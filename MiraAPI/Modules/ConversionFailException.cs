using System;

namespace MiraAPI.Modules;

/// <summary>
/// Denotes a failure in conversion of values.
/// </summary>
/// <param name="message">The message to log.</param>
public class ConversionFailException(string message) : Exception(message);
