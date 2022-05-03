using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public float health = 100;

    public Text healthText;

    public GameManager gameManager;

    public GameObject playerCamera;

    private float shakeTime;
    private float shakeDuration;
    private Quaternion playerOriginalRotation;

    public CanvasGroup hurtPanel;

    private void Start()
    {
        playerOriginalRotation = playerCamera.transform.localRotation;
    }

    private void Update()
    {
        if (hurtPanel.alpha > 0)
        {
            hurtPanel.alpha -= Time.deltaTime;
        }
        if (shakeTime < shakeDuration)
        {
            shakeTime += Time.deltaTime;
            CameraShake();
        }
        else if (playerCamera.transform.localRotation != playerOriginalRotation)
        {
            playerCamera.transform.localRotation = playerOriginalRotation;
        }
    }

    public void Hit(float damage)
    {
        health -= damage;
        healthText.text = "Health " + health.ToString();

        if (health <= 0)
        {
            gameManager.EndGame();
        }
        else
        {
            shakeTime = 0;
            shakeDuration = 0.2f;
            hurtPanel.alpha = 1;
        }
    }
    public void CameraShake()
    {
        playerCamera.transform.localRotation = Quaternion.Euler(Random.Range(-2, 2), 0, 0);
    }
}
