using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    public float health = 100;

    public Text healthText;

    public GameManager gameManager;

    public GameObject playerCamera;

    private float shakeTime;
    private float shakeDuration;
    private Quaternion playerOriginalRotation;

    public CanvasGroup hurtPanel;

    public GameObject weaponHolder;

    int activeWeaponIndex;
    GameObject activeWeapon;

    public float currentPoints;
    public Text pointsNumber;

    public float healthCap;

    public PhotonView photonView;


    private void Start()
    {
        playerOriginalRotation = playerCamera.transform.localRotation;

        WeaponSwitch(0);
        healthCap = health;
    }

    private void Update()
    {
        if (PhotonNetwork.InRoom && !photonView.IsMine)
        {
            playerCamera.SetActive(false);
            return;
        }

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

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            WeaponSwitch(activeWeaponIndex + 1);
        }

        pointsNumber.text = currentPoints.ToString();
    }

    public void Hit(float damage)
    {
        if (PhotonNetwork.InRoom)
        {
            //rpc
            photonView.RPC("PlayerTakeDamage", RpcTarget.All, damage, photonView.ViewID);
        }
        else
        {
            PlayerTakeDamage(damage, photonView.ViewID);
        }
    }

    [PunRPC]
    public void PlayerTakeDamage(float damage, int viewID)
    {
        if (photonView.ViewID == viewID)
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
    }

    public void CameraShake()
    {
        playerCamera.transform.localRotation = Quaternion.Euler(Random.Range(-2, 2), 0, 0);
    }

    public void WeaponSwitch(int weaponIndex)
    {
        int index = 0;
        int amountOfWeapons = weaponHolder.transform.childCount;

        if (weaponIndex > amountOfWeapons - 1)
        {
            weaponIndex = 0;
        }

        foreach (Transform child in weaponHolder.transform)
        {
            if (child.gameObject.activeSelf)
            {
                child.gameObject.SetActive(false);
            }
            if (index == weaponIndex)
            {
                child.gameObject.SetActive(true);
                activeWeapon = child.gameObject;
            }
            index++;
        }
        activeWeaponIndex = weaponIndex;

        if (photonView.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("weaponIndex", weaponIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!photonView.IsMine && targetPlayer == photonView.Owner && changedProps["weaponIndex"] != null)
        {
            WeaponSwitch((int)changedProps["weaponIndex"]);
        }
    }

    [PunRPC]
    public void WeaponShootVFX(int viewID)
    {
        activeWeapon.GetComponent<WeaponManager>().ShootVFX(viewID);
    }
}
