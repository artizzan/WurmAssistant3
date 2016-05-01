using System.Collections.Generic;

namespace AldursLab.Essentials.Collections
{
    public class BidirectionalMap<TKey1, TKey2>
    {
        readonly Dictionary<TKey1, TKey2> tkey1ToTkey2 = new Dictionary<TKey1, TKey2>();
        readonly Dictionary<TKey2, TKey1> tkey2ToTkey1 = new Dictionary<TKey2, TKey1>();

        public void Add(TKey1 key1, TKey2 key2)
        {
            tkey1ToTkey2.Add(key1, key2);
            tkey2ToTkey1.Add(key2, key1);
        }

        public TKey2 GetByKey1(TKey1 key1)
        {
            return tkey1ToTkey2[key1];
        }

        public TKey1 GetByKey2(TKey2 key2)
        {
            return tkey2ToTkey1[key2];
        }

        public bool TryGetByKey1(TKey1 key1, out TKey2 key2)
        {
            return tkey1ToTkey2.TryGetValue(key1, out key2);
        }

        public bool TryGetByKey2(TKey2 key2, out TKey1 key1)
        {
            return tkey2ToTkey1.TryGetValue(key2, out key1);
        }

        public void Remove(TKey1 key1)
        {
            var key2 = tkey1ToTkey2[key1];
            tkey1ToTkey2.Remove(key1);
            tkey2ToTkey1.Remove(key2);
        }

        public void Remove(TKey2 key2)
        {
            var key1 = tkey2ToTkey1[key2];
            tkey2ToTkey1.Remove(key2);
            tkey1ToTkey2.Remove(key1);
        }
    }
}