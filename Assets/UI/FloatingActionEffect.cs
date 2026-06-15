using UnityEngine;
using UnityEngine.UIElements;

public class FloatingActionEffect : MonoBehaviour
{
    private VisualElement rootContainer;

    private void Awake()
    {
        rootContainer = GetComponent<UIDocument>().rootVisualElement;
    }

    public void SpawnActionGlyphEffect(string actionText)
    {
        Label floatingLabel = new Label($"▲ {actionText.ToUpper()}");
        floatingLabel.style.position = Position.Absolute;
        
        // Target center area screen positioning space coordinates
        floatingLabel.style.left = Length.Percent(45);
        floatingLabel.style.top = Length.Percent(50);
        floatingLabel.style.color = new Color(0f, 0.95f, 1f, 1f);
        floatingLabel.style.fontSize = 24;
        
        rootContainer.Add(floatingLabel);
        StartCoroutine(AnimateFloatingGlyph(floatingLabel));
    }

    private System.Collections.IEnumerator AnimateFloatingGlyph(Label element)
    {
        float duration = 2.0f;
        float elapsed = 0f;
        float startTopPercent = 50f;
        float travelDistance = 25f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float normalizedProgress = elapsed / duration;

            // Translate layout upward over time
            float currentTop = startTopPercent - (normalizedProgress * travelDistance);
            element.style.top = Length.Percent(currentTop);
            
            // Fade opacity out smoothly
            element.style.opacity = Mathf.Lerp(1f, 0f, normalizedProgress);
            yield return null;
        }

        rootContainer.Remove(element);
    }
}