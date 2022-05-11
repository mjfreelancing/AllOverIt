using AllOverIt.Assertion;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AllOverIt.Diagnostics.Breadcrumbs
{
    /// <summary>Stores a collection of breadcrumb messages and metadata.</summary>
    public sealed class Breadcrumbs : IBreadcrumbs
    {
        private readonly IEnumerable<BreadcrumbData> _breadcrumbs;
        private Action<BreadcrumbData> _appender;

        /// <summary>Provides options that control how breadcrumb items are inserted and cached.</summary>
        public BreadcrumbsOptions Options { get; }

        /// <summary>The timestamp when breadcrumb collection begins.</summary>
        public DateTime StartTimestamp { get; } = DateTime.Now;

        /// <summary>Constructor.</summary>
        /// <param name="options">Provides options that control how breadcrumb items are inserted and cached.</param>
        public Breadcrumbs(BreadcrumbsOptions options = default)
        {
            Options = options ?? new BreadcrumbsOptions();

            _breadcrumbs = Options.ThreadSafe
                ? InitializeAsMultiThreadSafe()
                : InitializeAsMultiThreadUnsafe();
        }

        /// <inheritdoc />
        public IEnumerator<BreadcrumbData> GetEnumerator()
        {
            return _breadcrumbs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public IBreadcrumbs Add(BreadcrumbData breadcrumb)
        {
            _ = breadcrumb.WhenNotNull(nameof(breadcrumb));

            _appender.Invoke(breadcrumb);

            return this;
        }

        private IEnumerable<BreadcrumbData> InitializeAsMultiThreadSafe()
        {
            var queue = new ConcurrentQueue<BreadcrumbData>();

            _appender = breadcrumb => 
            {
                queue.Enqueue(breadcrumb);

                if (Options.MaxCapacity > 0)
                {
                    while (queue.Count > Options.MaxCapacity)
                    {
                        queue.TryDequeue(out _);
                    }
                }
            };

            return queue;
        }

        private IEnumerable<BreadcrumbData> InitializeAsMultiThreadUnsafe()
        {
            var list = new List<BreadcrumbData>();

            _appender = breadcrumb => 
            {
                list.Add(breadcrumb);

                if (Options.MaxCapacity > 0 && list.Count > Options.MaxCapacity)
                {
                    list.RemoveRange(0, list.Count - Options.MaxCapacity);
                }
            };

            return list;
        }
    }
}
