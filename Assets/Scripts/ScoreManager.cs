using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour {

    public int round = 1;
    public int playerHealth = 1;
    public float t,timeElapsed = 10000;
    public int score = 0;

    public static ScoreManager instance;

    public AudioSource source;

    public AudioClip Menu;
    public AudioClip Stage;

    void Awake () {
        instance = this;
    }

    void Start()
    {
        source = ScoreManager.instance.GetComponent<AudioSource>();

    }

    void Update()
    {
        if (!GameManager.instance.roundComplete) timeElapsed += 0.1f;
    }

    void FixedUpdate()
    {
        if (!source.isPlaying && source.loop && ScoreManager.instance.round == 8)
        {
            Debug.Log("puta merda");
            source.clip = GameManager.instance.Boss;
            source.Play();
            source.loop = false;
        }

        if (!source.isPlaying && SceneManager.GetActiveScene().name == "Menu")
        {
            if (OpeningController.instance.backstory.GetComponent<RectTransform>().anchoredPosition.y < 0)
            {
                ScoreManager.instance.source.clip = Menu;
                ScoreManager.instance.source.Play();
            }
        }

        if (SceneManager.GetActiveScene().name == "Stage")

        if (!source.isPlaying && ScoreManager.instance.round < 8 && SceneManager.GetActiveScene().name == "Stage")
        {
            source.clip = Stage;
            source.Play();
        }

        if (!source.isPlaying && !source.loop && ScoreManager.instance.round == 8)
        {
            source.clip = GameManager.instance.Boss2;
            source.Play();
            source.loop = true;
        }

    }


    IEnumerator BossMusic() //2 parts of loops, this was necessary :(
    {
        source.clip = GameManager.instance.Boss;
        source.Play();
        source.loop = false;
        yield return new WaitForSeconds(source.clip.length);
        source.clip = GameManager.instance.Boss2;
        source.Play();
        source.loop = true;
    }

    /*
    IEnumerator BossMusicPart2() //2 parts of loops, this was necessary :(
    {
        source.clip = GameManager.instance.Boss2;
        source.Play();
        source.loop = true;
    }
    */
}
