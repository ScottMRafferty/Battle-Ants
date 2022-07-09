using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pheromone : MonoBehaviour
{

    

    public void Fire(Color color) {
        GetComponent<SpriteRenderer>().color = color;
        LeanTween.color(gameObject, new Color(color.r, color.g, color.b, 0.0f), 5.0f);
        LeanTween.scale(gameObject, new Vector3(0,0,0),World.instance.simulationSpeed*200.0f)
        .setDelay(World.instance.simulationSpeed*10.0f).setOnComplete(()=>{
            Destroy(gameObject);
        });
    }

}
