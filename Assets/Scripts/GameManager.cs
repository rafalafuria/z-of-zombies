using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int enemiesAlive = 0;

    public int round = 0;

    public GameObject[] spawnPoints;

    public GameObject enemyPrefab;

    public Text roundNumber;
    public Text roundSurvived;

    public GameObject endScreen;
    public bool checkMenu;

    public GameObject pauseMenu;

    public GameObject hurtPanel;

    public Animator blackScreenAnimator;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (enemiesAlive == 0)
        {
            round++;
            NextWave(round);
            roundNumber.text = "Round: " + round.ToString();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }

    }

    public void NextWave(int round)
    {
        for (var x = 0; x < round; x++)
        {
            GameObject spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            GameObject enemySpawned = Instantiate(enemyPrefab, spawnPoint.transform.position, Quaternion.identity);
            enemySpawned.GetComponent<EnemyManager>().gameManager = GetComponent<GameManager>();
            enemiesAlive++;
        }
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    public void EndGame()
    {
        checkMenu = true;
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        endScreen.SetActive(true);
        hurtPanel.SetActive(false);
        roundSurvived.text = round.ToString();
        Destroy(pauseMenu);
    }

    public void BackToMainMenu()
    {
        AudioListener.volume = 1;
        Invoke("LoadMainMenuScene", 0.4f);
        blackScreenAnimator.SetTrigger("FadeIn");
        Time.timeScale = 1;
    }

    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene(0);
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        hurtPanel.SetActive(true);
        AudioListener.volume = 1;
    }

    public void Pause()
    {
        if (checkMenu != true)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            hurtPanel.SetActive(false);
            AudioListener.volume = 0;
        }
    }
}
