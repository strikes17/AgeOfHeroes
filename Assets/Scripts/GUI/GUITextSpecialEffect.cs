using System.Collections;
using Redcode.Moroutines;
using TMPro;
using UnityEngine;

namespace AgeOfHeroes
{
    public class GUITextSpecialEffect : MonoBehaviour
    {
        private TMP_Text _text;
        private Color _startColor;
        private int outlineThicknessId = Shader.PropertyToID("_OutlineWidth");
        private int outlineColorId = Shader.PropertyToID("_OutlineColor");
        private bool hasEffectRunning = false;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        public void Flicker(float targetThickness, Color color, float time, int repeats, bool forceEffect)
        {
            if (hasEffectRunning && !forceEffect) return;
            var thickness = _text.fontMaterial.GetFloat(outlineThicknessId);
            _text.fontMaterial.SetFloat(outlineThicknessId, thickness);
            _text.fontMaterial.SetColor(outlineColorId, _startColor);
            Moroutine.Run(IEFlickerEffect(thickness, targetThickness, color, time, repeats));
        }

        private IEnumerator IEFlickerEffect(float startThickness, float targetThickness, Color targetColor, float time, int repeats)
        {
            hasEffectRunning = true;
            for (int i = 0; i < repeats; i++)
            {
                float t = 0f;
                float thickness = startThickness;
                float progress = 0f;
                Color color = _startColor;
                while (progress < 1f)
                {
                    t += Time.deltaTime;
                    progress = t / time;
                    float progressX2 = progress * 2;
                    if (progress < 0.5f)
                    {
                        thickness = Mathf.Lerp(thickness, targetThickness, progressX2);
                        color = Color.Lerp(color, targetColor, progressX2);
                    }
                    else
                    {
                        thickness = Mathf.Lerp(thickness, startThickness, progressX2);
                        color = Color.Lerp(color, _startColor, progressX2);
                    }

                    _text.fontMaterial.SetFloat(outlineThicknessId, thickness);
                    _text.fontMaterial.SetColor(outlineColorId, color);
                    yield return null;
                }
            }

            hasEffectRunning = false;
        }
    }
}