using System.Collections.Generic;
using UnityEngine;

public interface IIDGetter
{
    string ID { get; }
}


[System.Serializable]
public class StringID: IIDGetter
{
    [SerializeField]
    private string      id = "";
    public string ID => id;
}

[System.Serializable]
    public class ObjectIDPair<TKey, T> where TKey: IIDGetter
    {
        [SerializeField]
        private TKey      id;
        public TKey ID => id;
        [SerializeField]
        private T         obj;
        public T Obj => obj;

        public bool Check(TKey id)
        {
            return this.id.ID == id.ID;
        }
    }

    [System.Serializable]
    public class ObjectIdPairContainer<TKey, T> where TKey: IIDGetter
    {
        [SerializeField]
        private List<ObjectIDPair<TKey, T>>   objectIds = null;

        private List<string> keys = null;
        public List<string> Keys
        {
            get
            {
                if(keys == null)
                {
                    keys = new List<string>();
                    foreach(var k in objectIds)
                    {
                        keys.Add(k.ID.ID);
                    }
                }
                return keys;
            }
        }

        private List<T> values = null;
        public List<T> Values
        {
            get
            {
                if(values == null)
                {
                    values = new List<T>();
                    foreach(var v in ObjectsDict.Values)
                    {
                        values.Add(v);
                    }
                }

                return values;
            }
        }

        private Dictionary<string, T> objIdDict = null;
        private Dictionary<string, T> ObjectsDict
        {
            get
            {
                if(objIdDict == null)
                {
                    objIdDict = new Dictionary<string, T>();
                    foreach(ObjectIDPair<TKey, T> id in objectIds)
                    {
                        if(id != null)
                        {
                            objIdDict[id.ID.ID] = id.Obj;
                        }
                    }
                }

                return objIdDict;
            }
        }

        private Dictionary<T, string> idObjectsDict = null;
        private Dictionary<T, string> IdDict
        {
            get
            {
                if(idObjectsDict == null)
                {
                    idObjectsDict = new Dictionary<T, string>();
                    foreach(ObjectIDPair<TKey, T> id in objectIds)
                    {
                        if(id != null)
                        {
                            idObjectsDict[id.Obj] = id.ID.ID;
                        }
                    }
                }

                return idObjectsDict;
            }
        }

        public T GetObject(string id)
        {
            if(ObjectsDict.ContainsKey(id))
            {
                return ObjectsDict[id];
            }

            return default(T);
        }

        public string GetObjectID(T index)
        {
            if(IdDict.ContainsKey(index))
            {
                return IdDict[index];
            }
            return "";
        }

        public T this[string key]
        {
            get
            {
                return GetObject(key);
            }
        }
    }