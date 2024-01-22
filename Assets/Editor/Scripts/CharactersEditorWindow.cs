using System;
using System.IO;
using AgeOfHeroes.MapEditor;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace AgeOfHeroes.Editor
{
    public class CharactersEditorWindow : EditorWindow
    {
        public static void AddHeroes(CharactersDatabaseInfo charactersDatabaseInfo)
        {
            var fractionsStrings = Enum.GetNames(typeof(Fraction));
            foreach (var fractionString in fractionsStrings)
            {
                string path = $"{Application.dataPath}/Resources/Heroes/{fractionString}";
                bool isDirectoryExists = Directory.Exists(path);
                if (!isDirectoryExists)
                    continue;
                var directoryInfo = new DirectoryInfo(path);
                var heroesFiles = directoryInfo.GetFiles("*.asset", SearchOption.TopDirectoryOnly);
                foreach (var heroFile in heroesFiles)
                {
                    string heroName = Path.GetFileNameWithoutExtension(heroFile.Name);
                    var fraction = Enum.Parse<Fraction>(fractionString);
                    charactersDatabaseInfo.Heroes[fraction].Add(heroName);
                }
            }
        }
    }
}