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
    public float maxSpeed = 2.0f;
    public float maxTurnAngle = 45.0f;
    public float turnSpeed = 1.0f;
    public float variability = 0.8f;
    public float updateInterval = 1.0f;

    private Rigidbody2D body;
    private float deltaForce = 0;
    private float deltaRotation = 0;                   
    private float elapsedTime = 0;
    private float actualUpdateInterval = 1.0f;
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
            deltaForce = Mathf.Clamp(deltaForce + Random.Range(-maxSpeed,maxSpeed),minSpeed, maxSpeed);
            float deltaAngle = maxTurnAngle / Mathf.Clamp(deltaForce,1.0f,maxSpeed);
            deltaRotation = Mathf.Clamp(deltaRotation + Random.Range(-deltaAngle, +deltaAngle), -180, 180);
        }
        Walk();
    }

    public void WalkRelativeToTarget() {
        if (Random.value > variability || target == null) {
            RandomWalk();
        } else {
            HeadRoughlyTowards(target.transform.position);
            if (moveAwayFromTarget) {
                deltaRotation = 180 - Quaternion.Inverse(Quaternion.Euler(0,0,deltaRotation)).eulerAngles.z;
                Debug.DrawLine(target.transform.position, transform.position, Color.white, 0.1f);
            } else
                Debug.DrawLine(target.transform.position, transform.position, Color.green, 0.1f);
            Walk();
        }
    }

    public void HeadRoughlyTowards(Vector3 targetCoords) {

        Vector3 normalizedCoords = (targetCoords - transform.position);
        deltaRotation = 180 * Mathf.Atan2(normalizedCoords.y, normalizedCoords.x) / Mathf.PI;
        deltaForce = Mathf.Clamp(deltaForce + Random.Range(-maxSpeed,maxSpeed),minSpeed, maxSpeed);

    }

    public bool IsAtEdge(Vector3 coords) {
        return (coords.x <= 0 || coords.x >= 1 || coords.y <= 0 || coords.y >= 1);
    }

    void Walk() {

        float dF = deltaForce / actualUpdateInterval;

        Vector3 coords = Camera.main.WorldToViewportPoint(transform.position);
        if (IsAtEdge(coords)) {
            target = (moveAwayFromTarget == true) ? null : target;
            HeadRoughlyTowards(Vector3.zero);
        }

        body.velocity = Vector3.zero;
        body.AddForce(transform.right * dF); 

        Quaternion targetRotation = Quaternion.Euler(0,0,deltaRotation);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed);

    }

    void Update()
    {

        // Remove refs to World.instance.simulationSpeed for independent use
        if (World.instance.simulationSpeed == 0) {
            body.velocity = Vector3.zero;
            return;
        }

        elapsedTime += Time.deltaTime;
        actualUpdateInterval = updateInterval / World.instance.simulationSpeed;
        if (elapsedTime >= actualUpdateInterval) {
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
