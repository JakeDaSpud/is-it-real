using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuBackground : MonoBehaviour
{
    public Image background; //The UI component
    public Sprite[] frames; //Frames for animation 
    public float frameRate = .1f; //Second Per Frame

    private int currentFrame;
    private float timer;



    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= frameRate)
        {
            timer -= frameRate;
            currentFrame = (currentFrame + 1) % frames.Length;
            background.sprite = frames[currentFrame];
        }
    }
}
