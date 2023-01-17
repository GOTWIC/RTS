using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MinimapTeleporter : MonoBehaviour
{
    [SerializeField] private GameObject floor = null;
    [SerializeField] private Canvas canvas = null;

    private Vector3 mainCamPos;
    private Rect newRect;

    void Update()
    {
        Vector3[] corners = new Vector3[4];
        GetComponent<RawImage>().rectTransform.GetWorldCorners(corners);
        newRect = new Rect(corners[0], corners[2] - corners[0]);
        mainCamPos = Camera.main.transform.position;
        // Mouse.current.leftButton.wasPressedThisFrame &&
        if (Mouse.current.leftButton.wasPressedThisFrame && newRect.Contains(Input.mousePosition))
        {
            float canvasScale = canvas.transform.localScale.x;
            float canvasMultiplier = 1 / canvasScale;

            Vector3 scaledMousePos = new Vector3(Input.mousePosition.x * canvasMultiplier, Input.mousePosition.y * canvasMultiplier, 0);

            Vector3 adjustedMousePos = new Vector3(scaledMousePos.x - 2.5f, scaledMousePos.y - 2.5f, 0);

            //Vector3 miniMapPosition = new Vector3(Input.mousePosition.x - borderWidth, Input.mousePosition.y - borderWidth, Input.mousePosition.z);
            
            Debug.Log(adjustedMousePos);

            float xScale = floor.transform.localScale.x / 201f;
            float zScale = floor.transform.localScale.z / 201f;
            float xShift = floor.transform.localScale.x / 2f;
            float zShift = floor.transform.localScale.z / 2f;

            Vector3 realtimePosition = new Vector3(-(adjustedMousePos.x * xScale - xShift), 0, -(adjustedMousePos.y * zScale - zShift));
            Debug.Log(realtimePosition);
            Camera.main.transform.position = new Vector3(realtimePosition.x, mainCamPos.y, realtimePosition.z);
            
            //Debug.Log("");
        }
    }
}
