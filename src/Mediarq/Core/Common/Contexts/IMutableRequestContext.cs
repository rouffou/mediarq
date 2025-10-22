using Mediarq.Core.Common.Requests.Abstraction;

namespace Mediarq.Core.Common.Contexts;

/// <summary>
/// Represent an immutable request context. That contains information about the current request being processed.
/// </summary>
/// <typeparam name="TRequest">The type of the request that need to extends <see cref="ICommandOrQuery{TResponse}"/> interface.</typeparam>
/// <typeparam name="TResponse">The type of the responses must be <see cref="Results.Result"/> or <see cref="Results.Result{TValue}"/>.</typeparam>
public interface IIMMutableRequestContext<TRequest, TResponse> : IRequestContext<TRequest, TResponse>
    where TRequest : ICommandOrQuery<TResponse>
{
    /// <summary>
    /// Add an item with a key and a value to the context.
    /// </summary>
    /// <param name="key">The key to refers of the items.</param>
    /// <param name="value">The values of the items.</param>
    /// <exception cref="ArgumentException">Throw if <paramref name="key"/> is null or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Throw if <paramref name="value"/> is null.</exception>
    void AddItem(string key, object value);

    /// <summary>
    /// Attempts to retrieve the value associated with the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="key">The key whose associated value is to be retrieved. Cannot be null.</param>
    /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise,
    /// the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
    /// <returns><see langword="true"/> if the key was found and the value was retrieved successfully; otherwise, <see
    /// langword="false"/>.</returns>
    /// <exception cref="ArgumentException">Throw if <paramref name="key"/> is null or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Throw if the dictionnary are not init.</exception>
    bool TryGetItem<T>(string key, out T value);

    /// <summary>
    /// Removes the item with the specified key from the collection.
    /// </summary>
    /// <param name="key">The key of the item to remove. Cannot be null.</param>
    /// <returns>true if the item was successfully removed; otherwise, false. This method also returns false if the key was not
    /// found in the collection.</returns>
    bool RemoveItem(string key);
}
