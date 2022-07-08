using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{

    private GameObject parent;

    void Start () {
        parent = transform.parent.gameObject;
    }

    public void UpdateMetric(STATUS_TYPES statusType, Color color, int amount)
    {
        if (statusType == STATUS_TYPES.HEALTH) {
            GameObject health = transform.GetChild(0).gameObject;
            float result = amount * 0.13f;
            health.transform.localScale = new Vector2(result,result);   
            health.GetComponent<SpriteRenderer>().color = color;

        }  
    }

    /*
    void Update() {
        if (parent.transform.hasChanged == true) {
            //transform.rotation = Quaternion.Euler(0,0,0);
            //transform.position = new Vector2(parent.transform.position.x, parent.transform.position.y);
            //transform.hasChanged = false;
        }
    }
    */


}
