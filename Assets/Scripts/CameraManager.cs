using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

    public GameObject playerPrefab;

	// Use this for initialization
	void Start () {
        playerPrefab = GameObject.FindWithTag("Player");
    }
	
	// Update is called once per frame
	void Update () {
	    if(ScoreManager.instance.round >= 3) //update camera position on specific rounds
        {
            if (this.transform.position.x != playerPrefab.transform.position.x) transform.position = new Vector3(playerPrefab.transform.position.x, transform.position.y, transform.position.z);
        }
	}
}
