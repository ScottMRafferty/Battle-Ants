using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Nest : MonoBehaviour
{

    [Header("Setup")]
    public GameObject ant;
    public GameObject antOriginal;
    public int ants = 40;
    public bool greenfootAnts = false;
    public Color color = new Color(1,1,1,1);

    private int food = 0;    
    private Text statusText;

    void Start()
    {
        
        // Add the ant(s)
        for (int x=0;x<ants;x++) {
            GameObject o = Instantiate(greenfootAnts? antOriginal : ant,transform.position, Quaternion.identity, transform);
            o.GetComponent<Ant>().nest = this;
        }

        // Handle to status text on UI bar (hardcoded references for quick example)
        statusText = GameObject.Find(greenfootAnts?"StatusA":"StatusB").GetComponent<Text>();
        statusText.color = color;
        UpdateStatus();

        // Color the semi-transparent circle inside nest so we have an easy visual indicator of ants home
        transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = color;

        // Adjust the scale of the nest so (roughly) the same size regardless of screen res.
        //transform.localScale = new Vector3((transform.localScale.x / Screen.width) * 800, (transform.localScale.y / Screen.width) * 800, 1f);

    }

    private void UpdateStatus() {
        statusText.text = "Food: " + food + ", Ants: " + ants;
    }

    public void DestroyAnt(GameObject gameObject) {
        Destroy(gameObject);
        ants--;
        UpdateStatus();
    }

    public int DropFood(int amount) {
        food += amount;
        UpdateStatus();
        return amount;
    }

    public int TakeFood(int amount) {
        amount = Mathf.Clamp(amount,0,food);
        food -= amount;
        UpdateStatus();
        return amount;
    }

}
