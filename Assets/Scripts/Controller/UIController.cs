using System.Collections;
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
        restartButton.onClick.AddListener(RestartLevel);
        failPanelRestartButton.onClick.AddListener(fieldController.RestartLevel);
        winPanelRestartButton.onClick.AddListener(fieldController.RestartLevel);
        endMovesPanelFailButton.onClick.AddListener(FailGame);

        StartGame();
        StartCoroutine(RestartUnable());
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

    private void RestartLevel()
    {
        fieldController.RestartLevel();
        StartCoroutine(RestartUnable());
    }

    private IEnumerator RestartUnable()
    {
        //  нопка перезапуска не доступна после нажати€ до начала уровн€
        restartButton.enabled = false;
        yield return new WaitForSeconds(GameDesignData.ANIM_SPAWN_CARDS_DURATION + GameDesignData.ANIM_OPEN_CARD_DURATION);
        restartButton.enabled = true;
    }


}
