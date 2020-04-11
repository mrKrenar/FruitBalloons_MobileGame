using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    public GameObject groundPrefab, bckgrClouds;
    GameObject player, bckgr, camParent;
    public float adjustableSpeed = 1.005f;
    //private float lastPos, currPos = 2f;
    bool /*isFalling,*/ instantiate;
    
    Transform lastGrPos, lastCloudsPos, currCloudsPos;

    // Use this for initialization
    void Start () {
        bckgr = GameObject.FindGameObjectWithTag("Background");
        currCloudsPos = Instantiate(bckgrClouds, new Vector3(10, 0, 0), Quaternion.Euler(0, 0, 0)).transform;
        lastCloudsPos = currCloudsPos;
        //MoveClouds();
        PauseCloudsMovement();
        lastGrPos = Instantiate(groundPrefab, new Vector3(0,0,0), Quaternion.Euler(0, 0, 0)).transform;
        player = GameObject.FindGameObjectWithTag("Player");
        //cam = Camera.main;
        camParent = GameObject.FindGameObjectWithTag("CamParent");
    }

    public void MoveClouds()
    {
        if(lastCloudsPos.GetComponent<Rigidbody2D>().velocity == new Vector2(0,0))
            lastCloudsPos.GetComponent<Rigidbody2D>().AddForce(new Vector2(0.0001f, 0), ForceMode2D.Impulse);
        if (currCloudsPos.GetComponent<Rigidbody2D>().velocity == new Vector2(0, 0))
            currCloudsPos.GetComponent<Rigidbody2D>().AddForce(new Vector2(0.0001f, 0), ForceMode2D.Impulse);
    }


    public void PauseCloudsMovement()
    {
        lastCloudsPos.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        currCloudsPos.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
    }

    //private void FixedUpdate()
    //{
    //    lastPos = currPos;
    //    currPos = player.transform.position.y;

    //    isFalling = lastPos >= currPos; 
    //}


    void LateUpdate () {
        if (player != null)
        {
            camParent.transform.position = new Vector3(player.transform.position.x, camParent.transform.position.y, camParent.transform.position.z);
            bckgr.transform.position = new Vector2(camParent.transform.position.x, bckgr.transform.position.y);
            if (player.transform.position.x >= lastGrPos.transform.position.x)
            {
                Destroy(lastGrPos.gameObject, 10f);
                lastGrPos = Instantiate(groundPrefab, new Vector3(lastGrPos.transform.position.x + 12, lastGrPos.transform.position.y, 0), Quaternion.Euler(0, 0, 0)).transform;
            }
            if (player.transform.position.x >= lastCloudsPos.transform.position.x)
            {
                if (instantiate)
                {
                    currCloudsPos = Instantiate(bckgrClouds, new Vector3(lastGrPos.transform.position.x + 16, lastGrPos.transform.position.y, 0), Quaternion.Euler(0, 0, 0)).transform;
                    MoveClouds();
                    instantiate = false;
                }
                if (lastCloudsPos != null && player.transform.position.x >= lastCloudsPos.transform.position.x)
                {
                    Destroy(lastCloudsPos.gameObject, 10f);
                    lastCloudsPos = currCloudsPos;
                    instantiate = true;
                }
            }

            if (/*!isFalling &&*/ Camera.main.orthographicSize < 7 && player.transform.position.y > 7)
            {
                Camera.main.orthographicSize *= adjustableSpeed;
                camParent.transform.position = new Vector3(player.transform.position.x, camParent.transform.position.y * adjustableSpeed, -10f);
                
            }

            if (/*isFalling &&*/ Camera.main.orthographicSize > 5 && camParent.transform.position.y > 5 && player.transform.position.y < 7)
            {
                Camera.main.orthographicSize /= adjustableSpeed;
                camParent.transform.position = new Vector3(player.transform.position.x, camParent.transform.position.y / adjustableSpeed, -10f);
            }
        }
    }
}
