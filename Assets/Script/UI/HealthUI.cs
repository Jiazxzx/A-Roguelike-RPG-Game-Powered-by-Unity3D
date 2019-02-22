using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HealthUI : MonoBehaviour {


    public GameObject player;
    private int playerHP;
    private int heartCount;
    private int halfHeartCount;
    private GameObject heartsContainer;
    private bool playerFound = false;

    public GameObject Heart;
    public GameObject HalfHeart;

	// Use this for initialization
	void Start () {

        player = transform.parent.parent.GetComponent<GameUI>().getCurrentPlayer();
        Heart = Resources.Load("Prefab/UI/Heart") as GameObject;
        HalfHeart = Resources.Load("Prefab/UI/HalfHeart") as GameObject;
    }
	
	// Update is called once per frame
	void Update () {

        if (player == null)
        {
            player = transform.parent.parent.GetComponent<GameUI>().getCurrentPlayer();
        }
        else if(!playerFound)
        {
            playerHP = player.GetComponent<PlayerStatusControl>().getCurrentHP();
            heartsContainer = transform.Find("HeartsContainer").gameObject;
            displayHealth(playerHP);
            playerFound = true;
        }
        else
        {
            playerHP = player.GetComponent<PlayerStatusControl>().getCurrentHP();
            displayHealth(playerHP);
        }
    }

    private void displayHealth(int HP)
    {
        heartCount = HP / 2;
        halfHeartCount = HP % 2;

        for (int i = 0; i< heartsContainer.transform.childCount; i++)
        {
            Destroy(heartsContainer.transform.GetChild(i).gameObject);
        }

        for(int i =0; i< heartCount; i++)
        {
            GameObject HeartInstance = Instantiate(Heart);
            HeartInstance.transform.parent = heartsContainer.transform;
            HeartInstance.transform.localScale = new Vector3(60, 60, 0);
        }

        if(halfHeartCount == 1)
        {
            GameObject HalfHeartInstance = Instantiate(HalfHeart);
            HalfHeartInstance.transform.parent = heartsContainer.transform;
            HalfHeartInstance.transform.localScale = new Vector3(60, 60, 0);
        }

    }

}
