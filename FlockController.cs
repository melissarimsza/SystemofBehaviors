using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * The scenario depicted in this virtual world is of a schools of fish and a shark
 * The fish mostly stay in clusters but do not crowd eachother. They also flee when the shark gets too close
 * The shark wanders until it comes within attacking distance of the fish
 * The shark attacks the fish until they are out of range
 * 
 * */

public class FlockController : MonoBehaviour
{

    public List<BoidController> boidList;

    public GameObject boidPrefab;
    public GameObject predatorPrefab;

    CameraHelper helper;

    Quaternion zeroRotation = new Quaternion();

    float predatorDetectionDistance = 50f;
    float boidDetectionDistance = 25f;
    Color fleeColor = new Color(1f, 0f, 0f, 1f);

    // Start is called before the first frame update
    void Start()
    {
        helper = new CameraHelper();

        boidList = new List<BoidController>();

        for (int i = 0; i < 50; i++)
        {
            float x = Random.Range(-0.5f, 0.5f);
            float y = Random.Range(-0.5f, 0.5f);
            Vector3 point = new Vector3(x, y, 0f);
            AddBoid(point);
        }
    }


    void AddBoid(Vector3 point)
    {
        GameObject tempObject = Instantiate(boidPrefab, point, zeroRotation);
        BoidController thisBoid = tempObject.GetComponent<BoidController>();
        boidList.Add(thisBoid);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < boidList.Count; i++)
        {
            float distanceBetween = Vector3.Distance(predatorPrefab.transform.position, boidList[i].transform.position);

            if (distanceBetween < predatorDetectionDistance)
            {
                boidList[i].Flee(predatorPrefab);
            }
            boidList[i].Flock(boidList);
        }
    }

    public void FleeBehavior(BoidController thisBoid)
    {

        float distanceBetween = Vector3.Distance(predatorPrefab.transform.position, thisBoid.transform.position);

        if (distanceBetween < predatorDetectionDistance)
        {

        }

    }

    public Vector3 tooClose()
    {
        for (int i = 0; i < boidList.Count; i++)
        {
            float distanceBetween = Vector3.Distance(predatorPrefab.transform.position, boidList[i].transform.position);

            if (distanceBetween < boidDetectionDistance)
            {
                return boidList[i].transform.position;
            }
        }
        return new Vector3(0f, 0f, 0f);
    }

}
