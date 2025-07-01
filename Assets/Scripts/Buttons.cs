using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Buttons : MonoBehaviour
{
    [SerializeField] Button _button;

    public void Replay()
    {
        SceneManager.LoadScene("NewScene");
    }
}
