using System.Collections.Generic;

namespace Dot.Core.Generic
{
    public interface IORMData<K>
    {
        K GetKey();
    }

    public class IndexMapORM<K, D> where D :class, IORMData<K>
    {
        private List<D> dataList = new List<D>();
        private Dictionary<K, D> dataDic = new Dictionary<K, D>();

        public D this[int index]
        {
            get
            {
                if (index >= 0 && index < dataList.Count)
                {
                    return dataList[index];
                }
                return default;
            }
        }

        public D this[K key]
        {
            get
            {
                if (dataDic.TryGetValue(key, out D data))
                {
                    return data;
                }
                return default;
            }
        }

        public int Count { get => dataList.Count; }

        public bool Contain(K key) => dataDic.ContainsKey(key);

        public bool TryGet(K key,out D data)
        {
            data = this[key];
            if(data == default(D))
            {
                return false;
            }
            return true;
        }

        public void PushData(D data)
        {
            dataList.Add(data);
            dataDic.Add(data.GetKey(), data);
        }

        public D PopData()
        {
            if (dataList.Count == 0)
            {
                return default;
            }

            D data = dataList[0];
            dataList.RemoveAt(0);
            dataDic.Remove(data.GetKey());

            return data;
        }

        public void DeleteData(D data)
        {
            dataList.Remove(data);
            dataDic.Remove(data.GetKey());
        }

        public void DeleteData(K key)
        {
            if(dataDic.TryGetValue(key,out D data))
            {
                DeleteData(data);
            }
        }

        public void Clear()
        {
            dataList.Clear();
            dataDic.Clear();
        }
    }
}
