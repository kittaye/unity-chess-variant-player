using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
    public float speed;
	
	// Update is called once per frame
	void LateUpdate () {
        transform.position += new Vector3(Input.GetAxis("Horizontal") * speed * Time.deltaTime, Input.GetAxis("Vertical") * speed * Time.deltaTime);
    }
}
