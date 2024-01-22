using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace AgeOfHeroes.MapEditor
{
    public class GUIMapEditorTreasuresView : MonoBehaviour
    {
        [SerializeField] private GUIMapEditorTreasureWidget _treasureWidgetPrefab;
        [SerializeField] private GUIMapEditorArtifactWidget _artifactWidgetPrefab;
        [SerializeField] private Transform _treasuresContent;
        private List<TreasureObject> _treasureObjects = new List<TreasureObject>();
        private List<ArtifactObject> _artifactObjects = new List<ArtifactObject>();
        private List<GUIMapEditorTreasureWidget> _treasureWidgets = new List<GUIMapEditorTreasureWidget>();
        private List<GUIMapEditorArtifactWidget> _artifactWidgets = new List<GUIMapEditorArtifactWidget>();

        public void Show()
        {
            TryLoadTreasures();
            TryLoadArtifacts();
            UpdateGUI();
            gameObject.SetActive(true);
        }

        private void TryLoadTreasures()
        {
            if (_treasureObjects.Count <= 0)
            {
                var jsonSettings = GlobalVariables.GetDefaultSerializationSettings();
                var treasuresDatabaseInfoFile = Resources.Load<TextAsset>(GlobalStrings.TREASURES_DATABASE_RESOURCES_PATH).text;
                var treasuresDatabaseInfo = JsonConvert.DeserializeObject<TreasuresDatabaseInfo>(treasuresDatabaseInfoFile, jsonSettings);
                treasuresDatabaseInfo.Treasures.ForEach(x =>
                {
                    var treasureObject = ResourcesBase.GetTreasureObject(x);
                    if (treasureObject != null)
                        _treasureObjects.Add(treasureObject);
                });
            }
        }

        private void TryLoadArtifacts()
        {
            if (_artifactWidgets.Count <= 0)
            {
                var jsonSettings = GlobalVariables.GetDefaultSerializationSettings();
                var artifactDatabaseInfoFile = Resources.Load<TextAsset>(GlobalStrings.ARTIFACTS_DATABASE_RESOURCES_PATH).text;
                var artifactsDatabaseInfo = JsonConvert.DeserializeObject<ArtifactsDatabaseInfo>(artifactDatabaseInfoFile, jsonSettings);
                artifactsDatabaseInfo.Artifacts.ForEach(x =>
                {
                    var artifactObject = ResourcesBase.GetArtifactObject(x);
                    if (artifactObject != null)
                        _artifactObjects.Add(artifactObject);
                });
            }
        }

        public void ShowForTreasures()
        {
            _artifactWidgets.ForEach(x => x.gameObject.SetActive(false));
            _treasureWidgets.ForEach(x => x.gameObject.SetActive(true));
        }

        public void ShowForArtifacts()
        {
            _artifactWidgets.ForEach(x => x.gameObject.SetActive(true));
            _treasureWidgets.ForEach(x => x.gameObject.SetActive(false));
        }

        private void UpdateGUI()
        {
            if (_treasureWidgets.Count <= 0)
                foreach (var treasureObject in _treasureObjects)
                {
                    var treasureWidgetInstance = GameObject.Instantiate(_treasureWidgetPrefab, Vector3.zero, Quaternion.identity, _treasuresContent).GetComponent<GUIMapEditorTreasureWidget>();
                    treasureWidgetInstance.TreasureObject = treasureObject;
                    treasureWidgetInstance.SelectButton.onClick.AddListener(() => SelectTreasureObject(treasureObject));
                    _treasureWidgets.Add(treasureWidgetInstance);
                }
            
            if (_artifactWidgets.Count <= 0)
                foreach (var artifactObject in _artifactObjects)
                {
                    var artifactWidgetInstance = GameObject.Instantiate(_artifactWidgetPrefab, Vector3.zero, Quaternion.identity, _treasuresContent).GetComponent<GUIMapEditorArtifactWidget>();
                    artifactWidgetInstance.ArtifactObject = artifactObject;
                    artifactWidgetInstance.SelectButton.onClick.AddListener(() => SelectArtifactObject(artifactObject));
                    _artifactWidgets.Add(artifactWidgetInstance);
                }
        }

        private void SelectTreasureObject(TreasureObject treasureObject)
        {
            MapEditorManager.Instance.SelectedTreasureObject = treasureObject;
        }
        
        private void SelectArtifactObject(ArtifactObject artifactObject)
        {
            MapEditorManager.Instance.SelectedArtifactObject = artifactObject;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}