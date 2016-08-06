using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Utils.Collections
{
    [DataContract]
    public class MemberChangeNotifyingList<T> : IList<T>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        [DataMember]
        List<T> List { get; set; }

        public MemberChangeNotifyingList()
        {
            List = new List<T>();
        }

        public MemberChangeNotifyingList(int capacity)
        {
            List = new List<T>(capacity);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return List.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            List.Add(item);
            HookPropertyChanged(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public void Clear()
        {
            var allValues = Values.ToArray();
            List.Clear();
            foreach (var value in allValues)
            {
                UnhookPropertyChanged(value);
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(T item)
        {
            return List.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            List.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            var removed = List.Remove(item);
            UnhookPropertyChanged(item);
            if (removed)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            }
            return removed;
        }

        public int Count => List.Count;
        public bool IsReadOnly => ((IList<T>)List).IsReadOnly;
        public int IndexOf(T item)
        {

            return List.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            List.Insert(index, item);
            HookPropertyChanged(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public void RemoveAt(int index)
        {
            if (index < List.Count)
            {
                var value = List[index];
                List.RemoveAt(index);
                UnhookPropertyChanged(value);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value));
            }
            else
            {
                // preserving default List behavior
                List.RemoveAt(index);
            }
        }

        public T this[int index]
        {
            get { return List[index]; }
            set
            {
                List[index] = value;
                HookPropertyChanged(value);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
            }
        }

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

        public IEnumerable<T> Values => List;

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