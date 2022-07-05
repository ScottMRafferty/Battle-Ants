using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{

    [Header("Options")]
    public int qty;

    void Start() {
        transform.rotation = new Quaternion(0,0,0,Random.Range(0,1));
        if (qty > 30) {
            float r = Random.Range(0.05f, 0.10f);
            transform.localScale = new Vector2(r,r);
        }
    }

    public int takeFood(int attemptingToTake) {
        if (qty < attemptingToTake)
            attemptingToTake = qty;
        this.qty -= attemptingToTake;
        return attemptingToTake;
    }

    public void setQuantity(int amount) {
        qty = amount;
    }

    void LateUpdate()
    {
        if (qty < 1) 
            Destroy(gameObject);
        else if (qty < 5)
            transform.localScale = new Vector2(0.003f,0.003f);
        else if (qty < 10)
            transform.localScale = new Vector2(0.03f,0.03f);
        else if (qty < 20)
            transform.localScale = new Vector2(0.04f,0.04f);
        
    }

    public int TakeFood(int requestedAmount) {
        if (requestedAmount > qty)
            requestedAmount = qty;
        qty -= requestedAmount;
        return requestedAmount;
    }   

}
