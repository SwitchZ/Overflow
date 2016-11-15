using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    public Material skybox;
    //public Skybox skyboxBoss;

    public static GameManager instance;
    public GameObject scoreManagerObject;

    public string levelName = "MissingLv.";
    public bool roundComplete = false;
    public bool canPrepareNextStage = false;
    public bool canPause = true;

    public int enemyCount = 1;
    public int memory = 0;

    public GameObject enemyPrefab;
    public GameObject bossPrefab;
    public GameObject playerPrefab;
    public GameObject groundPrefab;
    public GameObject blockPrefab;
    public GameObject scoreManagerPrefab;

    public GameObject results;
    public GameObject HUD;
    public GameObject transitionCutter;
    public CanvasGroup transitionFade;

    public AudioClip Stage;
    public AudioClip Boss;
    public AudioClip Boss2;

    //public List<EnemyController> levelComponents = new List<EnemyController>();

    public Text outputText;

    void Awake()
    {
        instance = this;
        if (GameObject.FindWithTag("Score") == null)
        {
            scoreManagerObject = (GameObject) Instantiate(scoreManagerPrefab, Vector3.zero, Quaternion.identity);
            DontDestroyOnLoad(scoreManagerObject);
        }
    }

    // Use this for initialization
    void Start () {


        playerPrefab = GameObject.FindWithTag("Player");
        scoreManagerObject = GameObject.FindWithTag("Score");
        // if(ScoreManager.instance.round == 8) skybox.SetTexture(; //set skybox
        LoadNewLevel();
    }
	
	// Update is called once per frame
	void Update () {
        if(memory>0) memory--; //recover shot cooldown slowly

        //Enter pressed = Triggers preparation for next stage: Transition Cutscene
        if (Input.GetButtonDown("Enter") && roundComplete)
        {
            canPrepareNextStage = true;
        }

        ///Transition Cutscene
        ///
        //cover screen in black, if not a stage before boss. Or else, fade out to black.
        if (canPrepareNextStage) {
            if (ScoreManager.instance.round < 7)
            {
                transitionCutter.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(transitionCutter.GetComponent<RectTransform>().anchoredPosition, new Vector3(0f, 0, 0f), 0.1f);
            }
            else
            {
                HUD.SetActive(false);
                if (transitionFade.alpha == 0f) StartCoroutine(FadeToBlack()); //fadeOut to Black
            } 
        }
        //When screen is black, restart level. This is not the most proper condition though. If transition is a fade out, this code below is done on FadeToBlack
        if (canPrepareNextStage && transitionCutter.GetComponent<RectTransform>().anchoredPosition.y > -42f)
        {
            ScoreManager.instance.round++;
            ScoreManager.instance.score -= Mathf.FloorToInt(ScoreManager.instance.timeElapsed); //deduct score from time
            if (ScoreManager.instance.score <= 0) ScoreManager.instance.score = 0;

            //submit score to GameJolt
            
            int scoreValue = ScoreManager.instance.score; // The actual score.
            string scoreText = ScoreManager.instance.score + " - End of Stage " + (ScoreManager.instance.round-1) ; // A string representing the score to be shown on the website.
            int tableID = 176177; // Set it to 0 for main highscore table.
            string extraData = ""; // This will not be shown on the website. You can store any information.
            
            GameJolt.API.Scores.Add(scoreValue, scoreText, tableID, extraData, (bool success) => {
                Debug.Log(string.Format("Score Add {0}.", success ? "Successful" : "Failed"));
            });
            
            //

            ScoreManager.instance.timeElapsed = 0; //reset timer
            SceneManager.LoadScene("Stage");
        }

        //remove black cover on screen
        if (transitionCutter.GetComponent<RectTransform>().anchoredPosition.y > -42f)
            transitionCutter.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(transitionCutter.GetComponent<RectTransform>().anchoredPosition, new Vector3(0f, 568f, 0f), 0.1f);


        //Auxiliar code. Could be put into another script; Transition box reached the top: Move back to bottom
        if (transitionCutter.GetComponent<RectTransform>().anchoredPosition.y > 560f) transitionCutter.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -568f, 0);

        //No enemies = level complete. This variable is decremented when a enemy is killed.
        if (enemyCount == 0) LevelComplete();
    }
    

    void LoadNewLevel()
    {
        roundComplete = false;
        canPrepareNextStage = false;

        if (ScoreManager.instance.round == 1)
        {
            levelName = "HELLOWORLD";
            Instantiate(enemyPrefab, new Vector3(1, 0, -1), Quaternion.Euler(0, 180, 0));
            Instantiate(enemyPrefab, new Vector3(8, 0, -2), Quaternion.Euler(0, 180, 0));
            Instantiate(enemyPrefab, new Vector3(5, 0, -3), Quaternion.Euler(0, 180, 0));

        }
        else if (ScoreManager.instance.round == 2)
        {
            levelName = "ROBBY";
            EnemyController enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(0, 0, -0), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 2;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(5, 0, -0), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 2;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(9, 0, -0), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 2;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(0, 0, -5), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 2;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(0, 0, -9), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 2;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(9, 0, -9), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 2;

            playerPrefab.transform.position = new Vector3(4, 0.5f, -7);
        }
        else if (ScoreManager.instance.round == 3)
        {
            levelName = "KONGOS";
            //additional platform
            Instantiate(groundPrefab, new Vector3(14.5f, 0, -4.5f), Quaternion.Euler(0, 0, 0));

            EnemyController enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(10, 0, -1), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 2;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(6, 0, -2), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 2;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(4, 0, -3), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 2;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(12, 0, -3), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 2;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(10, 0, -4), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 2;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(18, 0, -4), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 2;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(13, 0, -7), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 2;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(7, 0, -8), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 2;

            GameObject block = (GameObject)Instantiate(blockPrefab, new Vector3(9, 1, -0), Quaternion.identity);
            block.transform.localScale = new Vector3(3, 2, 1);
            block = (GameObject)Instantiate(blockPrefab, new Vector3(1, 1, -2), Quaternion.identity);
            block.transform.localScale = new Vector3(3, 2, 1);
            block = (GameObject)Instantiate(blockPrefab, new Vector3(4, 1, -2), Quaternion.identity);
            block.transform.localScale = new Vector3(3, 2, 1);
            block = (GameObject)Instantiate(blockPrefab, new Vector3(7, 1, -3), Quaternion.identity);
            block.transform.localScale = new Vector3(3, 2, 1);
            block = (GameObject)Instantiate(blockPrefab, new Vector3(10, 1, -3), Quaternion.identity);
            block.transform.localScale = new Vector3(3, 2, 1);
            block = (GameObject)Instantiate(blockPrefab, new Vector3(13, 1, -4), Quaternion.identity);
            block.transform.localScale = new Vector3(3, 2, 1);
            block = (GameObject)Instantiate(blockPrefab, new Vector3(16, 1, -4), Quaternion.identity);
            block.transform.localScale = new Vector3(3, 2, 1);

            block = (GameObject)Instantiate(blockPrefab, new Vector3(15, 1, -7), Quaternion.identity);
            block.transform.localScale = new Vector3(3, 1, 1);
            block = (GameObject)Instantiate(blockPrefab, new Vector3(18, 1, -7), Quaternion.identity);
            block.transform.localScale = new Vector3(3, 1, 1);
            block = (GameObject)Instantiate(blockPrefab, new Vector3(12, 1, -8), Quaternion.identity);
            block.transform.localScale = new Vector3(3, 1, 1);
            block = (GameObject)Instantiate(blockPrefab, new Vector3(9, 1, -8), Quaternion.identity);
            block.transform.localScale = new Vector3(3, 1, 1);
            block = (GameObject)Instantiate(blockPrefab, new Vector3(6, 1, -9), Quaternion.identity);
            block.transform.localScale = new Vector3(3, 1, 1);
            block = (GameObject)Instantiate(blockPrefab, new Vector3(3, 1, -9), Quaternion.identity);
            block.transform.localScale = new Vector3(3, 1, 1);

            playerPrefab.transform.position = new Vector3(0, 0.5f, -9);
        }
        else if (ScoreManager.instance.round == 4)
        {
            levelName = "VARIA";
            //additional platform
            Instantiate(groundPrefab, new Vector3(14.5f, 0, -4.5f), Quaternion.Euler(0, 0, 0));

            EnemyController enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(1, 0, -2), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 3;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(3, 0, -2), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 3;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(5, 0, -2), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 3;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(7, 0, -2), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 3;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(9, 0, -2), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 3;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(11, 0, -2), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 3;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(13, 0, -2), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 3;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(15, 0, -2), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 3;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(17, 0, -2), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 3;

            GameObject block = (GameObject)Instantiate(blockPrefab, new Vector3(2, 1, -2), Quaternion.identity);
            block = (GameObject)Instantiate(blockPrefab, new Vector3(4, 1, -2), Quaternion.identity);
            block = (GameObject)Instantiate(blockPrefab, new Vector3(6, 1, -2), Quaternion.identity);
            block = (GameObject)Instantiate(blockPrefab, new Vector3(8, 1, -2), Quaternion.identity);
            block = (GameObject)Instantiate(blockPrefab, new Vector3(10, 1, -2), Quaternion.identity);
            block = (GameObject)Instantiate(blockPrefab, new Vector3(12, 1, -2), Quaternion.identity);
            block = (GameObject)Instantiate(blockPrefab, new Vector3(14, 1, -2), Quaternion.identity);
            block = (GameObject)Instantiate(blockPrefab, new Vector3(16, 1, -2), Quaternion.identity);
            block = (GameObject)Instantiate(blockPrefab, new Vector3(18, 1, -2), Quaternion.Euler(0, 180, 0));

            playerPrefab.transform.position = new Vector3(9, 0.5f, -8);
        }

        else if (ScoreManager.instance.round == 5)
        {
            levelName = "LOVELACE";
            //additional platform
            Instantiate(groundPrefab, new Vector3(14.5f, 0, -4.5f), Quaternion.Euler(0, 0, 0));

            EnemyController enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(9, 0, -0), Quaternion.Euler(0, -90, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 4;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(8, 0, -2), Quaternion.Euler(0, -90, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 4;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(7, 0, -4), Quaternion.Euler(0, -90, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 4;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(8, 0, -6), Quaternion.Euler(0, -90, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 4;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(9, 0, -8), Quaternion.Euler(0, -90, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 4;

            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(17, 0, -0), Quaternion.Euler(0, -90, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 4;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(19, 0, -2), Quaternion.Euler(0, -90, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 4;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(19, 0, -4), Quaternion.Euler(0, -90, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 4;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(18, 0, -6), Quaternion.Euler(0, -90, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 4;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(17, 0, -8), Quaternion.Euler(0, -90, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 4;

            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(12, 0, -1), Quaternion.Euler(0, -90, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 4;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(13, 0, -4), Quaternion.Euler(0, -90, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 4;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(14, 0, -7), Quaternion.Euler(0, -90, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 4;

            playerPrefab.transform.position = new Vector3(0f, 0.5f, -4);
            playerPrefab.transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        else if (ScoreManager.instance.round == 6)
        {
            levelName = "CICADA";
            //additional platform
            Instantiate(groundPrefab, new Vector3(14.5f, 0, -4.5f), Quaternion.Euler(0, 0, 0));

            EnemyController enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(10, 0, -1), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 5;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(10, 0, -3), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 5;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(10, 0, -5), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 5;

            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(8, 0, -2), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 5;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(12, 0, -2), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 5;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(6, 0, -0), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 5;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(14, 0, -0), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 5;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(5, 0, -7), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 5;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(15, 0, -7), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 5;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(4, 0, -3), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 5;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(16, 0, -3), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 5;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(3, 0, -5), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 5;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(17, 0, -5), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 5;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(2, 0, -1), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 5;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(18, 0, -1), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 5;

            playerPrefab.transform.position = new Vector3(10, 0.5f, -9);

        }
        else if (ScoreManager.instance.round == 7)
        {
            levelName = "BEAMUS";
            //additional platform
            Instantiate(groundPrefab, new Vector3(14.5f, 0, -4.5f), Quaternion.Euler(0, 0, 0));

            EnemyController enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(2, 0, -3), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 6;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(4, 0, -3), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 6;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(6, 0, -3), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 6;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(9, 0, -3), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 6;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(11, 0, -3), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 6;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(13, 0, -3), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 6;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(16, 0, -3), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 6;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(18, 0, -3), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 6;

            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(2, 0, -6), Quaternion.Euler(0, 0, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 6;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(4, 0, -6), Quaternion.Euler(0, 0, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 6;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(6, 0, -6), Quaternion.Euler(0, 0, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 6;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(9, 0, -6), Quaternion.Euler(0, 0, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 6;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(11, 0, -6), Quaternion.Euler(0, 0, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 6;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(13, 0, -6), Quaternion.Euler(0, 0, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 6;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(16, 0, -6), Quaternion.Euler(0, 0, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 6;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(18, 0, -6), Quaternion.Euler(0, 0, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 6;

            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(14, 0, -0), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 6;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(3, 0, -1), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 6;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(7, 0, -2), Quaternion.Euler(0, 180, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 6;

            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(8, 0, -7), Quaternion.Euler(0, 0, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 6;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(3, 0, -8), Quaternion.Euler(0, 0, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 6;
            enemy = ((GameObject)Instantiate(enemyPrefab, new Vector3(15, 0, -9), Quaternion.Euler(0, 0, 0))).GetComponent<EnemyController>();
            enemy.enemyType = 6;

            playerPrefab.transform.position = new Vector3(0f, 0.5f, -4);
            playerPrefab.transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        else if (ScoreManager.instance.round == 8)
        {
            levelName = "KERNEL";
            Instantiate(groundPrefab, new Vector3(-5.5f, 0, -4.5f), Quaternion.Euler(0, 0, 0));
            Instantiate(groundPrefab, new Vector3(14.5f, 0, -4.5f), Quaternion.Euler(0, 0, 0));

            EnemyController boss = ((GameObject)Instantiate(bossPrefab, new Vector3(8, 0, -9), Quaternion.Euler(0, -90, 0))).GetComponent<EnemyController>();
            boss.enemyType = 101;
            boss.health = 20;

            playerPrefab.transform.position = new Vector3(-7f, 0.5f, -4);
            playerPrefab.transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        else
        {
            SceneManager.LoadScene("Menu");
        }

        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length + GameObject.FindGameObjectsWithTag("Boss").Length;

    }

    public void LevelComplete()
    {
        canPause = false;
        //Show results
        if (!roundComplete) StartCoroutine(AnimateResultsText());

    }


    IEnumerator AnimateResultsText() //Dude, this is a mess
    {
        roundComplete = true;
        results.SetActive(true);

        yield return new WaitForSeconds(0.25f);
        outputText.GetComponent<Text>().text = "America Network Services [v6.1.8]\n..\\" + levelName + "> ";
        yield return new WaitForSeconds(0.5f);
        outputText.GetComponent<Text>().text = "America Network Services [v6.1.8]\n..\\" + levelName + "> _";
        yield return new WaitForSeconds(0.5f);
        outputText.GetComponent<Text>().text = outputText.GetComponent<Text>().text.Remove(outputText.GetComponent<Text>().text.Length - 1);
        for (int i = 0; i < ("DONE.log".Length); i++) //typewriting effect on DONE.log
        {
            outputText.GetComponent<Text>().text += "DONE.log"[i] + "_";
            yield return new WaitForSeconds(.2f);
            outputText.GetComponent<Text>().text = outputText.GetComponent<Text>().text.Remove(outputText.GetComponent<Text>().text.Length - 1); //"Delete cursor character"
        }
        outputText.GetComponent<Text>().text += "\n\n\n";
        outputText.GetComponent<Text>().text += "Operation Started " + Mathf.Floor(UnityEngine.Random.Range(1, 13)) + "/" + Mathf.Floor(UnityEngine.Random.Range(1, 32)) + "/" + Mathf.Floor(UnityEngine.Random.Range(1970, 2039)) + "\n";
        yield return new WaitForSeconds(.25f);

        outputText.GetComponent<Text>().text += ("Operation Time Elapsed: " + String.Format("{0:0.00}", ScoreManager.instance.timeElapsed) + "\n\n\n");
        yield return new WaitForSeconds(.5f);
        outputText.GetComponent<Text>().text += "    ==Press [Enter] to proceed==\n";
        outputText.GetComponent<Text>().text += ">_";

        //readyForNextStage = true;

        for (int i = 0; i < 1; i += 0) //cursor blinking
        {
            outputText.GetComponent<Text>().text = outputText.GetComponent<Text>().text.Remove(outputText.GetComponent<Text>().text.Length - 1);
            yield return new WaitForSeconds(.5f);
            outputText.GetComponent<Text>().text += "_";
            yield return new WaitForSeconds(.5f);
        }
        
    }

    IEnumerator FadeToBlack()
    {
        while (transitionFade.alpha < 1f)
        {
            transitionFade.alpha += 0.01f;
            ScoreManager.instance.source.volume -= 0.01f; 
            yield return new WaitForSeconds(0.015f);
        }
        //submit score to GameJolt

        int scoreValue = ScoreManager.instance.score; // The actual score.
        string scoreText = ScoreManager.instance.score + " - End of Stage " + (ScoreManager.instance.round); // A string representing the score to be shown on the website.
        int tableID = 176177; // Set it to 0 for main highscore table.
        string extraData = ""; // This will not be shown on the website. You can store any information.

        GameJolt.API.Scores.Add(scoreValue, scoreText, tableID, extraData, (bool success) => {
            Debug.Log(string.Format("Score Add {0}.", success ? "Successful" : "Failed"));
        });

        //

        yield return new WaitForSeconds(1.5f);
        ScoreManager.instance.round++;
        ScoreManager.instance.timeElapsed = 0; //reset timer
        ScoreManager.instance.source.Stop();
        ScoreManager.instance.source.volume = 1.0f;
        if (ScoreManager.instance.round < 9) SceneManager.LoadScene("Stage");
        else SceneManager.LoadScene("Menu");
    }
}
