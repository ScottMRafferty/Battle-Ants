using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{

    [Header("Options")]
    public int qty;

    void Start() {
        transform.rotation = new Quaternion(0,0,0,Random.Range(0,1));
        float r = Random.Range(0.03f, 0.10f);
        transform.localScale = new Vector2(r,r);
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

    // Update is called once per frame
    void Update()
    {
        if (qty < 1)
            Destroy(gameObject);
    }

    public int TakeFood(int requestedAmount) {
        if (requestedAmount > qty)
            requestedAmount = qty;
        qty -= requestedAmount;
        return requestedAmount;
    }   

}
