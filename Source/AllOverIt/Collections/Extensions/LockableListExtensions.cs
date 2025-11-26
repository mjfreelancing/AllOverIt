using AllOverIt.Assertion;

namespace AllOverIt.Collections.Extensions
{
    /// <summary>Contains extension methods for <see cref="ILockableList{TType}"/>.</summary>
    public static class LockableListExtensions
    {
        /// <summary>Locks the collection to create, and return, a snapshot of the list's current contents.</summary>
        /// <returns>A snapshot of the list's current contents.</returns>
        public static List<TType> Clone<TType>(this ILockableList<TType> lockableList)
        {
            _ = lockableList.WhenNotNull();

            using (lockableList.GetReadLock(false))
            {
                return [.. lockableList];
            }
        }

        /// <summary>Adds the elements of the specified items to the end of the list.</summary>
        /// <typeparam name="TType">The element type.</typeparam>
        /// <param name="lockableList">The lockable list to append the items to.</param>
        /// <param name="items">The collection of items to be added to the lockable list.</param>
        public static void AddRange<TType>(this ILockableList<TType> lockableList, IEnumerable<TType> items)
        {
            _ = lockableList.WhenNotNull();
            _ = items.WhenNotNull();

            if (lockableList is LockableList<TType> lockableList2)
            {
                if (lockableList2._list is List<TType> list)
                {
                    using (lockableList.GetWriteLock())
                    {
                        list.AddRange(items);
                    }

                    return;
                }
            }

            using (lockableList.GetWriteLock())
            {
                foreach (var item in items)
                {
                    lockableList.Add(item);
                }
            }
        }
    }
}