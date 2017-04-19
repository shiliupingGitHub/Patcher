using UnityEngine;
using System.Collections;

public class TestPatcher : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Patcher p = new Patcher();
        p.Begin(1,"http:\\");
       // PatcherDownloader.BeginDownload("http://");



    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
