namespace Mediarq.Core.Common.Requests.Abstraction;

/// <summary>
/// Represents a void type, since <see cref="System.Void"/> cannot be used as a generic type argument.
/// Used as the response type for commands that do not return a value, allowing them to flow
/// through the same request pipeline as commands and queries that do.
/// </summary>
public readonly struct Unit : IEquatable<Unit>
{
    /// <summary>
    /// The single, default <see cref="Unit"/> value.
    /// </summary>
    public static readonly Unit Value;

    /// <summary>
    /// A completed task whose result is the <see cref="Unit"/> value.
    /// </summary>
    public static Task<Unit> Task { get; } = System.Threading.Tasks.Task.FromResult(Value);

    /// <summary>
    /// Determines whether this instance is equal to another <see cref="Unit"/>. Always <see langword="true"/>.
    /// </summary>
    /// <param name="other">The other unit value.</param>
    /// <returns><see langword="true"/>.</returns>
    public bool Equals(Unit other) => true;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Unit;

    /// <inheritdoc />
    public override int GetHashCode() => 0;

    /// <summary>Equality operator. Two <see cref="Unit"/> values are always equal.</summary>
    public static bool operator ==(Unit left, Unit right) => true;

    /// <summary>Inequality operator. Two <see cref="Unit"/> values are never unequal.</summary>
    public static bool operator !=(Unit left, Unit right) => false;

    /// <summary>Returns the string representation of the unit value, <c>()</c>.</summary>
    public override string ToString() => "()";
}
