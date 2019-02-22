using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// 
/// 物体的血条控制脚本
/// 挂载在UI画布上
/// 通过调用SetSlider()控制血条
/// </summary>

public class HealthSliderControl : MonoBehaviour {
 
    public float sliderTimer = 2.0f; //血条出现持续时间

    private GameObject sliderObject;
    private Slider slider;
    private bool SliderShown = false;

    //启用或停用血条的接口
    public void SetSlider(bool s)
    {  
        SliderShown = s;
    }


    void Start()
    {
        sliderObject = transform.Find("HealthSlider").gameObject;
        slider = sliderObject.GetComponent<Slider>();
        slider.maxValue = transform.parent.GetComponent<EnemyStatusControl>().getMaxHP();
        slider.value = slider.maxValue;
        SetSlider(false);
    }

	// Update is called once per frame
	void Update () {

        //Debug.Log("Slider Set: " + SliderShown);

        if (SliderShown)
        {
            
            sliderTimer -= Time.deltaTime;
            sliderObject.SetActive(true);

            if (sliderTimer < 0)
            {
                sliderTimer = 2.0f;
                sliderObject.SetActive(false);
                SliderShown = false;
            }
        }
        else
        {
            sliderObject.SetActive(false);
        }

        slider.value = transform.parent.GetComponent<EnemyStatusControl>().getCurrentHP();

    }

}
