using UnityEngine;
using System.Collections;

public class DestroyOnLoad : MonoBehaviour {

	void Update () {
        if(Application.isPlaying)
            Destroy(gameObject);
	}
}
