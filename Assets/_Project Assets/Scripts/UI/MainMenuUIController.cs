using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIController : MonoBehaviour
{
    [SerializeField] private Canvas creditsCanvas;
    [SerializeField] private Selectable startingElement;

    public void PlayGame()
    {
        SFXController.Instance.PlayOneShot(SFXController.Instance.menuSelect);
        GameManager.Instance.LoadScene("Level");
    }

    public void QuitGame()
    {
        SFXController.Instance.PlayOneShot(SFXController.Instance.menuSelect);
        GameManager.Instance.Quit();
    }

    private void OnEnable()
    {
        startingElement.Select();
    }

    public void ToggleCredits(bool isEnabled)
    {
        SFXController.Instance.PlayOneShot(SFXController.Instance.menuSelect);
        creditsCanvas.gameObject.SetActive(isEnabled);
    }
}
