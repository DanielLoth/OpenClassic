using System.Collections.Generic;
using System.Diagnostics;

namespace OpenClassic.Server.Collections
{
    public class EntityCollection<T>
    {
        private static ISet<T> empty = new HashSet<T>();
        private static ISet<T> emptyReadOnly = new ReadOnlySet<T>(empty);

        private readonly ISet<T> added = new HashSet<T>();
        private readonly ISet<T> known = new HashSet<T>();
        private readonly ISet<T> removed = new HashSet<T>();

        private readonly ISet<T> addedReadOnly;
        private readonly ISet<T> knownReadOnly;
        private readonly ISet<T> removedReadOnly;

        public EntityCollection()
        {
            addedReadOnly = new ReadOnlySet<T>(added);
            knownReadOnly = new ReadOnlySet<T>(known);
            removedReadOnly = new ReadOnlySet<T>(removed);
        }

        public ISet<T> AddedReadOnly => addedReadOnly;
        public ISet<T> KnownReadOnly => knownReadOnly;
        public ISet<T> RemovedReadOnly => removedReadOnly;

        public ISet<T> All
        {
            get
            {
                if (added.Count == 0 && known.Count == 0)
                {
                    return emptyReadOnly;
                }

                var results = new HashSet<T>();

                foreach (var entity in added)
                {
                    results.Add(entity);
                }

                foreach (var entity in known)
                {
                    results.Add(entity);
                }

                return results;
            }
        }

        public void Add(T entity)
        {
            Debug.Assert(!ReferenceEquals(entity, null));

            added.Add(entity);
        }

        public void Add(IEnumerable<T> entities)
        {
            Debug.Assert(entities != null);

            foreach (var entity in entities)
            {
                Debug.Assert(!ReferenceEquals(entity, null));

                added.Add(entity);
            }
        }

        public bool Contains(T entity)
        {
            Debug.Assert(!ReferenceEquals(entity, null));

            return known.Contains(entity) || added.Contains(entity);
        }


        public void Remove(T entity)
        {
            Debug.Assert(!ReferenceEquals(entity, null));

            removed.Add(entity);
        }

        public bool Removing(T entity)
        {
            Debug.Assert(!ReferenceEquals(entity, null));

            return removed.Contains(entity);
        }

        public bool Changed => !(added.Count == 0 && removed.Count == 0);

        public void Update()
        {
            if (removed.Count > 0)
            {
                foreach (var entity in removed)
                {
                    known.Remove(entity);
                }

                removed.Clear();
            }

            if (added.Count > 0)
            {
                foreach (var entity in added)
                {
                    known.Add(entity);
                }

                added.Clear();
            }
        }
    }
}
