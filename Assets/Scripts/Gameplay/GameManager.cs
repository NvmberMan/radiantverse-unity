using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game States")]
    public bool isGameActive = true;
    public bool isGameOver = false;

    [Header("UI References")]
    public GameObject winPanel; 
    public GameObject losePanel; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void OnFinishLineCrossed(string tag)
    {
        if (isGameOver) return;

        isGameOver = true;
        isGameActive = false;
        Time.timeScale = 0;

        if (tag == "Player")
        {
            WinnerMechanic();
        }
        else if (tag == "NPC")
        {
            LossMechanic();
        } 
    }

    private void WinnerMechanic()
    {
        Debug.Log("✨ PLAYER MENANG!");
        if (winPanel != null) winPanel.SetActive(true);
    }

    private void LossMechanic()
    {
        Debug.Log("💀 PLAYER KALAH!");
        if (losePanel != null) losePanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}