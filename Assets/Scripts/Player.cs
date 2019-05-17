using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    //configuration parameteres
    [Header("Player")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float padding = 1f;
    [SerializeField] int health = 5;
    [SerializeField] List<Image> hearts;
    [Header("Sound Effect")]
    [SerializeField] AudioClip deathSFX;
    [SerializeField] [Range(0, 1)] float deathSoundVolume = 0.75f;
    [SerializeField] AudioClip loseLifeSFX;
    [SerializeField] [Range(0, 1)] float loseLifeSoundVolume = 0.75f;
    [SerializeField] AudioClip shootSFX;
    [SerializeField] [Range(0, 1)] float shootSoundVolume = 0.25f;
    [Header("Projectile")]
    [SerializeField] GameObject laserPrefab;
    [SerializeField] float projectileSpeed;
    [SerializeField] float projectileFiringPeriod = 0.1f;


    Coroutine firingCoroutine;

    float xMin;
    float xMax;
    float yMin;
    float yMax;

	// Use this for initialization
	void Start () {

        SetUpMoveBoundaries();
		
	}

    // Update is called once per frame
    void Update () {

        Move();
        Fire();
	}

    private void Fire()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            firingCoroutine = StartCoroutine(FireContinuously());
        }
        if(Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(firingCoroutine);
        }
    }

    IEnumerator FireContinuously()
    {
        while(true)
        { 
            GameObject laser = Instantiate(
                laserPrefab,
                transform.position,
                Quaternion.identity) as GameObject;

            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed);

            AudioSource.PlayClipAtPoint(shootSFX, Camera.main.transform.position, shootSoundVolume);

            yield return new WaitForSeconds(projectileFiringPeriod);
        }
    }

    private void Move()
    {
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed; // Edit -> Project Settings -> Input
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;

        var newXPosition = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        var newYPosition = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);

        transform.position = new Vector2(newXPosition, newYPosition);
    }

    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + padding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - padding;
        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + padding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - padding;
    }

    public int GetScoreHealth()
    {
        return health;
    }

    public void UpdateScoreHealth(int scorePerKill)
    {
        health += scorePerKill;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if(!damageDealer)
        {
            return;
        }
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();

        if(health < 0)
        {
            health = 0;
        }
        damageDealer.Hit();
        if(health <= 0)
        {
            foreach(Image heart in hearts)
            {
                heart.GetComponent<Image>().color = new Color32(49, 49, 49, 255);
            }
            Die();
        }
        else
        {
            hearts[health].GetComponent<Image>().color = new Color32(49, 49, 49, 255);
            AudioSource.PlayClipAtPoint(loseLifeSFX, Camera.main.transform.position, loseLifeSoundVolume);
        }
    }

    private void Die()
    {
        FindObjectOfType<Level>().LoadGameOver();
        Destroy(gameObject);
        AudioSource.PlayClipAtPoint(deathSFX, Camera.main.transform.position, deathSoundVolume);
        
    }
}
