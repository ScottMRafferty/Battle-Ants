using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Similar to the creature class in Greenfoot Ants BUT here deals
    primarily with the movement.  Public variables can be set in 
    unity interface.

    - Random.Range used instead of adjustSpeed
    - Mathf.Clamp used instead of capSpeed function
    - SetTarget replaces setHomeHill
    - WalkRelativeToTarget combines walkAwayFromHome/walkTowardsHome
    
    Bound by viewport space i.e. won't go past the (camera) edge.
    
    Attach this to a 2D sprite.
    
    NOTE: 
    Trying to re-create the original movement as faithfully as possible.
    Not using any Lerp or smooth movements in this script.

*/

public class Creature : MonoBehaviour
{
  
    [Header("Configuration")]
    public int maxSpeed = 3;                        // Pixels to be moved each iteration
    public float updateInterval = 1.0f;             // Default update interval for creature
    public float variability = 0.7f;                // Hardcoded in Greenfoot

    private int deltaX = 0;                     
    private int deltaY = 0;
    private float elapsedTime = 0;
    private GameObject target;                  // Generic target rather than home
    private bool moveAwayFromTarget = true; 
    private Vector2 targetCoords;
    private Quaternion targetRotation;

    public void SetTarget(GameObject target, bool moveAwayFromTarget) {
        this.target = target;
        this.moveAwayFromTarget = moveAwayFromTarget;
    }

    public void RandomWalk() {
        if (Random.value < variability) {
            deltaX = SetSpeed(deltaX);
            deltaY = SetSpeed(deltaY);
        }
        Walk();
    }

    public int SetSpeed(int speed) => 
        Mathf.Clamp(speed + (Random.Range(0,maxSpeed*2-1)-maxSpeed + 1),-maxSpeed, maxSpeed);
    

    public void WalkRelativeToTarget() {
        if (Random.value > variability || target == null) {
            RandomWalk();
        } else {
            HeadRoughlyTowards(target);
            if (moveAwayFromTarget) {
                deltaX = -deltaX;
                deltaY = -deltaY;
            }
            Walk();
        }
    }

    public void HeadRoughlyTowards(GameObject target) {

        float distanceX = Mathf.Abs(transform.position.x - target.transform.position.x);
        float distanceY = Mathf.Abs(transform.position.y - target.transform.position.y);

        bool moveX = (distanceX > 0) && (Random.Range(0,(distanceX) + (distanceY)) < distanceX);
        bool moveY = (distanceY > 0) && (Random.Range(0,(distanceX) + (distanceY)) < distanceY);

        deltaX = ComputeTargetDelta(moveX, transform.position.x, target.transform.position.x);
        deltaY = ComputeTargetDelta(moveY, transform.position.y, target.transform.position.y);

    }

    private int ComputeTargetDelta(bool move, float currentPoint, float targetPoint) {
        if (move)
            return (currentPoint > targetPoint) ? -maxSpeed : maxSpeed;
        return 0;
    }

    public bool IsAtEdge(Vector3 coords) {
        return (coords.x == 0.0f || coords.x == 1.0f || coords.y == 0.0f || coords.y == 1.0f);
    }

    void Walk() {

        // Grab the currentPosition (World Coords) - this means that we
        // can refer to the entire viewports x,y as between 0.0 to 1.0.
        Vector3 coords = Camera.main.WorldToViewportPoint(transform.position);

        // Get the new x and y world coords premised on current deltas
        float offsetX = (1.0f / Screen.width) * deltaX;
        float offsetY = (1.0f / Screen.height) * deltaY;

        // Update the coords with the offsets
        coords.x = Mathf.Clamp01(coords.x + offsetX);
        coords.y = Mathf.Clamp01(coords.y + offsetY);

        // If we hit edge remove any target if we are walking AWAY from it and flip 
        // deltas (if your variability is too low or speed too high this could look like an arkanoid bounce!)
        if (IsAtEdge(coords) == true)
            target = (moveAwayFromTarget == true) ? null : target;
        
        // Update based on preferences
        targetRotation = Quaternion.Euler(0,0,(180 * Mathf.Atan2(deltaY, deltaX) / Mathf.PI));
        targetCoords = Camera.main.ViewportToWorldPoint(coords);

        Rigidbody2D body = GetComponent<Rigidbody2D>();
        if (body) {
            body.velocity = Vector3.zero; // Remove any residual velocity if we have a RigidBody as we want to mimic Greenfoot movement
        }

        // Original movement but may cause strange results if your object has a Rigid Body!
        transform.rotation = targetRotation;
        transform.position =  targetCoords;


    }

    void Update()
    {

        // Remove refs to World.instance.simulationSpeed for independent use
        if (World.instance.simulationSpeed == 0)
            return;
        
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= updateInterval / World.instance.simulationSpeed) {
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
