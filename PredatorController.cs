using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorController : MonoBehaviour
{

    Vector3 position;
    Vector3 velocity;
    Vector3 acceleration;

    float maxForce = 0.0525f;
    float maxSpeed = 0.3f;

    //variables for wandering
    float wanderTheta = 0f;
    float wanderR = 0.25f;
    float wanderD = 5.0f;

    Color attackColor = new Color(1f, 0f, 0f, 1f);
    Color wanderColor = new Color(1f, 1f, 1f, 1f);

    CameraHelper helper;

    public FlockController thisFlockController;

    // Start is called before the first frame update
    void Start()
    {
        helper = new CameraHelper();
        position = transform.position;
        velocity = new Vector3(0f, 0.025f);
        acceleration = new Vector3(0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Borders();

        Vector3 attackTarget = thisFlockController.tooClose();

        if (attackTarget == new Vector3(0f, 0f, 0f))
        {
            print("wandering");
            GoWander();
        }
        else
        {
            print("attacking");
            GoAttack(attackTarget);
        }


        velocity = velocity + acceleration;
        position = position + velocity;

        transform.position = position;

        //angle object in direction its moving
        float angle = (Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg) - 90f;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
        transform.rotation = rotation;

        acceleration = Vector3.zero;
    }


    void GoSeek(Vector3 target)
    {
        Vector3 desired = target - position;

        desired = desired.normalized * maxSpeed;

        Vector3 steer = desired - velocity;

        steer = Vector3.ClampMagnitude(steer, maxForce);

        ApplyForce(steer);
    }

    void GoAttack(Vector3 target)
    {

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.color = attackColor;

        //print("go attack called");
        Vector3 desired = target - position;

        float tempMaxSpeed = 0.6f;

        //desired = desired.normalized * maxSpeed;
        desired = desired.normalized * tempMaxSpeed;

        Vector3 steer = desired - velocity;

        steer = Vector3.ClampMagnitude(steer, maxForce);


        //print(steer);


        ApplyForce(steer);
    }

    void GoWander()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.color = wanderColor;


        //randomly change wander theta
        //was 0.3f;
        float change = 0.02f;
        wanderTheta += Random.Range(-change, change);

        //calcukate circle position
        Vector3 circlePos = velocity;
        circlePos.Normalize();
        circlePos = (circlePos * wanderD) + position;

        //heading angle for velocity vector
        float h = Mathf.Atan2(velocity.y, velocity.x);

        //target position
        float cx = wanderR * Mathf.Cos(wanderTheta + h);
        float cy = wanderR * Mathf.Sin(wanderTheta + h);
        Vector3 circleOffset = new Vector3(cx, cy);
        Vector3 target = circlePos + circleOffset;

        //seek to target
        GoSeek(target);
       // GoAttack(target);
    }

    void Borders()
    {
        if (position.x < helper.visible.min.x)
        {
            position.x = helper.visible.max.x;
        }
        else if (position.x > helper.visible.max.x)
        {
            position.x = helper.visible.min.x;
        }

        if (position.y < helper.visible.min.y)
        {
            position.y = helper.visible.max.y;
        }
        else if (position.y > helper.visible.max.y)
        {
            position.y = helper.visible.min.y;
        }
    }

    public void ApplyForce(Vector3 force)
    {
        acceleration = acceleration + force;
    }
}
