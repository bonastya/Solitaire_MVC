using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public FieldController fieldController;

    [Header("Restart")]
    public Button restartButton;

    [Header("End Moves Panel")]
    public GameObject endMovesPanel;
    public Button endMovesPanelFailButton;

    [Header("Fail Panel")]
    public GameObject failPanel;
    public Button failPanelRestartButton;

    [Header("Win Panel")]
    public GameObject winPanel;
    public Button winPanelRestartButton;

    void Start()
    {
        restartButton.onClick.AddListener(fieldController.RestartLevel);
        failPanelRestartButton.onClick.AddListener(fieldController.RestartLevel);
        winPanelRestartButton.onClick.AddListener(fieldController.RestartLevel);

        endMovesPanelFailButton.onClick.AddListener(FailGame);
        StartGame();
    }

    public void StartGame()
    {
        endMovesPanel.SetActive(false);
        failPanel.SetActive(false);
        winPanel.SetActive(false);
    }

    public void WinGame()
    {
        winPanel.SetActive(true);
    }

    public void FailGame()
    {
        failPanel.SetActive(true);
        endMovesPanel.SetActive(false);
    }
    public void EndMoves()
    {
        endMovesPanel.SetActive(true);
    }


}
