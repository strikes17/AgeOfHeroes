using System.Collections;
using Redcode.Moroutines;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace AgeOfHeroes
{
    public class ResourceTreasure : AbstractTreasure
    {
        private GameManager _gameManager;

        public override void Init()
        {
            base.Init();
            _gameManager = GameManager.Instance;
            _goldValue = _treasureObject.goldValue;
            _experienceValue = _treasureObject.experienceValue;
            _gemsValue = _treasureObject.gemsValue;
        }

        public override IEnumerator IEDestroy()
        {
            yield return base.IEDestroy();
        }

        public override void ShowDialogue(Hero heroCollector)
        {
            var dialogue = GUIDialogueFactory.CreateBasicChestTreasureDialogue();

            dialogue.goldGUIRoot.SetActive(false);
            dialogue.expGUIRoot.SetActive(false);
            dialogue.gemsGUIRoot.SetActive(false);
            dialogue.messageTMPText.text = string.Empty;
            heroCollector.Player.DisableInteractionWithWorld();
            dialogue.SetOnOkButtonEvent(() => { OnCollected(heroCollector); });
            if (GoldValue > 0)
            {
                dialogue.goldValueTMPText.text = GoldValue.ToString();
                dialogue.goldGUIRoot.SetActive(true);
                dialogue.SetOnGoldButtonEvent(() =>
                {
                    _chosenTreasureType = TreasureType.Gold;
                    dialogue.messageTMPText.text = $"Вы выбрали {GoldValue} золота";
                });
            }

            if (ExperienceValue > 0)
            {
                dialogue.expValueTMPText.text = ExperienceValue.ToString();
                dialogue.expGUIRoot.SetActive(true);
                dialogue.SetOnExpButtonEvent(() =>
                {
                    _chosenTreasureType = TreasureType.Experience;
                    dialogue.messageTMPText.text = $"Вы выбрали {ExperienceValue} опыта";
                });
            }

            if (GemsValue > 0)
            {
                dialogue.gemsValueTMPText.text = GemsValue.ToString();
                dialogue.gemsGUIRoot.SetActive(true);
                dialogue.SetOnGemsButtonEvent(() =>
                {
                    _chosenTreasureType = TreasureType.Gems;
                    dialogue.messageTMPText.text = $"Вы выбрали {GemsValue} камней душ";
                });
            }


            dialogue.Show();
        }

        public override void OnCollected(Hero hero)
        {
            base.OnCollected(hero);
            GivePlayerTreasure(hero);
            hero.MovementPointsLeft--;
        }

        public override void OnAICollected(Hero hero)
        {
            base.OnAICollected(hero);
            _chosenTreasureType = _gemsValue > 0 ? TreasureType.Gems :
                _goldValue > 0 ? TreasureType.Gold : TreasureType.Experience;
            GivePlayerTreasure(hero);
            // Debug.Log($"AI {hero.title} collected {_chosenTreasureType}");
            hero.MovementPointsLeft--;
        }

        private void GivePlayerTreasure(Hero hero)
        {
            var player = hero.Player;
            player.EnableInteractionWithWorld();
            if (_chosenTreasureType == TreasureType.Gold)
                player.Gold += GoldValue;
            else if (_chosenTreasureType == TreasureType.Gems)
                player.Gems += GemsValue;
            else if (_chosenTreasureType == TreasureType.Experience)
                hero.TotalExperience += ExperienceValue;

            // hero.PlayableTerrain.SpawnedTreasures.Remove(this);
            gameObject.SetActive(false);
            Moroutine.Run(IEDestroy());
            // Destroy(gameObject);
            // Debug.Log($"{hero.title} g: {_goldValue} | gems {_gemsValue} | exp {_experienceValue}");
        }
    }
}