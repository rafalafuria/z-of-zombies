using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    public GameObject playerCam;
    public float range = 100f;
    public float damage = 25f;
    public Animator playerAnimator;

    public ParticleSystem muzzleFlash;
    public GameObject hitParticle;
    public GameObject nonTargetHitParticles;

    public AudioClip gunShot;
    public AudioSource audioSource;

    public WeaponSway weaponSway;
    float swaySensitivity;

    public GameObject crossHair;

    public float maxAmmo;
    public float currentAmmo;
    public float reloadTime = 2f;

    bool isReloding;
    public float reserveAmmo;

    public Text currentAmmoText;
    public Text reserveAmmoText;

    public float firerate = 10;
    float firerateTimer = 0;
    public bool isAutomatic;

    public string weaponType;

    public PlayerManager playerManager;

    public float ammoCap;

    private void OnDisable()
    {

        playerAnimator.SetBool("isReloading", false);

        isReloding = false;

        //Debug.Log("Reload Interupted");

    }
    private void OnEnable()
    {

        playerAnimator.SetTrigger(weaponType);

        currentAmmoText.text = currentAmmo.ToString();

        reserveAmmoText.text = reserveAmmo.ToString();

    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        swaySensitivity = weaponSway.swaySensitivity;

        currentAmmoText.text = currentAmmo.ToString();
        reserveAmmoText.text = reserveAmmo.ToString();
        ammoCap = reserveAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerAnimator.GetBool("isShooting"))
        {
            playerAnimator.SetBool("isShooting", false);
        }

        if (reserveAmmo <= 0 && currentAmmo <= 0)
        {
            //Debug.Log("No ammo left for this weapon!");
            return;
        }

        if (currentAmmo <= 0 && isReloding == false)
        {
            //Debug.Log("No ammo in this clip");
            StartCoroutine(Reload(reloadTime));
            return;
        }

        if (isReloding == true)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.R) && reserveAmmo > 0)
        {
            //Debug.Log("Manual Reload");
            StartCoroutine(Reload(reloadTime));
            return;
        }

        if (firerateTimer > 0)
        {
            firerateTimer -= Time.deltaTime;
        }

        if (Input.GetButton("Fire1") && firerateTimer <= 0 && isAutomatic)
        {
            //Debug.Log("Shoot");
            Shoot();
            firerateTimer = 1 / firerate;
        }

        if (Input.GetButtonDown("Fire1") && firerateTimer <= 0 && !isAutomatic)
        {
            //Debug.Log("Shoot");
            Shoot();
            firerateTimer = 1 / firerate;
        }

        if (Input.GetButtonDown("Fire2"))
        {
            //Debug.Log("Shoot");
            Aim();
        }

        if (Input.GetButtonUp("Fire2"))
        {
            if (playerAnimator.GetBool("isAiming"))
            {
                playerAnimator.SetBool("isAiming", false);
            }
            weaponSway.swaySensitivity = swaySensitivity;
            crossHair.SetActive(true);
        }
    }

    public IEnumerator Reload(float rt)
    {
        isReloding = true;
        playerAnimator.SetBool("isReloading", true);
        yield return new WaitForSeconds(rt);
        playerAnimator.SetBool("isReloading", false);
        float missingAmmo = maxAmmo - currentAmmo;

        if (reserveAmmo >= missingAmmo)
        {
            currentAmmo += missingAmmo;
            reserveAmmo -= missingAmmo;
            currentAmmoText.text = currentAmmo.ToString();
            reserveAmmoText.text = reserveAmmo.ToString();
        }

        else
        {
            currentAmmo += reserveAmmo;
            reserveAmmo = 0;

            currentAmmoText.text = currentAmmo.ToString();
            reserveAmmoText.text = reserveAmmo.ToString();
        }

        isReloding = false;

    }

    void Aim()
    {
        playerAnimator.SetBool("isAiming", true);
        weaponSway.swaySensitivity = swaySensitivity / 3;
        crossHair.SetActive(false);
    }

    void Shoot()
    {
        currentAmmo--;
        currentAmmoText.text = currentAmmo.ToString();
        muzzleFlash.Play();
        audioSource.PlayOneShot(gunShot);
        playerAnimator.SetBool("isShooting", true);

        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, range))
        {
            //Debug.Log("Hit");
            EnemyManager enemyManager = hit.transform.GetComponent<EnemyManager>();
            if (enemyManager != null)
            {
                enemyManager.Hit(damage);
                if (enemyManager.health <= 0)
                {
                    playerManager.currentPoints += enemyManager.points;
                }

                GameObject instParticles = Instantiate(hitParticle, hit.point, Quaternion.LookRotation(hit.normal));
                instParticles.transform.parent = hit.transform;

                Destroy(instParticles, 2f);
            }
            else
            {
                GameObject InstParticles = Instantiate(nonTargetHitParticles, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(InstParticles, 20f);
            }
        }
    }

}
