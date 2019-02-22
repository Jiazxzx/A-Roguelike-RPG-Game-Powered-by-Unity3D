using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DisablePlayerControl : NetworkBehaviour
{
    private GameObject cam;
    private GameObject ite;
    //private AudioListener aud;
    private GameObject flas;
    private PlayerStatusControl sta;
    private PlayerMovementControl mov;
    private PlayerEquipmentControl equ;
    private leftJoystick lef;
    private rightJoystick rig;

    // Use this for initialization
    void Start()
    {
        if (isLocalPlayer)
            gameObject.name = "LocalPlayer";

        if (!isLocalPlayer)
        {//TODO:拿到子物体/ 消除多人时候的影响
            cam = this.gameObject.GetComponentInChildren<Camera>().gameObject;
            cam.SetActive(false);

            flas = this.gameObject.GetComponentInChildren<Flashlight>().gameObject;
            flas.SetActive(false);

            //sta = GetComponent<PlayerStatusControl>();
            //sta.enabled = false;

            //mov = GetComponent<PlayerMovementControl>();
            //mov.enabled = false;

            //equ = GetComponent<PlayerEquipmentControl>();
            //equ.enabled = false;

            //lef = GetComponent<leftJoystick>();
            //lef.enabled = false;

            //rig = GetComponent<rightJoystick>();
            //rig.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
