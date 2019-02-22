using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    private GameObject GameUI;

    private void Awake()
    {
        GameUI = transform.Find("GameUI").gameObject;
        //JoyStickUI = transform.Find("EasyTouchControlsCanvas").gameObject;
        GameUI.SetActive(false);
        //JoyStickUI.SetActive(false);
    }

    // Update is called once per frame
    void Update () {

		if(GameObject.Find("LocalPlayer") != null)
        {
            GameUI.SetActive(true);
            GetComponent<Canvas>().worldCamera = GameObject.Find("LocalPlayer").GetComponentInChildren<Camera>();
        }
	}
}
