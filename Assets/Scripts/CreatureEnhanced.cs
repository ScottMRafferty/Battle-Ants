using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Uses random direction and force rather than deltaX and deltaY
*/

public class CreatureEnhanced : MonoBehaviour
{
  
    [Header("Configuration")]
    public float minSpeed = 0.0f;
    public float maxSpeed = 0.2f;
    public float turnSpeed = 0.2f;
    public float variability = 0.7f;
    public float updateIntervalModifier = 1.0f;

    private Rigidbody2D body;
    private float deltaForce = 0;
    private float deltaRotation = 0;                     
    private float elapsedTime = 0;
    private Quaternion targetRotation;
    private GameObject target;
    private bool moveAwayFromTarget = true; 

    void Start() {
        body = GetComponent<Rigidbody2D>();
    }

    public void SetTarget(GameObject target, bool moveAwayFromTarget) {
        this.target = target;
        this.moveAwayFromTarget = moveAwayFromTarget;
    }

    public void RandomWalk() {
        if (Random.value < variability) {
            deltaRotation = Random.Range(0,359);
            deltaForce = Mathf.Clamp(deltaForce + Random.Range(-maxSpeed,maxSpeed),minSpeed, maxSpeed);
        }
        Walk();
    }

    public void WalkRelativeToTarget() {
        if (Random.value > variability || target == null) {
            RandomWalk();
        } else {
            HeadRoughlyTowards(target.transform.position);
            if (moveAwayFromTarget)
                deltaRotation = (180 + deltaRotation) % 180;
            Walk();
        }
    }

    public void HeadRoughlyTowards(Vector3 targetCoords) {

        Vector3 normalizedCoords = (targetCoords - transform.position);
        deltaRotation = 180 * Mathf.Atan2(normalizedCoords.y, normalizedCoords.x) / Mathf.PI;
        //Debug.Log("Heading relative to target using deltaRotation " + deltaRotation);

        //float distanceX = Mathf.Abs(transform.position.x - target.transform.position.x);
        //float distanceY = Mathf.Abs(transform.position.y - target.transform.position.y);

        //if (distanceX < .2 || distanceY < .2) 
        //    deltaForce = maxSpeed / 4;
        deltaForce = Mathf.Clamp(deltaForce + Random.Range(-maxSpeed/2,maxSpeed),minSpeed, maxSpeed);
    }

    public bool IsAtEdge(Vector3 coords) {
        return (coords.x <= 0 || coords.x >= 1 || coords.y <= 0 || coords.y >= 1);
    }

    void Walk() {

        
        float dF = deltaForce / World.instance.simulationSpeed;
        float tF = World.instance.simulationSpeed * dF * turnSpeed;

        Vector3 coords = Camera.main.WorldToViewportPoint(transform.position);
        
        if (IsAtEdge(coords)) {
            target = (moveAwayFromTarget == true) ? null : target;
            HeadRoughlyTowards(Vector3.zero);
            //tF = World.instance.simulationSpeed * (deltaForce * 50.0f); // Fast
        } else if (target != null) {
            tF = tF * 10.0f; // Fast
        }

        targetRotation = Quaternion.Euler(0,0,deltaRotation);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, tF);

        body.velocity = Vector3.zero;
        body.AddForce(transform.right * dF); 

        //Debug.Log("Walk -> deltaRotation: " + deltaRotation + ", deltaForce: " + deltaForce + ", coords: " + coords.ToString());

    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= World.instance.simulationSpeed * updateIntervalModifier) {
            elapsedTime = 0;
            if (target) {
                WalkRelativeToTarget();
            } else {
                RandomWalk();
            }

        }
    }

    // You will likely need only 1 of the 2 functions below depending on your unity setup - covering both scenarios

    void OnTriggerEnter2D(Collider2D collider) {
        if (target != null) {
            if (collider.gameObject.GetInstanceID() == target.GetInstanceID()) {
                target = null;
            }
        }
       

    }

    void OnCollisionEnter2D(Collision2D collision) => OnTriggerEnter2D(collision.collider);


}
