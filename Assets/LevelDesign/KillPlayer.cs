using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KillPlayer : MonoBehaviour
{

    [SerializeField] private int respawn;
    [SerializeField] private PlayerController player;

    // Update is called once per frame
    void FixedUpdate()
    {
        Collider2D hit = Physics2D.OverlapBox(transform.position, transform.localScale, 0.0f);

        if (hit != null)
        {
            if (hit.transform.CompareTag("Player"))
            {
                player.Respawn();
            }
        }
    }
}
