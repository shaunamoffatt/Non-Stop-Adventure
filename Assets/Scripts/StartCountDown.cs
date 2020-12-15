using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartCountDown : MonoBehaviour
{
    public int countDownTime = 3;
    public TMP_Text countDownDisplay;


    // Start is called before the first frame update
    void Start()
    {
        countDownDisplay.gameObject.SetActive(true);
        StartCoroutine(CountDown());
    }

    IEnumerator CountDown()
    {
        yield return new WaitForSecondsRealtime(1f);

        while (countDownTime > 0)
        {
            countDownDisplay.text = countDownTime.ToString();
            yield return new WaitForSecondsRealtime(1);
            countDownTime--;
        }

        countDownDisplay.text = "GO!";

        PlayerManager.instance.BeginPlay();

        yield return new WaitForSecondsRealtime(1f);

        gameObject.GetComponent<Timer>().enabled = true;
        countDownDisplay.gameObject.SetActive(false);
    }
}
