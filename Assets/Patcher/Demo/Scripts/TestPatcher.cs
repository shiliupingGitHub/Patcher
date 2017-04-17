using UnityEngine;
using System.Collections;

public class TestPatcher : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Patcher.InitVersion(1);
        Patcher.UnPackFiles();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
