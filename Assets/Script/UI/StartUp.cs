using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class StartUp : MonoBehaviour {

    private bool flag = false;
    private bool played = false;
    private bool flash = false;
    private float timer;
    private float timer_2;
    private float StartTime;
    private float alpha;
    private Image EyeImage;
    private Image BlackImage;
    private AudioSource Audio;
    private Text TouchText;

    public void Start()
    {
        flag = false;
        timer = 4f;
        timer_2 = 0.3f;
        alpha = 0f;
        EyeImage = transform.Find("EyeImage").GetComponent<Image>();
        EyeImage.enabled = false;
        BlackImage = transform.Find("BlackImage").GetComponent<Image>();
        Audio = GetComponent<AudioSource>();
        TouchText = transform.Find("TouchText").GetComponent<Text>();
    }

    public void StartUpBtn()
    {
        flag = true;
        StartTime = Time.realtimeSinceStartup;

    }


    public void Update()
    {
        timer_2 -= Time.deltaTime;
        if (timer_2 <= 0)
        {
            flash = !flash;
            if (flash)
            {
                TouchText.enabled = true;
            }
            else
            {
                TouchText.enabled = false;
            }
            timer_2 = 0.3f;
        }



        if (flag)
        {
            timer -= Time.deltaTime;

            if (timer >= 2.5f)
            {
                alpha = ((Time.realtimeSinceStartup - StartTime) / 1.5f);
                BlackImage.color = new Color(0, 0, 0, alpha);
            }
            else if (timer < 2f && timer >= 1)
            {
                Debug.Log("Set True");
                EyeImage.enabled = true;
                if (!played)
                {
                    Audio.PlayOneShot(Audio.clip);
                    played = true;
                }
            }else if(timer >= 0)
            {
                EyeImage.enabled = false;
            }
            else if(timer < 0)
            {
                SceneManager.LoadScene("MainMenuScene");
            }
        }
    }
}
