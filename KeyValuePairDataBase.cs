/*
 * Change Log:
 * Julian Nguyen(5/7/13)
 * This class was made.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SoftwareEng
{
    /// By Julian Nguyen
    /// Edited: Julian Nguyen(5/7/13)
    /// <summary>
    /// A Serializable key value struct.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    [Serializable]
    [XmlType(TypeName = "KeyValue")]
    public struct KeyValue<K, V>
    {
        public K Key { get; set; }
        public V Value { get; set; }
    } // End of KeyValue.

    /// By Julian Nguyen
    /// Edited: Julian Nguyen(5/7/13)
    /// <summary>
    /// An ADT of key value pairs.
    /// </summary>
    class KeyValuePairDataBase
    {

        private XmlLoader _xmlLoader;
        private Dictionary<String, String> _kvMap;

        /// By Julian Nguyen
        /// Edited: Julian Nguyen(5/7/13)
        /// <summary>
        /// A new ADT of key value pairs
        /// </summary>
        public KeyValuePairDataBase()
        {
            _xmlLoader = new XmlLoader();
            _kvMap = new Dictionary<string, string>();
        }
        /// By Julian Nguyen
        /// Edited: Julian Nguyen(5/7/13)
        /// <summary>
        /// Load the ADT of key value pairs from file.
        /// </summary>
        /// <param name="path"></param>
        public KeyValuePairDataBase(String path)
        {
            _xmlLoader = new XmlLoader();
            loadAndOverwriteFromFile(path);
        }

        /// By Julian Nguyen
        /// Edited: Julian Nguyen(5/7/13)
        /// <summary>
        /// Will save this ADT to file.
        /// </summary>
        /// <param name="path">The path where the file will be saved.</param>
        public void saveToFile(String path)
        {
            List<KeyValue<String, String>> list = mapToList(_kvMap);
            _xmlLoader.saveToFile<KeyValue<String, String>>(path, list);
        }

        /// By Julian Nguyen
        /// Edited: Julian Nguyen(5/7/13)
        /// <summary>
        /// Will load a key value file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        public void loadAndOverwriteFromFile(String path)
        {
            List<KeyValue<String, String>> list = null;
            _xmlLoader.loadToList<KeyValue<String, String>>(path, out list);
            _kvMap = listToMap(list);
        }

        /// By Julian Nguyen
        /// Edited: Julian Nguyen(5/7/13)
        /// <summary>
        /// To get the value for a key.
        /// </summary>
        /// <param name="k">The key</param>
        /// <returns>The value for that key.</returns>
        public String getValue(String k)
        {
            String v = null;
            if (_kvMap.TryGetValue(k, out v))
                return v;
            return null;
        }

        /// By Julian Nguyen
        /// Edited: Julian Nguyen(5/7/13)
        /// <summary>
        /// This will set or over write an key value pair.
        /// </summary>
        /// <param name="k">The key.</param>
        /// <param name="v">The value.</param>
        public void setKeyValuePair(String k, String v)
        {
            if (_kvMap.ContainsKey(k))
            {
                _kvMap.Remove(k);
            }

            _kvMap.Add(k, v);
        }

        /// By Julian Nguyen
        /// Edited: Julian Nguyen(5/7/13)
        /// <summary>
        /// Will take a list and return a dictienary.
        /// </summary>
        /// <param name="list">The list that will become the dictienary.</param>
        /// <returns>The dictionary form of the list. </returns>
        private Dictionary<String, String> listToMap(List<KeyValue<String, String>> list)
        {
            return list.ToDictionary(x => x.Key, x => x.Value);
        }

        /// By Julian Nguyen
        /// Edited: Julian Nguyen(5/7/13)
        /// <summary>
        /// This will take a Dictionary and return a list of KeyValue.
        /// </summary>
        /// <param name="map">The dictionary to become a list.</param>
        /// <returns>The list form of the dictionary.</returns>
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


    } // End of KeyValuePairDataBase.
}
