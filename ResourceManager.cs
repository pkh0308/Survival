using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

using Object = UnityEngine.Object;

public class ResourceManager : MonoBehaviour
{
    Dictionary<string, Object> resources = new Dictionary<string, Object>();
    Dictionary<string, AsyncOperationHandle> handles = new Dictionary<string, AsyncOperationHandle>();
    public int OnGoingHandles { get; private set; }

    #region �̱��� ����
    public static ResourceManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }
    #endregion

    #region ���ҽ� �ε�
    // ���ҽ� �ε� �Լ�
    // key ���ҽ��� �����Ѵٸ� �ٷ� ����, �ε����̶�� Completed�� �߰�
    // ó�� ��û�ƴٸ� �ε� ����
    public void GetResource<T>(string key, Action<T> callback) where T : Object
    {
        if (resources.TryGetValue(key, out Object resource))
        {
            callback?.Invoke(resource as T);
            return;
        }

        if (handles.TryGetValue(key, out AsyncOperationHandle handle))
        {
            handle.Completed += (op) => { callback?.Invoke(op.Result as T); };
            return;
        }

        handles.Add(key, Addressables.LoadAssetAsync<T>(key));
        OnGoingHandles++;
        handles[key].Completed += (op) =>
        {
            resources.Add(key, op.Result as T);
            callback?.Invoke(op.Result as T);
            OnGoingHandles--;
        };
    }

    public void GetResourceByIdx<T>(string key, int idx, Action<T, int> callback) where T : Object
    {
        if (resources.TryGetValue(key, out Object resource))
        {
            callback?.Invoke(resource as T, idx);
            return;
        }

        if (handles.TryGetValue(key, out AsyncOperationHandle handle))
        {
            handle.Completed += (op) => { callback?.Invoke(op.Result as T, idx); };
            return;
        }

        handles.Add(key, Addressables.LoadAssetAsync<T>(key));
        OnGoingHandles++;
        handles[key].Completed += (op) =>
        {
            resources.Add(key, op.Result as T);
            callback?.Invoke(op.Result as T, idx);
            OnGoingHandles--;
        };
    }
    #endregion
}
