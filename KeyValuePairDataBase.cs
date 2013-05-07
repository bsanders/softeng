using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareEng
{
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
            List<KeyValuePair<String, String>> list = mapToList(_kvMap);
            _xmlLoader.saveToFile<KeyValuePair<String, String>>(path, list);
        }

        public void loadAndOverwriteFromFile(String path)
        {
            List<KeyValuePair<String, String>> list = null;
            _xmlLoader.loadToList<KeyValuePair<String, String>>(path, out list);
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
            _kvMap.Add(k, v);
        }


        private Dictionary<String, String> listToMap(List<KeyValuePair<String, String>> list)
        {
            return list.ToDictionary(x => x.Key, x => x.Value);
        }

        private List<KeyValuePair<String, String>> mapToList(Dictionary<String, String> map)
        {
            return map.ToList<KeyValuePair<String, String>>();
        }


    }
}
