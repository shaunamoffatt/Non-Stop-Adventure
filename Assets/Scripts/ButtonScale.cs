using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScale : MonoBehaviour
{

    [SerializeField] private float scaleTime = 0.1f;
   
    //Size/Scale of the buttons
    [SerializeField] private Vector3 startingScale = new Vector3(1, 1, 1);
    Button button;

    private void Start()
    {
        button = GetComponent<Button>();
    }

    public void ScaleButton()
    {
        ScaleButton(button);
    }

    public void ScaleButton(Button b)
    {
        StartCoroutine(ScaleUpAndDown(b));
    }

    IEnumerator ScaleUpAndDown(Button b)
    {
        Vector3 myScale = b.transform.localScale * 1.1f;
        for (float time = 0; time < scaleTime * 2; time += Time.deltaTime)
        {
            float progress = Mathf.PingPong(time, scaleTime) / scaleTime;
            b.transform.localScale = Vector3.Lerp(startingScale, myScale, progress);
            yield return null;
        }
        //reset scale and unlock button
        b.transform.localScale = startingScale;
    }
}