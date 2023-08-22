using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PooledCustomFactory<T> : MonoBehaviour, IPooledCustomFactory<T> where T : class, IPoolable<T>
{
    [SerializeField] private ObjectIdPairContainer<StringID, GameObject> _componentsSettings = null;

    private Dictionary<string, List<T>> _componentsPool = new Dictionary<string, List<T>>();

    public T GetComponentByID(string slotInfoFillType)
    {
        T res = null;
        if (_componentsPool.TryGetValue(slotInfoFillType, out List<T> components) && components != null && components.Count > 0)
        {
            res = components[components.Count - 1];
            components.RemoveAt(components.Count - 1);
        }
        else
        {
            res = CreateComponent(slotInfoFillType);
        }

        return res;
    }

    public void ReleaseComponent(T component)
    {
        if (!_componentsPool.ContainsKey(component.ID) || _componentsPool[component.ID] == null)
            _componentsPool[component.ID] = new List<T>();

        _componentsPool[component.ID].Add(component);
        component.transform.gameObject.SetActive(false);
        component.transform.SetParent(this.transform);
    }

    protected virtual T CreateComponent(string id)
    {
        GameObject go = _componentsSettings.GetObject(id);

        if (go == default)
        {
            if(id != null && id != "")
                Debug.LogError($"[{nameof(PooledCustomFactory<T>)}] Try to instance non exists component with ID [{id}]");

            return null;
        }

        return Instantiate(go, this.transform).GetComponent<T>();
    }
}