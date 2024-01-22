using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace AgeOfHeroes
{
    public class GUICharacterStatsInfo : MonoBehaviour
    {
        [SerializeField] private GUICharacterStatWidget _attackWidget,
            _defenseWidget,
            _healthWidget,
            _damageWidget,
            _growthWidget,
            _stackCountWidget,
            _manaWidget,
            _movePointsWidget;

        [SerializeField] private GUICharacterStatWidget _attackRangeWidget;

        [SerializeField] private GUICharacterStatWidget _attacksCountWidget,
            _retaliationCountWidget,
            _visionRadiusWidget;

        [SerializeField] private TMP_Text _quantityInfo, _descriptionInfo;

        public void SetGUIInfo(ControllableCharacter controllableCharacter)
        {
            var type = controllableCharacter.GetType();
            if (type == typeof(Character))
            {
                var character = (Character) controllableCharacter;
                if (character != null)
                {
                    _stackCountWidget.Show();
                    _stackCountWidget.Text = $"{controllableCharacter.Count} / {character.CharacterObject.fullStackCount}";
                    // _growthWidget.Text = character.CharacterObject.baseGrowthPerDay.ToString();
                }
            }
            else if (type == typeof(Hero))
            {
                _stackCountWidget.Hide();
            }

            _growthWidget.Hide();

            _descriptionInfo.text = controllableCharacter.description;
            _quantityInfo.text = $"{controllableCharacter.Count.ToString()}";
            
            bool isWizard = controllableCharacter.isWizard;
            if (isWizard)
                _manaWidget.Text = $"{controllableCharacter.ManaLeft} / {controllableCharacter.startingMana}";
            else
                _manaWidget.Text = "-";
            // _movePointsWidget.Text = $"{controllableCharacter.MovementPointsLeft} / {controllableCharacter.startingMovementPoints}";
            SetWidgetInfo(_attackRangeWidget, controllableCharacter.attackRange, 0);
            SetWidgetInfo(_movePointsWidget, controllableCharacter.startingMovementPoints, controllableCharacter.MaxMovementDifference, controllableCharacter.MovementPointsLeft);
            SetWidgetInfo(_healthWidget, controllableCharacter.baseHealth, controllableCharacter.HealthDifference, controllableCharacter.HealthLeft);
            SetWidgetInfo(_attackWidget, controllableCharacter.baseAttackValue, controllableCharacter.AttackDifference);
            SetWidgetInfo(_damageWidget, controllableCharacter.baseDamageValue, controllableCharacter.DamageDifference);
            SetWidgetInfo(_defenseWidget, controllableCharacter.baseDefenseValue, controllableCharacter.DefenseDifference);
            SetWidgetInfo(_visionRadiusWidget, controllableCharacter.baseVisionRadius, controllableCharacter.VisionDifference);
            _attacksCountWidget.Text = controllableCharacter.AttacksLeft.ToString();
            _retaliationCountWidget.Text = controllableCharacter.RetilationsLeft.ToString();
        }

        private void SetWidgetInfo(GUICharacterStatWidget statWidget, int baseValue, int difference)
        {
            string differenceSign = difference > 0 ? "+" : string.Empty;
            string differenceColor = difference > 0 ? "<color=green>" : difference < 0 ? "<color=red>" : string.Empty;
            if (difference > 0 || difference < 0)
                statWidget.Text = $"{baseValue} {differenceColor}({differenceSign}{difference})</color>";
            else
                statWidget.Text = $"{baseValue}";
        }

        private void SetWidgetInfo(GUICharacterStatWidget statWidget, float baseValue, float difference)
        {
            string differenceSign = difference > 0 ? "+" : string.Empty;
            string differenceColor = difference > 0 ? "<color=green>" : difference < 0 ? "<color=red>" : string.Empty;
            if (difference > 0 || difference < 0)
                statWidget.Text = $"{baseValue} {differenceColor}({differenceSign}{difference})</color>";
            else
                statWidget.Text = $"{baseValue}";
        }

        private void SetWidgetInfo(GUICharacterStatWidget statWidget, int baseValue, int difference, int leftValue)
        {
            string differenceSign = difference > 0 ? "+" : string.Empty;
            string differenceColor = difference > 0 ? "<color=green>" : difference < 0 ? "<color=red>" : string.Empty;
            if (difference > 0 || difference < 0)
                statWidget.Text = $"{leftValue}/{baseValue} {differenceColor}({differenceSign}{difference})</color>";
            else
                statWidget.Text = $"{leftValue}/{baseValue}";
        }

        public void SetGUIInfo(CharacterObject characterObject, CastleTierIncomeValues incomeValues)
        {
            _quantityInfo.text = $"{characterObject.fullStackCount.ToString()}";
            _descriptionInfo.text = characterObject.description;
            _healthWidget.Text = characterObject.startingHealth.ToString();
            _attackWidget.Text = characterObject.attackValue.ToString();
            _damageWidget.Text = characterObject.damageValue.ToString();
            _attacksCountWidget.Text = characterObject.attacksCount.ToString();
            _defenseWidget.Text = characterObject.defenseValue.ToString();
            _movePointsWidget.Text = characterObject.startingMovementPoints.ToString();
            
            _manaWidget.Text = characterObject.startingMana.ToString();
            _stackCountWidget.Text = characterObject.fullStackCount.ToString();
            var baseGrowthPerDay = characterObject.baseGrowthPerDay;
            float growthDifference = baseGrowthPerDay * incomeValues.growth;
            SetWidgetInfo(_growthWidget, characterObject.baseGrowthPerDay, growthDifference);
            _visionRadiusWidget.Text = characterObject.visionRadius.ToString();
            _retaliationCountWidget.Text = characterObject.retilationsCount.ToString();
            SetWidgetInfo(_attackRangeWidget, characterObject.attackRange, 0);

            bool isWizard = characterObject.isWizard;
            if (isWizard)
                _manaWidget.Text = characterObject.startingMana.ToString();
            else
                _manaWidget.Text = "-";
        }
    }
}