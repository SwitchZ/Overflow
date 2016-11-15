using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        //transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1,1,1), 0.1f);
	}

    void OnCollisionEnter(Collision other)
    {
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -9f);
        if (other.gameObject.tag == "Ground" && ScoreManager.instance.round == 8) StartCoroutine(ShakeCamera(Mathf.FloorToInt(transform.localScale.x)));
    }


    IEnumerator ShakeCamera(int magnitude)
    {
        int originalY = Mathf.FloorToInt(Camera.main.transform.position.y);
        for (int i = magnitude; i >= 0; i--)
        {
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x,originalY-magnitude, Camera.main.transform.position.z);
            yield return new WaitForSeconds(0.125f);
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, originalY + magnitude, Camera.main.transform.position.z);
            yield return new WaitForSeconds(0.125f);
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, originalY, Camera.main.transform.position.z);
        }
        yield return null;
    }
}
