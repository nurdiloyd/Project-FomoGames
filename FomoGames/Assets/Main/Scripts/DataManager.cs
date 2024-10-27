using System;
using System.IO;
using UnityEngine;

namespace Main.Scripts
{
    public class DataManager
    {
        private static readonly string PersistentPath = Application.persistentDataPath;
        private static readonly string LevelsPath = Path.Combine(PersistentPath, "Levels");
        
        private LevelData[] _levelData;
        private int _currentLevelIndex;
        
        public void Bind()
        {
            CopyLevelDataFromStreamingAssets();
            LoadLevelData();
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
        
        private void LoadLevelData()
        {
            var levelFiles = Directory.GetFiles(LevelsPath, "*.json");
            _levelData = new LevelData[levelFiles.Length];
            
            for (var i = 0; i < levelFiles.Length; i++)
            {
                var jsonFile = File.ReadAllText(levelFiles[i]);
                var levelData = JsonUtility.FromJson<LevelData>(jsonFile);
                _levelData[i] = levelData;
            }
        }
        
        public LevelData GetCurrentLevelData()
        {
            return _levelData[_currentLevelIndex];
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
