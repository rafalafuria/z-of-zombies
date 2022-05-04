using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public int price = 50;

    public Text priceNumber;

    public Text priceText;

    PlayerManager playerManager;

    bool playerIsInReach = false;

    public bool HeathStation;
    public bool ammoStation;

    // Start is called before the first frame update
    void Start()
    {
        priceNumber.text = price.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerIsInReach)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                BuyShop();
            }
        }
    }

    public void BuyShop()
    {
        if (playerManager.currentPoints >= price)

        {
            playerManager.currentPoints -= price;
            if (HeathStation)
            {
                playerManager.health = playerManager.healthCap;
                playerManager.healthText.text = "Health: " + playerManager.health.ToString();
            }
            if (ammoStation)
            {
                foreach (Transform child in playerManager.weaponHolder.transform)
                {
                    WeaponManager weaponManager = child.GetComponent<WeaponManager>();
                    weaponManager.reserveAmmo = weaponManager.ammoCap;
                    StartCoroutine(weaponManager.Reload(weaponManager.reloadTime));
                    weaponManager.reserveAmmoText.text = weaponManager.reserveAmmo.ToString();
                }
            }
        }

        else
        {
            Debug.Log("Poor");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            priceNumber.gameObject.SetActive(true);
            priceText.gameObject.SetActive(true);
            playerIsInReach = true;
            playerManager = other.GetComponent<PlayerManager>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            priceNumber.gameObject.SetActive(false);
            priceText.gameObject.SetActive(false);
            playerIsInReach = false;
        }
    }
}
