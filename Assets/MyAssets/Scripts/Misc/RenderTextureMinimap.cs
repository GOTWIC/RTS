using UnityEngine;

public class RenderTextureMinimap : MonoBehaviour
{
    [SerializeField] RenderTexture rt = null;
    [SerializeField] Camera minimapCamera = null;

    // Start is called before the first frame update
    void Start()
    {
        minimapCamera.targetTexture = rt;
    }
}
