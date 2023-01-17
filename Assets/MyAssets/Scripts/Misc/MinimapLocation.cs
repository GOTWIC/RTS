using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapLocation : MonoBehaviour
{
    [SerializeField] private GameObject minimapIcon = null;

    [SerializeField] private int offset = 0;

    private Vector3 baseLoc = new Vector3(0,0,0);

    private void Update()
    {
        baseLoc = gameObject.transform.position;
        minimapIcon.transform.position = new Vector3(baseLoc.x / 1000f, 1000.5f, (baseLoc.z + offset) / 1000f);
        Debug.Log(minimapIcon.transform.position);
    }
}
