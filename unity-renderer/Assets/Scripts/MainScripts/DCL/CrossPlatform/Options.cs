using DCL;
using UnityEngine;

public class Options : MonoBehaviour
{
    private bool cameraEnabled = true;
    public Shader[] shaders = new Shader[15];
    private int shaderCount = 0;
    public void QuitApplication()
    {
        Application.Quit();
    }
    public  void DisableCameras()
    {
        object[] Cams = GameObject.FindObjectsOfType(typeof(Camera));
        foreach (Camera C in Cams)
        {
            Debug.Log($"Camera {C.name} state was {C.enabled}.  Now set to False");
            C.enabled = false;

        }
        
    }
    public void SwapShaders()
    {
        shaderCount = (shaderCount + 1 ) % shaders.Length;
        Debug.Log($"Shaders Switched to {shaders[shaderCount].name}");
        Object[] Rends = GameObject.FindObjectsOfType(typeof(Renderer));
        foreach (var o in Rends)
        {
            var r = (Renderer)o;
            r.material.shader = shaders[shaderCount];

        }
        
    }
    public void PauseWebview()
    {
        DebugConfigComponent.i.PauseWebview();
    }
}
