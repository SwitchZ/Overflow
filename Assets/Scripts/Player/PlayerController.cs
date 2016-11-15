using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public float movementSpeed = 5;
    public bool isRunning;
    public bool teleporting = true;

    public Animator capperAnimator;

    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    public AudioClip shot;
    public AudioClip teleport;

    void Update()
    {
        if(GameManager.instance.enemyCount > 0 && Time.timeScale == 1) Action(); //player only moves while all viruses were not expurged

        if (GameManager.instance.canPause)
        {
            if (Input.GetKeyDown(KeyCode.P)) {
                Time.timeScale = 1 - Time.timeScale;
                /*if (ScoreManager.instance.round == 8)*/ ScoreManager.instance.source.volume = Time.timeScale +0.2f;

            }
        }

        if (GameManager.instance.enemyCount <= 0)
        {
            GameManager.instance.canPause = false;
            this.GetComponent<Rigidbody>().useGravity = false;
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0.1f,1.9f,0.1f), 0.15f);
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, 20f, transform.position.z), 0.05f);

            if(teleporting)
            {
                AudioSource.PlayClipAtPoint(teleport, ScoreManager.instance.transform.position);
                teleporting = false;
            }
        }


        if (ScoreManager.instance.playerHealth <= 0) {
            SceneManager.LoadScene("Stage");
            ScoreManager.instance.playerHealth = 1;
        }

        if(transform.position.y < -5) SceneManager.LoadScene("Stage");
    }

    public void Action() //inputs
    {
        Vector3 UP = new Vector3(0f, 0f, 1f);
        Vector3 DOWN = new Vector3(0f, 0f, -1f);
        Vector3 LEFT = new Vector3(-1f, 0f, 0f);
        Vector3 RIGHT = new Vector3(1f, 0f, 0f);

        //Diagonals 
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
        {
            isRunning = true;
            //capperAnimator.SetBool("isRunning", isRunning);
            transform.rotation = Quaternion.LookRotation(UP+LEFT);
            transform.Translate((Vector3.forward + Vector3.left) * movementSpeed * Time.deltaTime, Space.World);
        } 
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
        {
            isRunning = true;
            //capperAnimator.SetBool("isRunning", isRunning);
            transform.rotation = Quaternion.LookRotation(UP + RIGHT);
            transform.Translate((Vector3.forward + Vector3.right) * movementSpeed * Time.deltaTime, Space.World);
        } 
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
        {
            isRunning = true;
            //capperAnimator.SetBool("isRunning", isRunning);
            transform.rotation = Quaternion.LookRotation(DOWN + LEFT);
            transform.Translate((Vector3.back + Vector3.left) * movementSpeed * Time.deltaTime, Space.World);
         } 
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
        {
            isRunning = true;
            //capperAnimator.SetBool("isRunning", isRunning);
            transform.rotation = Quaternion.LookRotation(DOWN + RIGHT);
            transform.Translate((Vector3.back + Vector3.right) * movementSpeed * Time.deltaTime, Space.World);
        } 

        else if (Input.GetKey(KeyCode.W))
        {
            isRunning = true;
            //capperAnimator.SetBool("isRunning", isRunning);
            transform.rotation = Quaternion.LookRotation(UP);
            transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime, Space.World);
        }

        else if (Input.GetKey(KeyCode.A))
        {
            isRunning = true;
            //capperAnimator.SetBool("isRunning", isRunning);
            transform.rotation = Quaternion.LookRotation(LEFT);
            transform.Translate(Vector3.left * movementSpeed * Time.deltaTime, Space.World);
        }

        else if (Input.GetKey(KeyCode.S))
        {
            isRunning = true;
            //capperAnimator.SetBool("isRunning", isRunning);
            transform.rotation = Quaternion.LookRotation(DOWN);
            transform.Translate(Vector3.back * movementSpeed * Time.deltaTime, Space.World);
        }

        else if (Input.GetKey(KeyCode.D))
        {
            isRunning = true;
            //capperAnimator.SetBool("isRunning", isRunning);
            transform.rotation = Quaternion.LookRotation(RIGHT);
            transform.Translate(Vector3.right * movementSpeed * Time.deltaTime, Space.World);
        }


        if (Input.GetKeyDown(KeyCode.H))
        {
            if (GameManager.instance.memory + 34 <= 100)
            {
                Fire();
                GameManager.instance.memory += 34;

            }
        }

        if (!Input.anyKey)
        {
            isRunning = false;
            //capperAnimator.SetBool("isRunning", isRunning);
        }
    }


    //shooting mechanics
    void Fire()
    {
        // Create the Bullet from the Bullet Prefab
        var bullet = (GameObject)Instantiate(
            bulletPrefab,
            bulletSpawn.position,
            bulletSpawn.rotation);

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 12;

        AudioSource.PlayClipAtPoint(shot, ScoreManager.instance.transform.position);
        // Destroy the bullet after 2 seconds
        Destroy(bullet, 2.0f);
    }

    void OnCollisionEnter(Collision other)
    {
        if ((other.gameObject.tag == "EnemyBullet") || 
            (other.gameObject.tag == "Enemy" && !other.transform.GetComponent<EnemyController>().dead) ||
            (other.gameObject.tag == "Boss" && !other.transform.GetComponent<EnemyController>().dead)) //HIT BY BULLET or TOUCHED ENEMY = -LIFE
        {
            ScoreManager.instance.playerHealth--;

        }
    }
}
