using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class FXVShield : MonoBehaviour
{
    public bool shieldActive = true;
    public float shieldActivationSpeed = 1.0f;
    private float shieldActivationRim = 0.2f;

    public float hitEffectDuration = 0.5f;

    public Light shieldLight;
    public Material hitMaterial;
    public Color hitColor;
	public bool autoHitPatternScale = true;

    private Color lightColor;

    private Material baseMaterial;
    private Material activationMaterial;
    public Material postprocessMaterial;
    public Material postprocessActivationMaterial;

    private Collider myCollider;
    private CommandBuffer cmdBuffer;
    private Renderer myRenderer;

    private float shieldActivationTime;
    private float shieldActivationDir;
    

    private int activationTimeProperty;
    private int shieldDirectionProperty;

    private float hitAttenuationBase = 1.0f;

    private int currentHitIndex = 1;

    void Awake()
    {
        myRenderer = GetComponent<Renderer>();
        activationTimeProperty = Shader.PropertyToID("_ActivationTime");
        shieldDirectionProperty = Shader.PropertyToID("_ShieldDirection");

        FXVShieldPostprocess shieldPostrocess = Camera.main.GetComponent<FXVShieldPostprocess>();
        if (shieldPostrocess)
            shieldPostrocess.AddShield(this);

        shieldActivationDir = 0.0f;

        if (shieldLight)
            lightColor = shieldLight.color;

        myCollider = transform.GetComponent<Collider>();
			
        if (shieldActive)
        {
            shieldActivationTime = 1.0f;
            myCollider.enabled = true;
        }
        else
        {
            shieldActivationTime = 0.0f;
            myCollider.enabled = false;
        }

        if (shieldLight)
            shieldLight.color = Color.Lerp(Color.black, lightColor, shieldActivationTime);

        SetMaterial(myRenderer.material);
        SetHitMaterial(hitMaterial);
    }

    void OnDestroy()
    {
        if (Camera.main)
        {
            FXVShieldPostprocess shieldPostrocess = Camera.main.GetComponent<FXVShieldPostprocess>();
            if (shieldPostrocess)
                shieldPostrocess.RemoveShield(this);
        }
    }

    public void SetHitMaterial(Material newMat)
    {
        hitMaterial = newMat;

        if (hitMaterial)
        {
            hitAttenuationBase = hitMaterial.GetFloat("_HitAttenuation");
        }
    }

    public void SetMaterial(Material newMat)
    {
        baseMaterial = new Material(newMat);
        baseMaterial.SetFloat(activationTimeProperty, 1.0f);

        postprocessMaterial = new Material(baseMaterial);
        List<string> keywords = new List<string>(postprocessMaterial.shaderKeywords);
        if (keywords.Contains("USE_REFRACTION"))
            keywords.Remove("USE_REFRACTION");
        if (keywords.Contains("ACTIVATION_EFFECT_ON"))
            keywords.Remove("ACTIVATION_EFFECT_ON");
        postprocessMaterial.shaderKeywords = keywords.ToArray();
        postprocessMaterial.SetVector(shieldDirectionProperty, new Vector4(1.0f, 0.0f, 0.0f, 0.0f));

        postprocessActivationMaterial = new Material(baseMaterial);
        keywords = new List<string>(postprocessActivationMaterial.shaderKeywords);
        if (keywords.Contains("USE_REFRACTION"))
            keywords.Remove("USE_REFRACTION");
        postprocessActivationMaterial.shaderKeywords = keywords.ToArray();
        postprocessActivationMaterial.SetVector(shieldDirectionProperty, new Vector4(1.0f, 0.0f, 0.0f, 0.0f));

        activationMaterial = new Material(baseMaterial);
        shieldActivationRim = activationMaterial.GetFloat("_ActivationRim");

        keywords = new List<string>(baseMaterial.shaderKeywords);
        if (keywords.Contains("ACTIVATION_EFFECT_ON"))
        {
            Debug.Log("remove ACTIVATION_EFFECT_ON");
            keywords.Remove("ACTIVATION_EFFECT_ON");
        }
        baseMaterial.shaderKeywords = keywords.ToArray();
        myRenderer.material = baseMaterial;

        SetShieldEffectDirection(new Vector3(1.0f, 0.0f, 0.0f));
    }

    void Update()
    {
        Debug.Log("shieldActivationDir " + shieldActivationDir);
        if (shieldActivationDir > 0.0f)
        {
            shieldActivationTime += shieldActivationSpeed * Time.deltaTime;
            if (shieldActivationTime >= 1.0f)
            {
                shieldActivationTime = 1.0f;
                shieldActivationDir = 0.0f;
                myRenderer.material = baseMaterial;
            }

            if (shieldLight)
                shieldLight.color = Color.Lerp(Color.black, lightColor, shieldActivationTime);
        }
        else if (shieldActivationDir < 0.0f)
        {
            shieldActivationTime -= shieldActivationSpeed * Time.deltaTime;
            if (shieldActivationTime <= -shieldActivationRim)
            {
                shieldActivationTime = -shieldActivationRim;
                shieldActivationDir = 0.0f;
                myRenderer.enabled = false;
                myRenderer.material = baseMaterial;
            }

            if (shieldLight)
                shieldLight.color = Color.Lerp(Color.black, lightColor, shieldActivationTime);
        }

        myRenderer.material.SetFloat(activationTimeProperty, shieldActivationTime);
        postprocessActivationMaterial.SetFloat(activationTimeProperty, shieldActivationTime);
    }

    public bool GetIsShieldActive()
    {
        return (shieldActivationTime == 1.0f) || (shieldActivationDir == 1.0f);
    }

    public bool GetIsDuringActivationAnim()
    {
        return shieldActivationDir != 0.0f;
    }

    public void SetShieldActive(bool active, bool animated = true)
    {
        if (animated)
        {
            shieldActivationDir = (active) ? 1.0f : -1.0f;
            if (activationMaterial)
            {
                activationMaterial.SetFloat("_ActivationRim", shieldActivationRim);
                activationMaterial.SetFloat(activationTimeProperty, shieldActivationTime);

                postprocessActivationMaterial.SetFloat("_ActivationRim", shieldActivationRim);
                postprocessActivationMaterial.SetFloat(activationTimeProperty, shieldActivationTime);

                myRenderer.material = activationMaterial;
            }

            if (active)
                myRenderer.enabled = true;
        }
        else
        {
            shieldActivationTime = (active) ? 1.0f : 0.0f;
            shieldActivationDir = 0.0f;
            myRenderer.enabled = active;
        }

        myCollider.enabled = active;
    }

    public void SetShieldEffectDirection(Vector3 dir)
    {
        Vector4 dir4 = new Vector4(dir.x, dir.y, dir.z, 0.0f);
        myRenderer.material.SetVector(shieldDirectionProperty, dir4);
        baseMaterial.SetVector(shieldDirectionProperty, dir4);
        activationMaterial.SetVector(shieldDirectionProperty, dir4);
        postprocessMaterial.SetVector(shieldDirectionProperty, dir4);
        postprocessActivationMaterial.SetVector(shieldDirectionProperty, dir4);
    }

    public void OnHit(Vector3 hitPos, float hitScale)
    {
        GameObject hitObject = new GameObject("hitFX");
        hitObject.transform.parent = transform;
        hitObject.transform.position = transform.position;
        hitObject.transform.localScale = Vector3.one;
        hitObject.transform.rotation = transform.rotation;

        Vector3 hitLocalSpace = transform.InverseTransformPoint(hitPos);

        Vector3 dir = hitLocalSpace.normalized;
        Vector3 tan1 = Vector3.up - dir * Vector3.Dot(dir, Vector3.up);
        tan1.Normalize();
        Vector3 tan2 = Vector3.Cross(dir, tan1);

        MeshRenderer mr = hitObject.AddComponent<MeshRenderer>();
        MeshFilter mf = hitObject.AddComponent<MeshFilter>();

        mf.mesh = gameObject.GetComponent<MeshFilter>().mesh;
        mr.material = new Material(hitMaterial);

        mr.material.SetVector("_HitPos", hitLocalSpace);
        mr.material.SetVector("_HitTan1", tan1);
        mr.material.SetVector("_HitTan2", tan2);
        mr.material.SetFloat("_HitRadius", hitScale);
        mr.material.SetVector("_WorldScale", transform.lossyScale);
        mr.material.SetFloat("_HitShieldCovering", 1.0f);
        mr.material.renderQueue = mr.material.renderQueue + currentHitIndex;

        if (autoHitPatternScale)
        {
            if (myRenderer.material.HasProperty("_PatternScale"))
                mr.material.SetFloat("_PatternScale", myRenderer.material.GetFloat("_PatternScale"));
            else
                autoHitPatternScale = false;
        }
        mr.material.color = hitColor;

        FXVShieldHit hit = hitObject.AddComponent<FXVShieldHit>();
        hit.StartHitFX(hitEffectDuration);

        currentHitIndex++;
        if (currentHitIndex > 100)
            currentHitIndex = 1;
    }

    public Material GetPostprocessMaterial()
    {
        if (GetIsDuringActivationAnim())
            return postprocessActivationMaterial;

        return postprocessMaterial;
    }

    public Renderer GetRenderer()
    {
        return myRenderer;
    }
}