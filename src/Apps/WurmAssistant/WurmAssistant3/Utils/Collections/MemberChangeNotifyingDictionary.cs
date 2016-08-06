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
    public class MemberChangeNotifyingDictionary<TKey, TValue>
        : IDictionary<TKey, TValue>,
            INotifyPropertyChanged,
            INotifyCollectionChanged
    {
        [DataMember]
        Dictionary<TKey, TValue> Dictionary { get; set; }

        public MemberChangeNotifyingDictionary()
        {
            Dictionary = new Dictionary<TKey, TValue>();
        }

        public MemberChangeNotifyingDictionary(int capacity)
        {
            Dictionary = new Dictionary<TKey, TValue>(capacity);
        }
        
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Dictionary.Add(item.Key, item.Value);
            HookPropertyChanged(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public void Clear()
        {
            var allValues = Values.ToArray();
            Dictionary.Clear();
            foreach (var value in allValues)
            {
                UnhookPropertyChanged(value);
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return Dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((IDictionary<TKey, TValue>)Dictionary).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            var removed = ((IDictionary<TKey, TValue>)Dictionary).Remove(item);
            UnhookPropertyChanged(item.Value);
            if (removed)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            }
            return removed;
        }

        public int Count => Dictionary.Count;
        public bool IsReadOnly => ((IDictionary<TKey, TValue>)Dictionary).IsReadOnly;

        public bool ContainsKey(TKey key)
        {
            return Dictionary.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            Dictionary.Add(key, value);
            HookPropertyChanged(value);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
        }

        public bool Remove(TKey key)
        {
            TValue value;
            Dictionary.TryGetValue(key, out value);
            var removed = Dictionary.Remove(key);
            if (value != null)
            {
                UnhookPropertyChanged(value);
            }
            if (removed)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value));
            }
            return removed;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return Dictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get { return Dictionary[key]; }
            set
            {
                Dictionary[key] = value;
                HookPropertyChanged(value);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
            }
        }

        public ICollection<TKey> Keys => Dictionary.Keys;
        public ICollection<TValue> Values => Dictionary.Values;

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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }
    }
}
