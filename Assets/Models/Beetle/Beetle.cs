using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beetle : MonoBehaviour
{
    public GameObject decal;
    private CreatureEnhanced creature;
    public float intervalDecal = 1.0f;
    private float elapsedTime;

    void Start()
    {
        creature = gameObject.GetComponent<CreatureEnhanced>();
        //transform.localScale = new Vector3((transform.localScale.x / Screen.width) * 800, (transform.localScale.y / Screen.width) * 800, 1f);
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= World.instance.simulationSpeed * intervalDecal) {
            elapsedTime = 0;
            Instantiate(decal,transform.position, Quaternion.identity,transform.parent.transform);
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.name == "Ant(Clone)" && collision.gameObject != null) {
            Ant ant = collision.gameObject.GetComponent<Ant>();
            ant.Hit();
        }
    }

}
