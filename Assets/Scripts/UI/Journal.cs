using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Journal : MonoBehaviour
{
    [SerializeField] float turningSpeed = 0.5f;
    [SerializeField] List<Transform> pages;
    int index = -1;
    bool rotating = false;

    public void rotateNext()
    {
        if (index >= pages.Count - 1) return;

        index++;
        pages[index].SetAsLastSibling();
        float angle = -180;
        StartCoroutine(Rotation(angle, true));
    }


    public void rotatePrevious()
    {
        if (index < 0) return;

        pages[index].SetAsLastSibling();
        float angle = 0;
        StartCoroutine(Rotation(angle, false));
    }

    IEnumerator Rotation(float angle, bool next)
    {
        float value = 0f;
        while (true)
        {
            rotating = true;
            Quaternion targetRotation = Quaternion.Euler(0, angle, 0); //rotation along the Y Axis
            value += Time.deltaTime * turningSpeed;
            pages[index].rotation = Quaternion.Slerp(pages[index].rotation, targetRotation, value);
            float angledis = Quaternion.Angle(pages[index].rotation, targetRotation);
            if (angledis < 0.1f)
            {
                if (next == false)
                {
                    index--;
                }
                rotating = false;
                break;
            }

        yield return null;

        }
    }

}
