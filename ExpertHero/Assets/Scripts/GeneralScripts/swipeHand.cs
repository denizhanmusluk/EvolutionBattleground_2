using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class swipeHand : MonoBehaviour
{
    [SerializeField(), Range(0f, 5f)] private float moveFactors;
    [SerializeField(), Range(0f, 10f)] private float moveSpeed;
    [SerializeField(), Range(0f, 50f)] private float rotateSpeed;
    RectTransform rect;
    void Start()
    {
        rect = GetComponent<RectTransform>();
        //transform.DOMoveX(Screen.width/3f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        StartCoroutine(swipeMove());
    }
    IEnumerator swipeMove()
    {
        float counter = 0f;
        float value = 0;
        float counter2 = 0f;
        float value2 = 0;
        while (true)
        {
            counter += moveSpeed * Time.deltaTime;
            counter2 += Time.deltaTime;
            value = Mathf.Cos(counter);
            value2 = Mathf.Cos(counter2);
            value *= 120 * moveFactors;
            rect.localPosition = new Vector3(value, 0, 0);
            rect.parent.Rotate(0, 0, 20 * Time.deltaTime * rotateSpeed * value2);
            yield return null;
        }
    }
}
