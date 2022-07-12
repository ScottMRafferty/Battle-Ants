using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pheromone : MonoBehaviour
{
    public void Fire(Color color) {

        float sec = 9.0f - Mathf.Sqrt(World.instance.simulationSpeed);
        //Debug.Log("sec: " + sec + ", bla: " + Mathf.Sqrt(World.instance.simulationSpeed));

        GetComponent<SpriteRenderer>().color = color;
        LeanTween.color(gameObject, new Color(color.r, color.g, color.b, 0.0f), sec);
        LeanTween.scale(gameObject, new Vector3(0,0,0),sec)
        .setDelay(0.0f).setOnComplete(()=>{
            Destroy(gameObject);
        });
    }
}
