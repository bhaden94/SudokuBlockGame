using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[System.Serializable]
public class SquareTextureData : ScriptableObject
{
    [System.Serializable]
    public class TextureData
    {
        public Sprite texture;
        public Config.SquareColor squareColor;
    }
    // used to switch to another color
    public int thresholdVal = 10;
    private const int START_THRESHOLD_VALUE = 10;
    public List<TextureData> activeSquareTextures;

    public Config.SquareColor currentColor;
    private Config.SquareColor _nextColor;

    public int GetCurrentColorIndex()
    {
        var currentIndex = 0;

        for (int i = 0; i < activeSquareTextures.Count; i++)
        {
            if (activeSquareTextures[i].squareColor == currentColor)
            {
                currentIndex = i;
            }
        }

        return currentIndex;
    }

    public void UpdateColor(int currentScore)
    {
        currentColor = _nextColor;
        var currentColorIndex = GetCurrentColorIndex();

        // if on last color, reset colors
        if(currentColorIndex == activeSquareTextures.Count - 1)
        {
            _nextColor = activeSquareTextures[0].squareColor;
        }
        else
        {
            _nextColor = activeSquareTextures[currentColorIndex + 1].squareColor;
        }

        thresholdVal = START_THRESHOLD_VALUE + currentScore;
    }

    public void SetStartColor()
    {
        thresholdVal = START_THRESHOLD_VALUE;
        currentColor = activeSquareTextures[0].squareColor;
        _nextColor = activeSquareTextures[1].squareColor;
    }

    private void Awake()
    {
        SetStartColor();
    }

    private void OnEnable()
    {
        SetStartColor();
    }
}
