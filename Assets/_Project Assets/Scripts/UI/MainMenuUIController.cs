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
        SFXController.Instance.PlayOneShot(SFXController.Instance.menuConfirm);
        creditsCanvas.gameObject.SetActive(isEnabled);
    }
}
