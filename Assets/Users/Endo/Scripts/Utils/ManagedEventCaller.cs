using System.Collections.Generic;
using UnityEngine;

public interface IManagedMethod
{
    void ManagedStart();
    void ManagedUpdate();
}

public class ManagedEventCaller : MonoBehaviour
{
    [SerializeField, Header("Startの実行を制御するオブジェクト (実行させたい順に並び替える)")]
    private GameObject[] managedStartObjects;

    [SerializeField, Header("Updateの実行を制御するオブジェクト (実行させたい順に並び替える)")]
    private GameObject[] managedUpdateObjects;

    private List<IManagedMethod> _managedUpdateObjects;

    private void Start()
    {
        _managedUpdateObjects = new List<IManagedMethod>();

        // Startを順次実行
        foreach (GameObject o in managedStartObjects)
        {
            IManagedMethod[] components = o.GetComponents<IManagedMethod>();

            foreach (IManagedMethod managedMethod in components)
            {
                managedMethod.ManagedStart();
            }
        }

        // Update対象を格納
        foreach (GameObject o in managedUpdateObjects)
        {
            IManagedMethod[] components = o.GetComponents<IManagedMethod>();

            if (components.Length == 0) continue;

            _managedUpdateObjects.AddRange(components);
        }

        // 用済みとなるため、シリアライズ側の参照を解放
        managedStartObjects  = null;
        managedUpdateObjects = null;
    }

    private void Update()
    {
        foreach (IManagedMethod managedObject in _managedUpdateObjects)
        {
            managedObject.ManagedUpdate();
        }
    }
}
