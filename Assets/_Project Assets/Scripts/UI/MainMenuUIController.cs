using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUIController : MonoBehaviour
{
    public void PlayGame()
    {
        GameManager.Instance.LoadScene("Level");
    }

    public void QuitGame()
    {
        GameManager.Instance.Quit();
    }
}