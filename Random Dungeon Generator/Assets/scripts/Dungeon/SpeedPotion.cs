using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]

public class SpeedPotion : MonoBehaviour
{
    private Player thePlayer;
    public float duration;

    private void Reset()
    {
        GetComponent<Rigidbody2D>().isKinematic = true;
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        box.size = Vector2.one * 0.1f;
        box.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            thePlayer = other.gameObject.transform.parent.GetComponent<Player>();
            StartCoroutine("SpeedBoost");
        }
    }
    IEnumerator SpeedBoost()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        thePlayer.speed *= 2;
        yield return new WaitForSeconds(duration);
        thePlayer.speed = thePlayer.dfltSpeed;
        Destroy(gameObject);
    }
}




