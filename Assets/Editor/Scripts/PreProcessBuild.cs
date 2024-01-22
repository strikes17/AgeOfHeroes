using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace AgeOfHeroes.Editor
{
    class PreProcessBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder { get { return 0; } }
        public void OnPreprocessBuild(BuildReport report)
        {
            EditorFunctionTools.CreateMapsInfoContainer();
            EditorFunctionTools.CreateCharactersDatabaseInfo();
            EditorFunctionTools.CreateTreasuresDatabaseInfo();
            EditorFunctionTools.CreateTerrainDatabaseInfo();
            EditorFunctionTools.CreateArtifactsDatabaseInfo();
        }
    }
}