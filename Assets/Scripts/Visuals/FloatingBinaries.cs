using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class FloatingBinaries : MonoBehaviour {

    public Text binaryText;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if(!GameManager.instance.roundComplete && Time.timeScale == 1) StartCoroutine("BinariesAnimation");

    }

    IEnumerator BinariesAnimation()
    {
        int month = Random.Range(0, 2);
        binaryText.GetComponent<Text>().text = binaryText.GetComponent<Text>().text.Substring(1);
        binaryText.GetComponent<Text>().text += month;
        yield return new WaitForSeconds(0.1f);

    }
}
