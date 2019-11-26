using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private PresentationManager presentationManager;
    [SerializeField] private GameObject pauseMenuContainer;
    [SerializeField] private GameObject resumeBtn;
    [SerializeField] private GameObject pauseBtn;
    [SerializeField] private GameObject startBtn;
    [SerializeField] private TextMeshProUGUI slideNumber;
    [SerializeField] private GameObject restartBtn;

    public void StartShow()
    {
        startBtn.SetActive(false);
        resumeBtn.SetActive(true);
        restartBtn.SetActive(true);
        pauseMenuContainer.SetActive(false);

        slideNumber.text = 1 + "";
        slideNumber.gameObject.SetActive(true);
        pauseBtn.SetActive(true);

        presentationManager.StartShow();
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void RestartShow()
    {
        startBtn.SetActive(true);
        resumeBtn.SetActive(false);
        restartBtn.SetActive(false);
        pauseMenuContainer.SetActive(true);

        slideNumber.gameObject.SetActive(false);
        pauseBtn.SetActive(false);

        presentationManager.RestartShow();
    }

    public void Pause()
    {
        pauseMenuContainer.SetActive(true);

        slideNumber.gameObject.SetActive(false);
        pauseBtn.SetActive(false);

        presentationManager.StopShow();
    }

    public void Resume()
    {
        pauseMenuContainer.SetActive(false);

        slideNumber.gameObject.SetActive(true);
        pauseBtn.SetActive(true);

        presentationManager.ResumeShow();
    }

    private void Update()
    {
        slideNumber.text = (presentationManager.slideIndex + 1).ToString();
    }
}
