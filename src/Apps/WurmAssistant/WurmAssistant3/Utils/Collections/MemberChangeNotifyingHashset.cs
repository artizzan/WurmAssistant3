using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Tests.Unit.Utils.Collections
{
    public class MemberChangeNotifyingHashset<T> : ISet<T>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        public HashSet<T> HashSet { get; set; }

        public MemberChangeNotifyingHashset()
        {
            HashSet = new HashSet<T>();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return HashSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            ((ISet<T>)this).Add(item);
        }

        public void UnionWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return HashSet.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return HashSet.IsSupersetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return HashSet.IsProperSupersetOf(other);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return HashSet.IsProperSubsetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return HashSet.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return HashSet.SetEquals(other);
        }

        bool ISet<T>.Add(T item)
        {
            var added = HashSet.Add(item);
            if (added)
            {
                HookPropertyChanged(item);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            }
            return added;
        }

        public void Clear()
        {
            var allValues = Values.ToArray();
            HashSet.Clear();
            foreach (var value in allValues)
            {
                UnhookPropertyChanged(value);
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

        }

        public bool Contains(T item)
        {
            return HashSet.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            HashSet.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            var removed = HashSet.Remove(item);
            UnhookPropertyChanged(item);
            if (removed)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            }
            return removed;
        }

        public int Count => HashSet.Count;
        public bool IsReadOnly => ((ISet<T>) HashSet).IsReadOnly;

        void HookPropertyChanged(object item)
        {
            var observable = item as INotifyPropertyChanged;
            if (observable != null)
            {
                observable.PropertyChanged += OnObservableOnPropertyChanged;
            }
        }

        void UnhookPropertyChanged(object item)
        {
            var observable = item as INotifyPropertyChanged;
            if (observable != null)
            {
                observable.PropertyChanged -= OnObservableOnPropertyChanged;
            }
        }

        void OnObservableOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            this.OnPropertyChanged(nameof(Values));
        }

        public IEnumerable<T> Values => HashSet;

        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }
    }
}
