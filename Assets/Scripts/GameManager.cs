using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks
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

    public PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawners");
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient && photonView.IsMine)
        {
            if (enemiesAlive == 0)
            {
                round++;
                NextWave(round);
                if (PhotonNetwork.InRoom)
                {
                    Hashtable hash = new Hashtable();
                    hash.Add("currentRound", round);
                    PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
                }
                else
                {
                    DisplayNextRound(round);
                }

            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }

    }

    private void DisplayNextRound(int round)
    {
        roundNumber.text = round.ToString();
    }

    public void NextWave(int round)
    {
        for (var x = 0; x < round; x++)
        {
            GameObject spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject enemySpawned;
            if (PhotonNetwork.InRoom)
            {
                enemySpawned = PhotonNetwork.Instantiate("Zombie", spawnPoint.transform.position, Quaternion.identity);
            }
            else
            {
                enemySpawned = Instantiate(Resources.Load("Zombie"), spawnPoint.transform.position, Quaternion.identity) as GameObject;
            }
            enemySpawned.GetComponent<EnemyManager>().gameManager = GetComponent<GameManager>();
            enemiesAlive++;
        }
    }

    public void Restart()
    {
        if (!PhotonNetwork.InRoom)
        {
            Time.timeScale = 0;
        }
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    public void EndGame()
    {
        if (!PhotonNetwork.InRoom)
        {
            Time.timeScale = 0;
        }
        checkMenu = true;
        ;
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
        if (!PhotonNetwork.InRoom)
        {
            Time.timeScale = 1;
        }
    }

    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene(0);
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        if (!PhotonNetwork.InRoom)
        {
            Time.timeScale = 1;
        }
        Cursor.lockState = CursorLockMode.Locked;
        hurtPanel.SetActive(true);
        AudioListener.volume = 1;
    }

    public void Pause()
    {
        if (checkMenu != true)
        {
            Cursor.lockState = CursorLockMode.None;
            hurtPanel.SetActive(false);
            if (!PhotonNetwork.InRoom)
            {
                Time.timeScale = 0;
            }
            AudioListener.volume = 0;
            pauseMenu.SetActive(true);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log("Player" + targetPlayer + " changed " + changedProps);

        if (photonView.IsMine)
        {
            if (changedProps["currentRound"] != null)
            {
                DisplayNextRound((int)changedProps["currentRound"]);
            }
        }
    }
}
