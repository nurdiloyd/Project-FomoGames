using System;
using System.IO;
using UnityEngine;

namespace Main.Scripts
{
    public class DataManager : IContextUnit
    {
        private static readonly string PersistentPath = Application.persistentDataPath;
        private static readonly string LevelsPath = Path.Combine(PersistentPath, "Levels");
        
        public int CurrentLevelIndex { get; private set; }
        private int _totalLevelCount;
        
        public void Bind()
        {
            CopyLevelDataFromStreamingAssets();
            _totalLevelCount = Directory.GetFiles(LevelsPath, "*.json").Length;
        }
        
        private void CopyLevelDataFromStreamingAssets()
        {
            var destinationPath = LevelsPath;
            
            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }
            
            var levelsSourcePath = Path.Combine(Application.streamingAssetsPath, "Levels");
            var levelFiles = Directory.GetFiles(levelsSourcePath, "*.json");
            for (var i = 0; i < levelFiles.Length; i++)
            {
                var levelFile = levelFiles[i];
                var destinationFile = Path.Combine(destinationPath, Path.GetFileName(levelFile));
                
                if (!File.Exists(destinationFile))
                {
                    File.Copy(levelFile, destinationFile);
                }
            }
        }
        
        public void IncreaseCurrentLevelIndex()
        {
            CurrentLevelIndex = (CurrentLevelIndex + 1) % _totalLevelCount;
        }
        
        public LevelData GetCurrentLevelData()
        {
            var levelFiles = Directory.GetFiles(LevelsPath, "*.json");
            var jsonFile = File.ReadAllText(levelFiles[CurrentLevelIndex]);
            var levelData = JsonUtility.FromJson<LevelData>(jsonFile);
            return levelData;
        }
    }
    
    [Serializable]
    public struct LevelData
    {
        public int MoveLimit;
        public int RowCount;
        public int ColCount;
        public MovableInfo[] MovableInfo;
        public ExitInfo[] ExitInfo;
    }
    
    [Serializable]
    public struct MovableInfo
    {
        public int Row;
        public int Col;
        public int[] Direction;
        public int Length;
        public int Colors;
    }
    
    [Serializable]
    public struct ExitInfo
    {
        public int Row;
        public int Col;
        public int Direction;
        public int Colors;
    }
}
