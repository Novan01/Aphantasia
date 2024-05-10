using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarManager : MonoBehaviour
{
    public GameObject targetCamera;
    // Start is called before the first frame update
    void Start()
    {
        targetCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(targetCamera.transform);
    }
}
