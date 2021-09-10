using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeManager : MonoBehaviour
{
    public Vector3 ContentPos;

    [SerializeField]
    public GameObject Content;

    [SerializeField]
    public Vector3 MinPos;

    [SerializeField]
    public Vector3 MaxPos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ContentPosCheck()
    {
        ContentPos = Content.transform.position;
    }
}
