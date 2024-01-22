using System;
using System.Collections;
using System.Collections.Generic;
using Redcode.Moroutines;
using UnityEngine;
using UnityEngine.UI;

public class GUISpriteAnimation : MonoBehaviour
{
    public List<Sprite> animationSequence;
    private Image _image;
    private float updateTime, timeForUpdate;
    private int currentFrameIndex = 0;
    private int sequenceLength = 0;
    private bool isPlaying = false;
    private bool isReversed = false;
    private bool isLooped = false;

    public delegate void OnGUISpriteAnimationDelegate();

    private OnGUISpriteAnimationDelegate _onSequenceCompleted;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _image.sprite = animationSequence[currentFrameIndex];
        sequenceLength = animationSequence.Count;
    }

    public void PlayOneShot(float fps, OnGUISpriteAnimationDelegate onSequenceCompleted = null)
    {
        _onSequenceCompleted = onSequenceCompleted;
        currentFrameIndex = 0;
        timeForUpdate = 1f / fps;
        isReversed = false;
        isPlaying = true;
    }

    public void PlayOneShotReversed(float fps, OnGUISpriteAnimationDelegate onSequenceCompleted = null)
    {
        _onSequenceCompleted = onSequenceCompleted;
        currentFrameIndex = sequenceLength - 1;
        timeForUpdate = 1f / fps;
        isReversed = true;
        isPlaying = true;
    }

    public void PlayLooped(float fps)
    {
        currentFrameIndex = 0;
        timeForUpdate = 1f / fps;
        isReversed = false;
        isLooped = true;
        isPlaying = true;
    }

    public void Stop()
    {
        _onSequenceCompleted?.Invoke();
        isPlaying = false;
        timeForUpdate = 0f;
        currentFrameIndex = 0;
    }

    private void Update()
    {
        if (!isPlaying)
            return;
        updateTime += Time.deltaTime;
        if (updateTime < timeForUpdate)
            return;
        updateTime = 0;
        currentFrameIndex = isReversed ? currentFrameIndex - 1 : currentFrameIndex + 1;
        if ((currentFrameIndex >= sequenceLength || currentFrameIndex < 0) && !isLooped)
        {
            Stop();
            return;
        }

        if (isLooped)
        {
            if (currentFrameIndex >= sequenceLength)
                currentFrameIndex = 0;
        }

        _image.sprite = animationSequence[currentFrameIndex];
    }
}