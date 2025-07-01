using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class End : MonoBehaviour
{
    Collider _collider;

    private void Start()
    {
        _collider = GetComponent<Collider>();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() != null)

        {
            Debug.Log("Player has reached the end of the level.");
            SceneManager.LoadScene("EndScene");
        }
    }
}
