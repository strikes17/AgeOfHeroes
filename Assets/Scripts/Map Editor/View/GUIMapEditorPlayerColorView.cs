using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfHeroes.MapEditor
{
    public class GUIMapEditorPlayerColorView : MonoBehaviour
    {
        [SerializeField] private GUIPlayerColorSelectorWidget _playerColorSelectorWidgetPrefab;
        [SerializeField] private Transform _contentTransform;
        [SerializeField] private Image _playerColorToolButtonImage;

        private void Awake()
        {
            var bannersCount = GlobalVariables.playerBanners.Count;
            var playerBannersColors = GlobalVariables.playerBanners.Keys.ToList();
            for (int i = 0; i < bannersCount; i++)
            {
                var playerColorWidgetInstance = GameObject.Instantiate(_playerColorSelectorWidgetPrefab, Vector3.zero, Quaternion.identity, _contentTransform);
                playerColorWidgetInstance.Set(playerBannersColors[i], _playerColorToolButtonImage);
            }
        }

        public void SwitchState()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}