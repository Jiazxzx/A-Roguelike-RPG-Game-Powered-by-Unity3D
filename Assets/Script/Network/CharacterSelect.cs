using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class CharacterSelect : MonoBehaviour {

    public List<GameObject> CharacterList;
    public GameObject CharacterSlected;
    public SpriteRenderer CharacterImage;
    public int CurrentIndex;

    public void Start()
    {
        CharacterImage = GameObject.Find("CharacterImage").GetComponent<SpriteRenderer>();

        CurrentIndex = 0;
        CharacterSlected = CharacterList[CurrentIndex];        
        CharacterImage.sprite = CharacterSlected.GetComponent<SpriteRenderer>().sprite;

    }

    public void NextCharac()
    {
        CurrentIndex = (CurrentIndex + 1) % CharacterList.Count;
        CharacterSlected = CharacterList[CurrentIndex];
        CharacterImage.sprite = CharacterSlected.GetComponent<SpriteRenderer>().sprite;       
    }

    public void PreCharac()
    {
        if (CurrentIndex > 0)
        {
            CurrentIndex--;
        }
        else
        {
            CurrentIndex = CharacterList.Count - 1;
        }
        CharacterSlected = CharacterList[CurrentIndex];
        CharacterImage.sprite = CharacterSlected.GetComponent<SpriteRenderer>().sprite;
    }

    public void BackBtn()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

}
