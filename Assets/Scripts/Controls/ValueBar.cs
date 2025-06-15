using System;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
//using TMPro;

public class ValueBar : MonoBehaviour
{
    //public TextMeshProUGUI textDisplay;
    [SerializeField]
    public TextMeshProUGUI textDisplay;
    public int maxValue = 0;
    public int currentValue = 0;
    public float maxSize = 0f;
    public RectTransform valueBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (textDisplay == null)
        {
            textDisplay = GetComponentInChildren<TextMeshProUGUI>();
        }
        if (textDisplay == null)
        {
            Debug.Log(string.Format("TextMeshProUGUI not found in childre of {0}.", this.name));
        } else
        {
            Debug.Log(string.Format("TextMeshProUGUI found in childre of {0}.", this.name));
        }
    }

    // Update is called once per frame
    void Update()
    {
        updateVisualBar();
        updateTextDisplay();
    }

    private void updateTextDisplay()
    {
        this.textDisplay.text = String.Format("{0} / {1}", currentValue, maxValue);
    }

    private void updateVisualBar()
    {
        float currentSize = (maxValue > 0) ? (currentValue / (float)maxValue) * maxSize : 0f;
        float valuePercentage = currentValue / (float)maxValue;
        float parentWidth = maxSize; //valueBar.parent.GetComponent<RectTransform>().rect.width;
        valueBar.sizeDelta = new Vector2(parentWidth * valuePercentage, valueBar.sizeDelta.y);
        //UpdateHealthBar(ref valueBar);
    }

    private void UpdateHealthBar(ref RectTransform healthBarFillRect)
    {
        if (healthBarFillRect == null)
        {
            Debug.LogError("Health Bar Fill RectTransform is not assigned!", this);
            return;
        }

        // Calculate the health percentage
        float healthPercentage = currentValue / (float)maxValue;

        // --- Methods to change the width based on your RectTransform setup ---

        // METHOD 1: If your Image is anchored to a single point (e.g., top-left, center)
        // and you're controlling its absolute Width/Height directly.
        // This is suitable if the health bar's *parent* has a fixed width.
        float parentWidth = healthBarFillRect.parent.GetComponent<RectTransform>().rect.width;
        healthBarFillRect.sizeDelta = new Vector2(parentWidth * healthPercentage, healthBarFillRect.sizeDelta.y);

        // METHOD 2: If your Image is anchored to the LEFT and its pivot is also LEFT (0, 0.5)
        // This is the most common and recommended way for a growing/shrinking health bar.
        // We set the 'anchorMax.x' to the health percentage, which makes the right anchor move.
        // And then ensure 'offsetMax.x' (which is 'Right' in the Inspector) is 0 to stretch to that anchor.

        // Get current anchors. Assuming the Y anchors are fixed (e.g., 0.5, 0.5)
        Vector2 currentMinAnchor = healthBarFillRect.anchorMin;
        Vector2 currentMaxAnchor = healthBarFillRect.anchorMax;

        // Update the max X anchor to represent the health percentage
        currentMaxAnchor.x = healthPercentage;

        healthBarFillRect.anchorMin = currentMinAnchor; // Keep min X anchor (should be 0 for left-grow)
        healthBarFillRect.anchorMax = currentMaxAnchor; // Set max X anchor to health percentage

        // Crucially, set the offsets to zero after changing anchors if you want it to stretch perfectly
        // from the left anchor (0) to the new right anchor (healthPercentage)
        healthBarFillRect.offsetMin = new Vector2(0, healthBarFillRect.offsetMin.y); // Left, Bottom
        healthBarFillRect.offsetMax = new Vector2(0, healthBarFillRect.offsetMax.y); // Right, Top
        // In the Inspector, offsetMin.x is "Left" and offsetMax.x is "Right".
        // Setting them to 0 means "stretch exactly to the anchors".

        // --- Optional: Change color based on health ---
        // if (healthBarImage != null)
        // {
        //     if (healthPercentage > 0.6f) healthBarImage.color = Color.green;
        //     else if (healthPercentage > 0.3f) healthBarImage.color = Color.yellow;
        //     else healthBarImage.color = Color.red;
        // }
    }
}
