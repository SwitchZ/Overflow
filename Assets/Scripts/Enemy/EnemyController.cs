using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
    public int enemyType;
    public int bulletSpeed;
    public int health = 1;
    Vector3 moveDirection = Vector3.back;

    public bool dead = false;
    private bool canShoot = true;
    private bool canMove = true;

    public GameObject playerPrefab;

    public GameObject enemyBulletPrefab;
    public Transform enemyBulletSpawn;

    public GameObject explosion;

    public AudioClip shot;
    public AudioClip bossShot;
    public AudioClip explosionSound;
    public AudioClip bossDead;

    // Use this for initialization
    void Start () {
        GL.wireframe = true;
    }

    // Update is called once per frame
    void Update() {
        if (health <= 0)
        {
            if (!dead)
            {
                dead = true;
                if (tag == "Enemy")
                {
                    ScoreManager.instance.score += 100;
                    GameManager.instance.enemyCount--; //because the boss only dies after the glorious explosion (code below)
                    AudioSource.PlayClipAtPoint(explosionSound, ScoreManager.instance.transform.position);
                }
            }
        }

        if (dead)
        {
            transform.GetComponent<Collider>().enabled = false;
            if (tag == "Enemy") transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.15f);
            else if (tag == "Boss")
            {
                AudioSource.PlayClipAtPoint(explosionSound, ScoreManager.instance.transform.position);
                AudioSource.PlayClipAtPoint(bossDead, ScoreManager.instance.transform.position);
                GameManager.instance.canPause = false;
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.05f);
                if (transform.localScale.x > 0.01f) Instantiate(explosion, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.Euler(-90, 0, 0));
                else {
                    GameManager.instance.enemyCount--;
                    ScoreManager.instance.score += 10000;
                    Destroy(this.gameObject);
                }
            }
        }
        if (transform.localScale == Vector3.zero) Destroy(gameObject);

        playerPrefab = GameObject.FindWithTag("Player");
        //Enemy Behaviors using a Switch-Case
        switch (enemyType)
        {
            case (1): //Enemy Type 1
                break;
            case (2):
                bulletSpeed = 4;
                playerPrefab = GameObject.FindWithTag("Player");
                //shoot player when on "line of sight"
                if ((transform.position.x >= playerPrefab.transform.position.x - 0.75f && transform.position.x <= playerPrefab.transform.position.x + 0.75f) ||
                    (transform.position.z >= playerPrefab.transform.position.z - 0.75f && transform.position.z <= playerPrefab.transform.position.z + 0.75f))
                    if(canShoot && !dead) StartCoroutine(EnemyType2());
                break;
            case (3):
                bulletSpeed = 6;
                if(canMove && !dead) StartCoroutine(EnemyType3());
                if(dead) StopCoroutine(EnemyType3());
                break;
            case (4):
                if (canMove && !dead) StartCoroutine(EnemyType4());
                if (dead) StopCoroutine(EnemyType4());
                break;
            case (5): //aim and shoot every 2 seconds
                bulletSpeed = 10;
                playerPrefab = GameObject.FindWithTag("Player");
                if (canShoot && !dead) StartCoroutine(EnemyType5());
                break;
            case (6): //unavoidable shots when crossing line of sight
                bulletSpeed = 40;
                playerPrefab = GameObject.FindWithTag("Player");
                //shoot player when on "line of sight"
                if ((transform.position.x >= playerPrefab.transform.position.x - 0.75f && transform.position.x <= playerPrefab.transform.position.x + 0.75f) ||
                    (transform.position.z >= playerPrefab.transform.position.z - 0.75f && transform.position.z <= playerPrefab.transform.position.z + 0.75f))
                    if (canShoot && !dead) StartCoroutine(EnemyType2());
                break;
            case (101): //BOSS 1: up and down, up and down... shoots random-spawn bullets when on middle
                transform.LookAt(new Vector3(playerPrefab.transform.position.x, transform.position.y, playerPrefab.transform.position.z));
                bulletSpeed = 12;
                playerPrefab = GameObject.FindWithTag("Player");

                transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(1f, 0.0f, 0.0f, 1.0f) * (1f - health / 20f));
 
                if (canMove && !dead) Boss1();
                if (transform.position.z < -4 && transform.position.z > -5)
                {
                    if (GameObject.FindGameObjectsWithTag("EnemyBullet").Length < 1 && !dead )
                    {
                        BossFire();
                    }
                }
                break;
            default:
                break;
        }
	}

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Bullet" && !dead) //HIT BY CAPPER'S BULLET = DED
        {
            if (tag == "Enemy")
            {
                Instantiate(explosion, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.Euler(-90, 0, 0));
                AudioSource.PlayClipAtPoint(explosionSound, ScoreManager.instance.transform.position);
            }
            Destroy(other.gameObject);
            health--;
            if (tag == "Boss") transform.localScale = new Vector3(transform.localScale.x + 0.1f, transform.localScale.y + 0.1f, transform.localScale.z + 0.1f);


        }

    }

    IEnumerator EnemyType3() //space invader style; sine movement and shoot
    {
        canMove = false;

        //left and fire...
        transform.position = new Vector3(transform.position.x-1, transform.position.y,transform.position.z);
        if(!dead) Fire();
        yield return new WaitForSeconds(1f);
        //...and right and fire
        transform.position = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
        if (!dead) Fire();
        yield return new WaitForSeconds(1f);

        canMove = true;
    }

    IEnumerator EnemyType2() //aim and shoot
    {
        canShoot = false;
        transform.LookAt(new Vector3(playerPrefab.transform.position.x, transform.position.y, playerPrefab.transform.position.z));
        if(!dead) Fire();
        
        yield return new WaitForSeconds(1f);
        canShoot = true;
    }

    IEnumerator EnemyType4() //scrolling movement, right to left
    {
        canMove = false;
        transform.position = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
        if(transform.position.x < 0) transform.position = new Vector3(19, transform.position.y, transform.position.z);
        yield return new WaitForSeconds(0.125f);
        canMove = true;
    }

    IEnumerator EnemyType5() //variation of 2
    {
        canShoot = false;

        yield return new WaitForSeconds(2f);
        transform.LookAt(new Vector3(playerPrefab.transform.position.x, transform.position.y, playerPrefab.transform.position.z));
        if (!dead) Fire();

        canShoot = true;
    }

    IEnumerator EnemyType6() //extremely hardcore variation of 2: lazors
    {
        canShoot = false;

        yield return new WaitForSeconds(2f);
        transform.LookAt(new Vector3(playerPrefab.transform.position.x, transform.position.y, playerPrefab.transform.position.z));
        Fire();

        canShoot = true;
    }

    void Boss1()
    {
        //canMove = false;
        if(transform.position.z >= 0)
        {
            moveDirection = Vector3.back;
 
        }

        else if (transform.position.z <= -9)
        {
            moveDirection = Vector3.forward;

        }

        transform.Translate(moveDirection * 5 *transform.localScale.x * Time.deltaTime, Space.World);


        //canMove = true;
    }

    void Fire()
    {
        // Create the Bullet from the Bullet Prefab
        var bullet = (GameObject)Instantiate(
            enemyBulletPrefab,
            enemyBulletSpawn.position,
            enemyBulletSpawn.rotation);

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;
        AudioSource.PlayClipAtPoint(shot, ScoreManager.instance.transform.position);

        // Destroy the bullet after 2 seconds
        Destroy(bullet, 2.0f);
    }

    void BossFire()
    {
        // Create the Bullet from the Bullet Prefab
        var bullet = (GameObject)Instantiate(
            enemyBulletPrefab,
            new Vector3(playerPrefab.transform.position.x, 10, playerPrefab.transform.position.z),
            enemyBulletSpawn.rotation);

        if (transform.localScale.x >= 3) bullet.transform.position = new Vector3(Random.Range(playerPrefab.transform.position.x - 3, playerPrefab.transform.position.x + 3), 10, Random.Range(playerPrefab.transform.position.z - 3, playerPrefab.transform.position.z + 3));

        bullet.transform.localScale = transform.localScale;
        bullet.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        bullet.GetComponent<Rigidbody>().useGravity = true;

        // Add velocity to the bullet
        bullet.transform.LookAt(playerPrefab.transform);
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;

        AudioSource.PlayClipAtPoint(bossShot, ScoreManager.instance.transform.position);

        // Destroy the bullet after 2 seconds
        Destroy(bullet, 2.0f);
    }


}
