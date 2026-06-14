using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Mediarq.SourceGenerators;

/// <summary>
/// A value-equatable wrapper around <see cref="ImmutableArray{T}"/> so that incremental generator
/// pipeline outputs are compared by content, enabling proper caching.
/// </summary>
internal readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>>, IEnumerable<T>
    where T : IEquatable<T>
{
    private readonly ImmutableArray<T> _array;

    public EquatableArray(ImmutableArray<T> array) => _array = array;

    public int Length => _array.IsDefault ? 0 : _array.Length;

    public bool Equals(EquatableArray<T> other)
    {
        if (_array.IsDefault)
        {
            return other._array.IsDefault;
        }

        if (other._array.IsDefault || _array.Length != other._array.Length)
        {
            return false;
        }

        for (int i = 0; i < _array.Length; i++)
        {
            if (!_array[i].Equals(other._array[i]))
            {
                return false;
            }
        }

        return true;
    }

    public override bool Equals(object? obj) => obj is EquatableArray<T> other && Equals(other);

    public override int GetHashCode()
    {
        if (_array.IsDefault)
        {
            return 0;
        }

        int hash = 17;
        foreach (var item in _array)
        {
            hash = (hash * 31) + (item?.GetHashCode() ?? 0);
        }

        return hash;
    }

    public IEnumerator<T> GetEnumerator()
    {
        if (_array.IsDefault)
        {
            yield break;
        }

        foreach (var item in _array)
        {
            yield return item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
