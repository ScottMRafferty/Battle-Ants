using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureWrapper {

    private Creature m1;
    private CreatureEnhanced m2;

    public CreatureWrapper(Creature m1, CreatureEnhanced m2) {
        this.m1 = m1;
        this.m2 = m2;
    }

    public void SetTarget(GameObject target, bool moveAwayFromTarget) {
        if (m1 != null)
            m1.SetTarget(target, moveAwayFromTarget);
        else
            m2.SetTarget(target, moveAwayFromTarget);
    }

    public void HeadRoughlyTowards(GameObject target) {
        if (m1 != null)
            m1.HeadRoughlyTowards(target);
        else
            m2.HeadRoughlyTowards(target.transform.position);
    }

}

public class Ant : MonoBehaviour
{

    [Header("Setup")]
    public Sprite ant;
    public Sprite antWithFood;
    public GameObject pheromone;
    public GameObject decal;
    public Status status;
    public float updateInterval = 1.0f;
    public bool showUnitStatus = false;
    public int carryingCapacity = 2;
    public int health = 5;

    private int food = 0;
    private float elapsedTime;

    private Ant enemyAnt;
    private int MAX_PH_LEVEL = 18;
    private int pheromoneLevel = 18;
    private GameObject lastPheromone;   // replacing PH_TIME and foundLastPheromone (decays to null relative to world time)
    
    public Nest nest;
    private CreatureWrapper creature;
    private GameObject leaf;


    void Start()
    {

        if (showUnitStatus)
            status = Instantiate(status,transform.position, Quaternion.identity, transform);

        creature = new CreatureWrapper(GetComponent<Creature>(), GetComponent<CreatureEnhanced>());       
        LeanTween.delayedCall(gameObject, Random.Range(1.0f,20.0f), ()=>{
            status.UpdateMetric(STATUS_TYPES.HEALTH, nest.color, health);
            status.gameObject.SetActive(false);
            gameObject.SetActive(true);
        });
        gameObject.SetActive(false);
        leaf = transform.GetChild(0).gameObject;

        // No re-scaling of the ant required as they are a child of nest (which is scaled appropriately)

    }

    void HandlePheromoneDrop(Color color)
    {
        if ((pheromoneLevel == MAX_PH_LEVEL) || enemyAnt != null ) {
            GameObject o = Instantiate(pheromone,transform.position, Quaternion.identity, nest.gameObject.transform.parent.transform);
            Instantiate(decal,transform.position, Quaternion.identity, nest.gameObject.transform.parent.transform);
            Pheromone p = o.GetComponent<Pheromone>();
            p.Fire(color);
            pheromoneLevel = 0;
        }
        else {
            pheromoneLevel++;
        }
    }

    private void SearchForFood()
    {   
        if (lastPheromone) {
            creature.SetTarget(nest.gameObject,true);
        } else {
            creature.SetTarget(null,true);
        }
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= updateInterval / World.instance.simulationSpeed) {

            elapsedTime = 0;

            if (enemyAnt != null) {
                enemyAnt.Hit();
                enemyAnt = null;
            } else if (food > 0 && nest != null) {
                creature.SetTarget(nest.gameObject,false);
                HandlePheromoneDrop(new Color(1,1,1,1));
            } else {
                SearchForFood();
            }

            if (health < 1) {
                if (food > 0 && enemyAnt && enemyAnt.food == 0) {
                    enemyAnt.food = food;
                    //GameObject foodDrop = Instantiate(World.instance.food,transform.position, Quaternion.identity, nest.gameObject.transform.parent.transform);
                    //foodDrop.GetComponent<Food>().qty = food;
                }
                nest.DestroyAnt(gameObject);
            }

            if (leaf != null)
                leaf.SetActive(food > 0);

            status.gameObject.SetActive(World.instance.showAntHealth);
           
        }

    }

    void OnCollisionEnter2D(Collision2D collision) {

        GameObject colGameObject = collision.gameObject;

        if (colGameObject != null) {
            if ((colGameObject.name == "Ant(Clone)" || colGameObject.name == "AntOriginal(Clone)")) {
                // Handle this scenario in OnCollisionStay2D
                OnCollisionStay2D(collision);

            } else if (colGameObject.name == "Nest(Clone)") {

                Nest nestInProximity  = colGameObject.GetComponent<Nest>();
                if (colGameObject.GetInstanceID() == nest.gameObject.GetInstanceID()) {
                    // Home nest - drop what we have and head away
                    if (food > 0) {
                        food -= nestInProximity.DropFood(food);
                        this.GetComponent<SpriteRenderer>().sprite = ant;
                        lastPheromone = null;
                        LeanTween.delayedCall(gameObject, Random.Range(0.0f,3.0f), ()=>gameObject.SetActive(true));
                        gameObject.SetActive(false);
                    }
                } else {
                    // Enemy nest -  only take what we can carry and then head to home nest
                    if (carryingCapacity > food) {
                        food += nestInProximity.TakeFood(carryingCapacity - food);
                        if (food > 0) {
                            this.GetComponent<SpriteRenderer>().sprite = antWithFood;
                            creature.SetTarget(nest.gameObject,false);
                        }
                        LeanTween.delayedCall(gameObject, Random.Range(0.0f,3.0f), ()=>gameObject.SetActive(true));
                        gameObject.SetActive(false);
                    }
                } 

            } else if (colGameObject.name == "Food(Clone)" && food < carryingCapacity) {
                food += colGameObject.GetComponent<Food>().TakeFood(carryingCapacity-food);
                if (food > 0) {
                    this.GetComponent<SpriteRenderer>().sprite = antWithFood;
                    creature.SetTarget(nest.gameObject,false);
                }
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision) {

        GameObject colGameObject = collision.gameObject;

        if (colGameObject != null && (colGameObject.name == "Ant(Clone)" || colGameObject.name == "AntOriginal(Clone)")) {
            // If this ant is from another nest then drop a pheromone
            Ant someAnt = colGameObject.GetComponent<Ant>();
            if (someAnt.nest.gameObject.GetInstanceID() != nest.gameObject.GetInstanceID()) {
                if (enemyAnt == null)
                    HandlePheromoneDrop(nest.color);
                enemyAnt = someAnt;
            }
        }

    }

    void OnTriggerStay2D(Collider2D collision)
    {

        GameObject colGameObject = collision.gameObject;

        if (colGameObject.name == "Pheromone(Clone)" && lastPheromone == null) {
            float distanceX = Mathf.Abs(colGameObject.transform.position.x - transform.position.x);
            float distanceY = Mathf.Abs(colGameObject.transform.position.y - transform.position.y);
            //Debug.Log("DistanceX: " + distanceX + " Y: " + distanceY);
            if (distanceX < 0.01f && distanceY < 0.01f) {
                lastPheromone = colGameObject;
            } else if (food < 1) {
                creature.HeadRoughlyTowards(colGameObject);
            }
            
        }
    }
    

    public void Hit() {
        if (Random.value < .2 && health > 0)
            health--;
        status.UpdateMetric(STATUS_TYPES.HEALTH, nest.color, health); 
    }
    
}
