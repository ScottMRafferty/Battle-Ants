using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// The available game types
public enum STATUS_TYPES {
    HEALTH=0
}

public class World : MonoBehaviour
{

    [Header("Setup")]
    public Sprite background;
    public GameObject nest;
    public GameObject food;

    [Header("Options")]
    public int foodQty = 18;
    public int nestQty = 3;
    public float simulationSpeed = 0.04f;
    public bool showAntHealth = false;

    private List<Vector3> coords = new List<Vector3>();
    public static World instance;
    

    void Start()
    {
        
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else {
            if (this != instance)
                Destroy(this.gameObject);
        }

        LeanTween.init( 800 );

        Color[] colors = new Color[3] {new Color(1f,0f,0f,1f),new Color(0f,.5f,0f,1f),new Color(0f,0f,1f,1f)};

        // Food
        for (int x=0; x<foodQty; x++) {
            Instantiate(food,Camera.main.ViewportToWorldPoint(getRandomDelta(false)), Quaternion.identity, transform);
        }

        // Nests (teams)
        for (int x=0; x<nestQty; x++) {
            GameObject o = Instantiate(nest,Camera.main.ViewportToWorldPoint(getRandomDelta(true)), Quaternion.identity, transform);
            o.GetComponent<Nest>().color = colors[x];
            o.GetComponent<Nest>().greenfootAnts = (x < 1);
        }

        SetRenderScale(0.1f);
        StartCoroutine(DoFadeIn());       

    }

    IEnumerator DoFadeIn()
        {
            UniversalRenderPipelineAsset asset = (UniversalRenderPipelineAsset) GraphicsSettings.renderPipelineAsset;
            while (asset.renderScale < 1.0f)
            {
                SetRenderScale(asset.renderScale += 0.05f);
                yield return new WaitForSeconds (0.1f);
            }
            yield return null;
        }

    void SetRenderScale(float scale) {
        UniversalRenderPipelineAsset asset = (UniversalRenderPipelineAsset) GraphicsSettings.renderPipelineAsset;
        asset.renderScale = scale;
        GraphicsSettings.renderPipelineAsset = asset;
    }

    Vector3 getRandomDelta(bool requiresSpace) {
        Vector3 v;
        int counter = 0;
        do {
            v = new Vector3(Mathf.Clamp(Random.value,0.01f,0.99f), Mathf.Clamp(Random.value,0.05f,0.95f),10.0f);
            counter++;
        } while (AlreadyAllocated(v) && counter < 50 && requiresSpace == true);
        coords.Add(v);
        //Debug.Log("Found counter: " + counter + ", x: " + v.x + ", y: " + v.y);
        return v;
    }

    bool AlreadyAllocated(Vector3 v) {
        for (int i=0;i<coords.Count;i++) {
            Vector3 c = coords[i];
            float distanceX = Mathf.Abs(c.x - v.x);
            float distanceY = Mathf.Abs(c.y - v.y);
            //Debug.Log("Vector " + i + ": " + distanceX + ", y: " + distanceY);
            if (distanceX < 0.05f || distanceY < 0.05f)
                return true;
            
        }
        return false;
    }

    public void SetSimulationSpeed(float value) {
        simulationSpeed = 0.2f - value;
    }

    public void SetShowAntHealth(bool value) {
        showAntHealth = value;
    }

}
