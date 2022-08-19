using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUIDisplay : MonoBehaviour
{
    private Camera mainCamera;
    private Quaternion defaultRotation;

    [SerializeField]

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        defaultRotation = Quaternion.identity;  
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = defaultRotation * Quaternion.Euler(Vector3.right * mainCamera.transform.rotation.x);
    }

    private void OnMouseEnter()
    {
        //gameObject.SetActive(true);
    }

    private void OnMouseExit()
    {
        //gameObject.SetActive(false);
    }
}
