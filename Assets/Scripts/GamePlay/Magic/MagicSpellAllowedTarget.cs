namespace AgeOfHeroes.Spell
{
    public enum MagicSpellAllowedTarget
    {
        Ally = 1,
        Enemy = 2,
        Building = 4,
        Mechanism = 8,
        Alive = 16,
        Undead = 32,
        Summon = 64,
        Hero = 128,
        Empty = 256,
        Prop = 512,
        Corpse = 1024,
        Tree = 2048,
        UndeadCorpse = 4096,
        AliveCorpse = 8192,
        Any = 16384
    }
}