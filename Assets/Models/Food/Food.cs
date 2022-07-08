using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{

    [Header("Options")]
    public int qty = 50;
    public int qtyMax = 50;
    
    private Material material;
    private Material materialShadow;

    void Start() {
        float r = Random.Range(0.03f, 0.10f);
        qty = qtyMax = (int) Mathf.Ceil(10 * (r * 100));
        //Debug.Log("Qty: " + qty + ", Qty Max: " + qtyMax + ", r: " + r);
        transform.localScale = new Vector2(r,r);
        transform.rotation = new Quaternion(0,0,Random.Range(-1.0f, 1.0f),1.0f);
        material = GetComponent<Renderer>().material;
        materialShadow =  transform.GetChild(0).GetComponent<Renderer>().material;
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

}
