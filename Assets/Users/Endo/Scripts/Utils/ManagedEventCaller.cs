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
            var component = o.GetComponent<IManagedMethod>();

            component?.ManagedStart();
        }

        // Update対象を格納
        foreach (GameObject o in managedUpdateObjects)
        {
            var component = o.GetComponent<IManagedMethod>();

            if (component == null) continue;

            _managedUpdateObjects.Add(component);
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
