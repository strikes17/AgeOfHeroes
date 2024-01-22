using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace AgeOfHeroes
{

    public class GUISavedGameWidget : GUIBaseWidget
    {
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text mapNameText, lastDateText, saveNameText;
        private Button button;
        private string targetFileName;

        public Sprite Icon
        {
            set
            {
                image.sprite = value;
            }
        }

        public string MapNameText { set => mapNameText.text = value; }
        public string LastDateText { set => lastDateText.text = value; }
        public string SaveNameText { set => saveNameText.text = value; }
        public Button Button { get => button; }
        public string TargetFileName { get => targetFileName; set => targetFileName = value; }

        public void VisualSelectionOn()
        {
            button.image.color = Color.green;
        }

        public void VisualSelectionOff()
        {
            button.image.color = Color.white;
        }

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() =>
            {

            });
        }
    }
}