using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D) , typeof(BoxCollider2D))]
public class ExitDoorway : MonoBehaviour
{
    private GameManager gameManager = GameManager.Instance;

    private void Reset()
    {
        GetComponent<Rigidbody2D>().isKinematic = true;
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        box.size = Vector2.one * 0.1f;
        box.isTrigger = true;
    }

     void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            //reset dungeon and random seed
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            gameManager.LoadGameSettings(true);
            gameManager.SetGameSettings();
        }
    }

}
