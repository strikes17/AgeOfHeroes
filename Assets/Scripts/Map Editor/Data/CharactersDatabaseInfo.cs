using System;
using System.Collections.Generic;

namespace AgeOfHeroes.MapEditor
{
    public class CharactersDatabaseInfo
    {
        public Dictionary<Fraction, List<string>> Characters = new Dictionary<Fraction, List<string>>()
        {
            {Fraction.Human, new List<string>()},
            {Fraction.Undead, new List<string>()},
            {Fraction.Inferno, new List<string>()},
            {Fraction.Mages, new List<string>()},
            {Fraction.None, new List<string>()},
        };
        
        public Dictionary<Fraction, List<string>> Heroes = new Dictionary<Fraction, List<string>>()
        {
            {Fraction.Human, new List<string>()},
            {Fraction.Undead, new List<string>()},
            {Fraction.Inferno, new List<string>()},
            {Fraction.Mages, new List<string>()},
            {Fraction.None, new List<string>()},
        };
    }
}