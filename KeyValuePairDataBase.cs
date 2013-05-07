using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SoftwareEng
{

    [Serializable]
    [XmlType(TypeName = "KeyValue")]
    public struct KeyValue<K, V>
    {
        public K Key { get; set; }
        public V Value { get; set; }
    }


    class KeyValuePairDataBase
    {

        private XmlLoader _xmlLoader;
        private Dictionary<String, String> _kvMap;


        public KeyValuePairDataBase()
        {
            _xmlLoader = new XmlLoader();
            _kvMap = new Dictionary<string, string>();
        }

        public KeyValuePairDataBase(String path)
        {
            _xmlLoader = new XmlLoader();
            loadAndOverwriteFromFile(path);
        }


        public void saveToFile(String path)
        {
            List<KeyValue<String, String>> list = mapToList(_kvMap);
            _xmlLoader.saveToFile<KeyValue<String, String>>(path, list);
        }

        public void loadAndOverwriteFromFile(String path)
        {
            List<KeyValue<String, String>> list = null;
            _xmlLoader.loadToList<KeyValue<String, String>>(path, out list);
            _kvMap = listToMap(list);
        }


        public String getValue(String k)
        {
            String v = null;
            if (_kvMap.TryGetValue(k, out v))
                return v;
            return null;
        }

        public void setKeyValuePair(String k, String v)
        {
            if (_kvMap.ContainsKey(k))
            {
                _kvMap.Remove(k);
            }

            _kvMap.Add(k, v);
        }


        private Dictionary<String, String> listToMap(List<KeyValue<String, String>> list)
        {
            return list.ToDictionary(x => x.Key, x => x.Value);
        }

        private List<KeyValue<String, String>> mapToList(Dictionary<String, String> map)
        {
            List<KeyValue<String, String>> list = new List<KeyValue<String, String>>();
            foreach (KeyValuePair<String, String> kv in map)
            {
                KeyValue<String, String> mykv = new KeyValue<string,string>();
                mykv.Key = kv.Key;
                mykv.Value = kv.Value;
                list.Add(mykv);
            }
            return list;
        }


    }
}
