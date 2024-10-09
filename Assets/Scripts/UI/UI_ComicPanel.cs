using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ComicPanel : MonoBehaviour, IPointerDownHandler
{
    private Image myImage;

    [SerializeField] private Image[] comicPanel;
    [SerializeField] GameObject buttonToEnable;

    private bool comicShowOver;
    private int imageIndex;

    private void Start()
    {
        myImage = GetComponent<Image>();
        ShowNextImage();
    }

    private void ShowNextImage()
    {
        if (comicShowOver)
            return;

        StartCoroutine(ChangeImageAlpha(1, 1.5f, ShowNextImage));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ShowNextImageOnClick();
    }

    private void ShowNextImageOnClick()
    {
        if (imageIndex >= comicPanel.Length)
            return;

        comicPanel[imageIndex].color = Color.white;
        imageIndex++;

        if (imageIndex >= comicPanel.Length)
            FinishComicShow();

        if (comicShowOver)
            return;

        ShowNextImage();
    }

    private IEnumerator ChangeImageAlpha(float _targetAlpha, float _duration, System.Action onComplete)
    {
        float time = 0;

        Color currentColor = comicPanel[imageIndex].color;
        float startAlpha = currentColor.a;

        while (time <= _duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, _targetAlpha, time / _duration);

            comicPanel[imageIndex].color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            //这个代表这帧暂停，下一帧又开始，用于确保alpha通道的改变是逐渐的而不是瞬时的
            //这种用法很常见
            yield return null;
        }

        comicPanel[imageIndex].color = new Color(currentColor.r, currentColor.g, currentColor.b, _targetAlpha);

        imageIndex++;

        if (imageIndex >= comicPanel.Length)
        {
            FinishComicShow();
        }

        onComplete?.Invoke();
    }

    private void FinishComicShow()
    {
        StopAllCoroutines();
        comicShowOver = true;
        buttonToEnable.SetActive(true);
        myImage.raycastTarget = false;
    }

}
