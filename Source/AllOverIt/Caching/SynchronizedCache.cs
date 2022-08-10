using AllOverIt.Threading;
using AllOverIt.Threading.Extensions;
using System;
using System.Collections.Generic;

namespace AllOverIt.Caching
{
    public sealed class SynchronizedCache<TKey, TValue> : IDisposable
    {
        private IReadWriteLock _cacheLock = new ReadWriteLock();
        private readonly IDictionary<TKey, TValue> _innerCache = new Dictionary<TKey, TValue>();

        public int Count => _innerCache.Count;

        public TValue Get(TKey key)
        {
            using (_cacheLock.GetReadLock(false))
            {
                return _innerCache[key];
            }
        }

        public void Add(TKey key, TValue value)
        {
            using (_cacheLock.GetWriteLock())
            {
                _innerCache.Add(key, value);
            }
        }

        public bool Add(TKey key, TValue value, int milliseconds)
        {
            return Add(key, value, TimeSpan.FromMilliseconds(milliseconds));
        }

        public bool Add(TKey key, TValue value, TimeSpan timeout)
        {
            if (_cacheLock.TryEnterWriteLock(timeout))
            {
                try
                {
                    _innerCache.Add(key, value);
                }
                finally
                {
                    _cacheLock.ExitWriteLock();
                }

                return true;
            }

            return false;
        }

        public void AddOrUpdate(TKey key, TValue value, Func<TValue, TValue, bool> comparison)
        {
            AddOrUpdate(key, value, comparison, -1);
        }

        public void AddOrUpdate(TKey key, TValue value, Func<TValue, TValue, bool> comparison, int milliseconds)
        {
            AddOrUpdate(key, value, comparison, TimeSpan.FromMilliseconds(milliseconds));
        }

        public void AddOrUpdate(TKey key, TValue value, Func<TValue, TValue, bool> comparison, TimeSpan timeout)
        {
            if (_cacheLock.TryEnterReadLock(true, timeout))
            {
                try
                {
                    if (_innerCache.TryGetValue(key, out var oldValue))
                    {
                        // Update the cache if the old/new values are different
                        if (!comparison.Invoke(oldValue, value))
                        {
                            using (_cacheLock.GetWriteLock())
                            {
                                _innerCache[key] = value;
                            }
                        }
                    }
                    else
                    {
                        Add(key, value);
                    }
                }
                finally
                {
                    _cacheLock.ExitReadLock();
                }
            }
        }

        public void Delete(TKey key)
        {
            using (_cacheLock.GetWriteLock())
            {
                _innerCache.Remove(key);
            }
        }

        public void Dispose()
        {
            _innerCache.Clear();

            _cacheLock?.Dispose();
            _cacheLock = null;
        }
    }
}