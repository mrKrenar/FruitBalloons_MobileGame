using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondsCollector : MonoBehaviour {
    private UiController ui;

    private void Start()
    {
        ui = GameObject.Find("GameController").GetComponent<UiController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Collectibles")
        {
            ui.IncreaseDiamondsNo(1);
            //Destroy(collision.gameObject);
            collision.GetComponent<Animator>().SetTrigger("small_collect");
            foreach (Transform child in collision.transform)
            {
                if (child.GetComponent<Rigidbody2D>() != null)
                {
                    child.GetComponent<Rigidbody2D>().gravityScale = 1f;
                    child.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-1f, 1f), (Random.Range(1f, 2f))), ForceMode2D.Impulse);
                    child.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-700f, 700f);
                }
                Destroy(child.gameObject, 0.5f);
            }
        }
        else if (collision.tag == "CollectibleHuge")
        {
            Handheld.Vibrate();
            ui.IncreaseDiamondsNo(10);
            collision.GetComponent<Animator>().SetTrigger("huge_collect");
            foreach (Transform child in collision.transform)
            {
                if (child.GetComponent<Rigidbody2D>() != null)
                {
                    child.GetComponent<Rigidbody2D>().gravityScale = 2f;
                    child.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-4f, 4f), (Random.Range(3f, 6f))), ForceMode2D.Impulse);
                    child.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-700f, 700f);
                }
                Destroy(child.gameObject, 1f);
            }

        }
    }
}
