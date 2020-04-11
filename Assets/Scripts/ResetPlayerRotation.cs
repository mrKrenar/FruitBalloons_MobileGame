using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetPlayerRotation : MonoBehaviour {
    PlayerAndTrampolines player;

    private SpriteRenderer objectToRotate;
    private bool rotating;

    private void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAndTrampolines>();
        objectToRotate = GameObject.FindGameObjectWithTag("RotatingPlayerImg").GetComponent<SpriteRenderer>();
    }


    private IEnumerator Rotate(float duration)
    {
        rotating = true;
        Quaternion startRotation = objectToRotate.transform.rotation;
        Quaternion endRotation = Quaternion.Euler(0, 0, 0);
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            objectToRotate.transform.rotation = Quaternion.Lerp(startRotation, endRotation, t / duration);
            yield return null;
        }
        objectToRotate.transform.rotation = endRotation;
        rotating = false;
    }

    public void StartReseting()
    {
        if (!rotating)
            StartCoroutine(Rotate(0.2f));
    }
}
