using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDManager : MonoBehaviour {

    public Text levelNamePath;
    public Text time;
    public Text cpuUsageText;
    public Text roundsText;
    public Text bossLife;

    public GameObject pauseOverlay;

    // Use this for initialization
    void Start () {
        levelNamePath.text += GameManager.instance.levelName + "\\"; //escape character-> \
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Time.timeScale == 1) pauseOverlay.SetActive(false);
        else pauseOverlay.SetActive(true);

        string healthBar = "||||||||||||||||||||";

        //time.text = "Time: " + String.Format("{0:0.00}", ScoreManager.instance.timeElapsed);
        time.text = "Time: " + String.Format("{0:0.0}", ScoreManager.instance.timeElapsed);
        cpuUsageText.text = "Memory: " + GameManager.instance.memory + "%";
        if (GameManager.instance.memory > 66) cpuUsageText.GetComponent<Text>().color = new Color(1.0f, 0.1f, 0.1f);
        else cpuUsageText.GetComponent<Text>().color = new Color(0.40f, 1f, 0.32f);

        roundsText.text = "Scan #" + (ScoreManager.instance.round) + " * " + ScoreManager.instance.score;
        if (ScoreManager.instance.round == 8 &&
            !GameManager.instance.roundComplete)
        {
            bossLife.gameObject.SetActive(true);
            bossLife.text = "Cronnos ?: " + healthBar.Substring(0, GameObject.FindWithTag("Boss").GetComponent<EnemyController>().health);
        }

    }
}
