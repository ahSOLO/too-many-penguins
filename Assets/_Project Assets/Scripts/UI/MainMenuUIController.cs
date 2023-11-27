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
        SFXController.Instance.PlayOneShot(SFXController.Instance.menuConfirm);
        MusicController.Instance.Pause();
        GameManager.Instance.LoadScene("Level");
    }

    public void QuitGame()
    {
        SFXController.Instance.PlayOneShot(SFXController.Instance.menuConfirm);
        GameManager.Instance.Quit();
    }

    private void OnEnable()
    {
        startingElement.Select();
    }

    public void ToggleCredits(bool isEnabled)
    {
        if (isEnabled)
        {
            SFXController.Instance.PlayOneShot(SFXController.Instance.menuConfirm);
        }
        else
        {
            SFXController.Instance.PlayOneShot(SFXController.Instance.menuBack);
        }
        creditsCanvas.gameObject.SetActive(isEnabled);
    }
}
