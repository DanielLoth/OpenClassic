using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace OpenClassic.Server.Collections
{
    public class ReadOnlySet<T> : ISet<T>
    {
        private readonly ISet<T> deleg;

        public ReadOnlySet(ISet<T> deleg)
        {
            Debug.Assert(deleg != null);

            this.deleg = deleg;
        }

        public int Count => deleg.Count;

        public bool IsReadOnly => true;

        public bool Add(T item)
        {
            throw new InvalidOperationException();
        }

        public void Clear()
        {
            throw new InvalidOperationException();
        }

        public bool Contains(T item) => deleg.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => deleg.CopyTo(array, arrayIndex);

        public void ExceptWith(IEnumerable<T> other)
        {
            throw new InvalidOperationException();
        }

        public IEnumerator<T> GetEnumerator() => deleg.GetEnumerator();

        public void IntersectWith(IEnumerable<T> other)
        {
            throw new InvalidOperationException();
        }

        public bool IsProperSubsetOf(IEnumerable<T> other) => deleg.IsProperSubsetOf(other);

        public bool IsProperSupersetOf(IEnumerable<T> other) => deleg.IsProperSupersetOf(other);

        public bool IsSubsetOf(IEnumerable<T> other) => deleg.IsSubsetOf(other);

        public bool IsSupersetOf(IEnumerable<T> other) => deleg.IsSupersetOf(other);

        public bool Overlaps(IEnumerable<T> other) => deleg.Overlaps(other);

        public bool Remove(T item)
        {
            throw new InvalidOperationException();
        }

        public bool SetEquals(IEnumerable<T> other) => deleg.SetEquals(other);

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new InvalidOperationException();
        }

        public void UnionWith(IEnumerable<T> other)
        {
            throw new InvalidOperationException();
        }

        void ICollection<T>.Add(T item)
        {
            throw new InvalidOperationException();
        }

        IEnumerator IEnumerable.GetEnumerator() => deleg.GetEnumerator();
    }
}
