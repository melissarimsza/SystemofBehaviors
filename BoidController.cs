using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidController : MonoBehaviour
{
    Vector3 position;
    Vector3 velocity;
    Vector3 acceleration;
    float maxForce = 0.025f;
    float maxSpeed = 0.8f;

    Color wanderColor = new Color(1f, 1f, 1f, 1f);
    Color fleeColor = new Color(1f, 0f, 0f, 1f);

    CameraHelper helper;

    //public GameObject predator;

    // Start is called before the first frame update
    void Start()
    {
        helper = new CameraHelper();
        position = transform.position;
        velocity = new Vector3(0f, 0f);
        acceleration = new Vector3(0f, 0f);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //FleeBehavior();
       

        velocity = velocity + acceleration;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        position = position + velocity;

        transform.position = position;

        Borders();

        float angle = (Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg) - 90f;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
        transform.rotation = rotation;

        acceleration *= 0f;
    }

    public void ApplyForce(Vector3 force)
    {
        acceleration = acceleration + force;
    }

    //passing it a list of objects of itself
    public void Flock(List<BoidController> boidList)
    {

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.color = wanderColor;

        Vector3 sep = Seperate(boidList);
        Vector3 ali = Align(boidList);
        Vector3 coh = Cohesion(boidList);

        sep = sep * 1.5f;
        ali = ali * 1.0f;
        coh = coh * 1.0f;

        ApplyForce(sep);
        ApplyForce(ali);
        ApplyForce(coh);
    }


    //in example this is a void function but changed it to return steer
    ////then call ApplyForce in Flock function instead of at the end of each behavior
    public Vector3 Seperate(List<BoidController> boidList)
    {
        float desiredSeperation = 4f;
        Vector3 steer = Vector3.zero;
        int count = 0;

        //for every boid in the system, check if it is too close
        for (int i = 0; i < boidList.Count; i++)
        {
            float d = Vector3.Distance(position, boidList[i].position);
            //if distance > 0 and distance < arbitrary amount
            if ((d > 0.0001f) && (d < desiredSeperation))
            {
                //calculate vector pointing away from neighbor
                Vector3 diff = position - boidList[i].position;
                diff.Normalize();
                diff = diff / d;
                steer = steer + diff;
                count++;
            }
        }

        //Average
        if (count > 0)
        {
            steer = steer / (float)count;

            //as long as the vector is > 0
            if (steer.magnitude > 0f)
            {
                steer.Normalize();
                steer = steer * maxSpeed;
                steer = steer - velocity;
                steer = Vector3.ClampMagnitude(steer, maxForce);
            }
        }

        //ApplyForce(steer);
        return steer;
    }

    Vector3 Align(List<BoidController> boidList)
    {
        float neighborDistance = 6f;
        Vector3 sum = Vector3.zero;
        int count = 0;

        for (int i = 0; i < boidList.Count; i++)
        {
            float d = Vector3.Distance(position, boidList[i].position);
            if ((d > 0.0001f) && (d < neighborDistance))
            {
                sum = sum + boidList[i].velocity;
                count++;
            }
        }

        if (count > 0)
        {
            sum = sum / (float)count;
            sum.Normalize();
            sum = sum * maxSpeed;
            Vector3 steer = sum - velocity;
            steer = Vector3.ClampMagnitude(steer, maxForce);
            return steer;
        }
        else
        {
            return Vector3.zero;
        }
    }

    Vector3 Cohesion(List<BoidController> boidList)
    {

        float neighborDistance = 6f;
        Vector3 sum = Vector3.zero;
        int count = 0;

        for (int i = 0; i < boidList.Count; i++)
        {
            float d = Vector3.Distance(position, boidList[i].position);

            if ((d > 0.0001f) && (d < neighborDistance))
            {
                sum = sum + boidList[i].position;
                count++;
            }
        }

        if (count > 0)
        {
            sum = sum / (float)count;
            return Seek(sum);
        }
        else
        {
            return Vector3.zero;
        }
    }



    public Vector3 Seek(Vector3 target)
    {
        Vector3 desired = target - position;

        desired = desired.normalized * maxSpeed;

        Vector3 steer = desired - velocity;

        steer = Vector3.ClampMagnitude(steer, maxForce);

        return steer;
    }

    public void Flee(GameObject predator)
    {

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.color = fleeColor;


        Vector3 desired = predator.transform.position - position;
        desired = desired * -1f;

        desired = desired.normalized * maxSpeed;

        Vector3 steer = desired - velocity;



        steer = Vector3.ClampMagnitude(steer, maxForce);

        ApplyForce(steer);
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


}
