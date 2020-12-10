using UnityEngine;


[ExecuteInEditMode]
public class DepthCamera : MonoBehaviour
{
    void Start()
    {
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
    }

}
