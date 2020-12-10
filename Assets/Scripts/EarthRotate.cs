using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthRotate : MonoBehaviour
{
    //public void Rotate(bool rightArrow)
    //{

    //}
    //IEnumerator RotateEarthandSky()
    //{
    //    float offset = (arrowRight) ? 0.25f : -0.25f;
    //    Vector3 startingUV = skyDomeRender.material.mainTextureOffset;
    //    Vector3 endingUV = startingUV + new Vector3(offset, 0, 0);
    //    Quaternion finalRotation = Quaternion.Euler(levelLocations[(int)currentLevel - 1]);//minus 1 as the LEVEL starts at 1

    //    if (durationOfSpin > 0f)
    //    {
    //        float startTime = Time.time;
    //        float endTime = startTime + durationOfSpin;

    //        yield return null;
    //        while (Time.time < endTime)
    //        {
    //            float progress = (Time.time - startTime) / durationOfSpin;
    //            //spin the earth
    //            earth.transform.rotation = Quaternion.Slerp(earth.transform.rotation, finalRotation, progress);
    //            //spin the skydome UVS
    //            skyDomeRender.material.mainTextureOffset = Vector2.Lerp(startingUV, endingUV, progress);
    //            yield return null;
    //        }
    //    }
    //    earth.transform.rotation = finalRotation;
    //    skyDomeRender.material.mainTextureOffset = endingUV;

    //    rotating = false;
    //}
}
