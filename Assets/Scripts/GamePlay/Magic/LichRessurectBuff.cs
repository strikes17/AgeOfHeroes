namespace AgeOfHeroes.Spell
{
    public class LichRessurectBuff : RessurectBuff
    {
        protected override void ChangeHealthValueOfCharacter(ControllableCharacter character, int value)
        {
            character.ChangeHealthValue(value * _caster.Character.Count);
        }
    }
}