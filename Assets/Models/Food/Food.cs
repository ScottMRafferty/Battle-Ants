using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{

    [Header("Options")]
    public int qty = 200;
    public int qtyMax = 200;
    
    private Material material;
    private Material materialShadow;

    void Start() {
        float r = Random.Range(0.4f, 0.5f);
        qty = qtyMax = (int) Mathf.Ceil(3 * (r * 100));
        //Debug.Log("Qty: " + qty + ", Qty Max: " + qtyMax + ", r: " + r);
        transform.localScale = new Vector2(r,r);
        transform.rotation = new Quaternion(0,0,Random.Range(-1.0f, 1.0f),1.0f);
        material = GetComponent<Renderer>().material;
        GameObject shadow = transform.GetChild(0).gameObject;
        materialShadow =  shadow.GetComponent<Renderer>().material;

        // Skew and position shadow (relative to world position)
        Vector2 coords = Camera.main.WorldToViewportPoint(transform.position);
        coords.x += 0.01f;
        coords.y -= 0.03f;
        shadow.transform.position = Camera.main.ViewportToWorldPoint(coords);
        //shadow.transform.localScale = new Vector2(shadow.transform.localScale.x, shadow.transform.localScale.y + 0.3f);

        //transform.localScale = new Vector3((transform.localScale.x / Screen.width) * 800, (transform.localScale.y / Screen.width) * 800, 1f);
        
    }

    public int takeFood(int attemptingToTake) {
        if (qty < attemptingToTake)
            attemptingToTake = qty;
        this.qty -= attemptingToTake;
        return attemptingToTake;
    }

    public int TakeFood(int requestedAmount) {
        if (requestedAmount > qty)
            requestedAmount = qty;
        qty -= requestedAmount;
        float fade = 1.0f - ((float) qty/qtyMax);
        material.SetFloat("_Fade", fade);
        materialShadow.SetFloat("_Fade", fade);
        return requestedAmount;
    }   

    void LateUpdate() {
        if (qty < 1)
            Destroy(gameObject);
    }

}
