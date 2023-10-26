using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;

public class LevelUIController : Singleton<LevelUIController>
{
    [SerializeField] private Image weightBar;
    [SerializeField] private TextMeshProUGUI weightPercentText;
    [SerializeField] private TextMeshProUGUI weightText;
    [SerializeField] private GameObject overLimitStack;
    [SerializeField] private TextMeshProUGUI overLimitText;
    [SerializeField] private GameObject gameOverStack;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Canvas pauseMenuCanvas;
    [SerializeField] BoolEvent pauseGameToggle;
    [SerializeField] private Canvas scoreCanvas;
    [SerializeField] private GameObject scoreRowPrefab;
    [SerializeField] private Transform scoreRowParent;
    [SerializeField] private float presentScorePauseDuration;
    [SerializeField] private TextMeshProUGUI clockText;
    [SerializeField] private Selectable startingElement;

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        pauseGameToggle.Register(OnPauseGameToggle);
    }

    private void OnDisable()
    {
        pauseGameToggle.Unregister(OnPauseGameToggle);
    }

    private void OnPauseGameToggle(bool isPaused)
    {
        pauseMenuCanvas.gameObject.SetActive(isPaused);
        if (isPaused)
        {
            startingElement.Select();
            SFXController.Instance.PlayOneShot(SFXController.Instance.menuPause);
            SFXController.Instance.PlayInstance(SFXController.Instance.pauseSnapshot, ref SFXController.Instance.pauseSnapshotInstance);
        }
        else
        {
            SFXController.Instance.StopInstance(SFXController.Instance.pauseSnapshotInstance);
            SFXController.Instance.PlayOneShot(SFXController.Instance.menuUnpause);
        }
    }

    public void SetWeightDisplay(int current, int max)
    {
        var percentage = Mathf.Clamp01((float) current / max);
        weightBar.fillAmount = percentage;
        weightPercentText.text = $"{Mathf.Round(percentage * 100)}%";
        weightText.text = $"{current}/{max}";
    }

    public void UpdateOverlimitCountdown(float countdownValue)
    {
        countdownValue = Mathf.Max(0f, countdownValue);
        overLimitStack.SetActive(true);
        overLimitText.text = countdownValue.ToString("0.00");
    }

    public void ResetOverlimitCountdown()
    {
        overLimitStack.SetActive(false);
    }

    public void ShowGameOverUI(string text)
    {
        gameOverStack.SetActive(true);
        gameOverText.text = text;
    }

    public void ResumeGame()
    {
        pauseGameToggle.Raise(false);
        SFXController.Instance.PlayOneShot(SFXController.Instance.menuConfirm);
    }

    public void RestartLevel()
    {
        pauseGameToggle.Raise(false);
        SFXController.Instance.PlayOneShot(SFXController.Instance.menuConfirm);
        GameManager.Instance.LoadScene("Level");
    }

    public void MainMenu()
    {
        pauseGameToggle.Raise(false);
        SFXController.Instance.PlayOneShot(SFXController.Instance.menuConfirm);
        GameManager.Instance.LoadScene("Main Menu");
    }

    public void AddScoreRow(string name, int score)
    {
        var row = Instantiate(scoreRowPrefab, scoreRowParent, false);
        row.GetComponent<ScoreRow>().Populate(name, score);
    }

    public void PresentScore(float delay)
    {
        StartCoroutine(PresentScoreCoroutine(presentScorePauseDuration, delay));
    }

    public void PresentTime(float totalSeconds)
    {
        int minutes = (int)totalSeconds / 60;
        int seconds = (int)totalSeconds % 60;
        if (minutes < 0) minutes = 0;
        if (seconds < 0) seconds = 0;
        clockText.text = $"{minutes.ToString("0")}:{seconds.ToString("00")}";
    }

    private IEnumerator PresentScoreCoroutine(float pauseBetweenUpdates, float delay)
    {
        GameManager.Instance.gameIsPausable = false;
        yield return new WaitForSeconds(delay);
        scoreCanvas.gameObject.SetActive(true);
        for (int i = 0; i < scoreRowParent.childCount; i++)
        {
            yield return new WaitForSeconds(pauseBetweenUpdates);
            scoreRowParent.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void Quit()
    {
        SFXController.Instance.PlayOneShot(SFXController.Instance.menuConfirm);
        GameManager.Instance.Quit();
    }
}
