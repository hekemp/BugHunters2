using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class FXVShieldPostprocess : MonoBehaviour
{
    public float postprocessPower = 3.2f;

    private RenderTexture screenTex = null;

	private Camera thisCamera;

    private Material blendAddMaterial;
    private Material blurHorizontalMaterial;
    private Material blurVerticalMaterial;

    private List<FXVShield> shieldObjects = new List<FXVShield>();

    private Color clearToTransparentColor;

    private CommandBuffer cmdBuffer;

    void Awake()
	{
        blendAddMaterial = new Material(Shader.Find("FXV/FXVPostprocessAdd"));
        blendAddMaterial.SetFloat("_ColorMultiplier", postprocessPower);

        blurHorizontalMaterial = new Material(Shader.Find("FXV/FXVPostprocessBlurH"));
        blurVerticalMaterial = new Material(Shader.Find("FXV/FXVPostprocessBlurV"));

        clearToTransparentColor = new Color (0.0f, 0.0f, 0.0f, 0.0f);

        if (Screen.width > 0)
        {
            screenTex = new RenderTexture(Screen.width, Screen.height, 24);
            screenTex.wrapMode = TextureWrapMode.Clamp;
            screenTex.Create();

            {
                RenderTexture currentRT = RenderTexture.active;
                RenderTexture.active = screenTex;
                GL.Clear(true, true, clearToTransparentColor);
                RenderTexture.active = currentRT;
            }
        }

		thisCamera = GetComponent<Camera> ();

        cmdBuffer = new CommandBuffer();
        cmdBuffer.name = "FXVShieldBlur";
    }

    void Start () 
	{

    }

    public void AddShield(FXVShield s)
    {
        shieldObjects.Add(s);
    }

    public void RemoveShield(FXVShield s)
    {
        shieldObjects.Remove(s);
    }

    void OnEnable()
    {
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (screenTex == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        if (cmdBuffer == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        var rtW = screenTex.width;
        var rtH = screenTex.height;
        var rtW2 = screenTex.width / 2;
        var rtH2 = screenTex.height / 2;
        //var rtW4 = screenTex.width / 4;
        //var rtH4 = screenTex.height / 4;


        cmdBuffer.Clear();

        cmdBuffer.SetRenderTarget(new RenderTargetIdentifier(screenTex), BuiltinRenderTextureType.CurrentActive);

        cmdBuffer.ClearRenderTarget(false, true, clearToTransparentColor);

        foreach (FXVShield s in shieldObjects)
        {
            if (s.GetRenderer().enabled && s.gameObject.activeSelf && s.gameObject.activeInHierarchy)
            {
                cmdBuffer.DrawRenderer(s.GetRenderer(), s.GetPostprocessMaterial());
            }
        }

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = screenTex;
        Graphics.ExecuteCommandBuffer(cmdBuffer);
        RenderTexture.active = currentRT;

        RenderTexture tmpGlow = RenderTexture.GetTemporary(rtW2, rtH2, 0, RenderTextureFormat.Default);
        RenderTexture tmpGlow2 = RenderTexture.GetTemporary(rtW2, rtH2, 0, RenderTextureFormat.Default);

        Graphics.Blit(screenTex, tmpGlow, blurHorizontalMaterial);
        Graphics.Blit(tmpGlow, tmpGlow2, blurVerticalMaterial);

        Graphics.Blit(tmpGlow2, tmpGlow, blurHorizontalMaterial);
        Graphics.Blit(tmpGlow, tmpGlow2, blurVerticalMaterial);

        Graphics.Blit(tmpGlow2, tmpGlow, blurHorizontalMaterial);
        Graphics.Blit(tmpGlow, tmpGlow2, blurVerticalMaterial);

        Graphics.Blit(tmpGlow2, tmpGlow, blurHorizontalMaterial);
        Graphics.Blit(tmpGlow, tmpGlow2, blurVerticalMaterial);

        tmpGlow.DiscardContents();
        RenderTexture.ReleaseTemporary(tmpGlow);

        blendAddMaterial.SetTexture("_BlurTex", tmpGlow2);
        blendAddMaterial.SetTexture("_MainTex", source);

        Graphics.Blit(source, destination, blendAddMaterial);

        tmpGlow2.DiscardContents();
        RenderTexture.ReleaseTemporary(tmpGlow2);
    }
}
