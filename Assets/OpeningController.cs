using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class OpeningController : MonoBehaviour
{
    public GameObject scoreManagerObject;
    public GameObject scoreManagerPrefab;

    public Text overflowDirectory;
    public bool typingDirectory = false;

    //Canvases
    public GameObject intro;
    public GameObject backstory;
    public GameObject mainTitle;
    public GameObject stageSelect;


    public GameObject map;
    public Text stageName;

    public Sprite americaClear;

    public static OpeningController instance;

    void Awake()
    {
        instance = this;

        if (GameObject.FindWithTag("Score") == null)
        {
            scoreManagerObject = (GameObject)Instantiate(scoreManagerPrefab, Vector3.zero, Quaternion.identity);
            DontDestroyOnLoad(scoreManagerObject);
        }
    }

    // Use this for initialization
    void Start()
    {

        if (!typingDirectory && ScoreManager.instance.round <= 8) StartCoroutine(Typewrite(overflowDirectory, "C:" + "\\OVERFLOW~1>Overflow.exe"));

        scoreManagerObject = GameObject.FindWithTag("Score");
        if (ScoreManager.instance.round > 8)
        {
            
            GameJolt.API.Trophies.Unlock(63992, (bool success) => {
                if (success)
                {
                   Debug.Log("Success!");
                }
                else
                {
                Debug.Log("Something went wrong");
                }
            });
            
            StopCoroutine("Typewrite");
            intro.SetActive(false);
            mainTitle.SetActive(false);
            stageSelect.SetActive(true);

        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Return) && mainTitle.activeSelf)
        {
            mainTitle.SetActive(false);
            stageSelect.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Return) && backstory.GetComponent<RectTransform>().anchoredPosition.y > 0 && !stageSelect.activeSelf)
        {
            mainTitle.SetActive(true);
            overflowDirectory.GetComponent<Text>().text = "";
        }
        else if(Input.GetKeyDown(KeyCode.Return) && stageSelect.activeSelf && ScoreManager.instance.round == 1)
        {
            ScoreManager.instance.source.Stop();
            SceneManager.LoadScene("Stage");
        }

        //finished typing Overflow Directory -> show game backstory
        if (!typingDirectory && overflowDirectory.GetComponent<Text>().text == "C:" + "\\OVERFLOW~1>Overflow.exe")
        {
            backstory.SetActive(true);
            overflowDirectory.gameObject.SetActive(false);
            backstory.GetComponent<RectTransform>().anchoredPosition += new Vector2(0,0.47f);
        }

        //finished backstory -> show game title
        if (!typingDirectory && backstory.GetComponent<RectTransform>().anchoredPosition.y > 1826 && !stageSelect.activeSelf)
        {
            mainTitle.SetActive(true);
            overflowDirectory.GetComponent<Text>().text = "";
        }

        //Update Map
        if (ScoreManager.instance.round > 8)
        {
            map.GetComponent<Image>().sprite = americaClear;
            stageName.text = "Mission 1 Clear \nThank you for playing. More stages should come someday.";
        };
    }


    IEnumerator Typewrite(Text output, string inputText)
    {
        output.GetComponent<Text>().text += "_";
        output.GetComponent<Text>().text = output.GetComponent<Text>().text.Remove(output.GetComponent<Text>().text.Length - 1);

        typingDirectory = true;
        for (int i = 0; i < (inputText.Length); i++) //typewriting effect on DONE.log
        {
            output.GetComponent<Text>().text += inputText[i] + "_";
            yield return new WaitForSeconds(Random.Range(.05f,.2f));
            output.GetComponent<Text>().text = output.GetComponent<Text>().text.Remove(output.GetComponent<Text>().text.Length - 1); //"Delete cursor character"
        }
        typingDirectory = false;
    }

}
