using UnityEngine;
using System.Collections;

public class TestPatcher : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Patcher.UnPackFiles(1);
        PatcherDownloader.BeginDownload("http://");



    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
