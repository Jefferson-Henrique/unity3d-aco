using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {

    public float speed = 10;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.Translate(Vector3.forward * speed * Time.deltaTime);
	}
}
