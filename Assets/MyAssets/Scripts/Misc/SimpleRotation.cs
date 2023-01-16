using UnityEngine;

public class SimpleRotation : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 10f;

    void Update()
    {
        transform.RotateAround(transform.parent.position, new Vector3(0, 1, 0), rotationSpeed * Time.deltaTime);
    }

    // Hello
}
