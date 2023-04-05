using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public IEnumerator FadeToBlack(float time)
    {
        yield return image.DOFade(1, time).WaitForCompletion();
    }

    public IEnumerator FadeIn(float time)
    {
        yield return image.DOFade(0, time).WaitForCompletion();
    }
}