using System;
using System.IO;
using UnityEngine;

namespace Main.Scripts.General
{
    public class DataManager : IContextUnit
    {
        private static readonly string PersistentPath = Application.persistentDataPath;
        private static readonly string LevelsPath = Path.Combine(PersistentPath, "Levels");
        
        private int _totalLevelCount;
        public UserData User { get; private set; }
        
        public void Bind()
        {
            CopyLevelDataFromStreamingAssets();
            _totalLevelCount = Directory.GetFiles(LevelsPath, "*.json").Length;
            LoadUserData();
        }

        private void LoadUserData()
        {
            var destinationPath = PersistentPath;
            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }
            
            var userDataPath = Path.Combine(destinationPath, "UserData.json");
            if (File.Exists(userDataPath))
            {
                var jsonFile = File.ReadAllText(userDataPath);
                var userData = JsonUtility.FromJson<UserData>(jsonFile);
                User = userData;
            }
            else
            {
                User = new UserData();
                SaveUserData();
            }
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
            User.levelIndex = (User.levelIndex + 1) % _totalLevelCount;
            SaveUserData();
        }
        
        private void SaveUserData()
        {
            var destinationPath = PersistentPath;
            var userDataPath = Path.Combine(destinationPath, "UserData.json");
            var jsonFile = JsonUtility.ToJson(User);
            File.WriteAllText(userDataPath, jsonFile);
        }
        
        public LevelData GetCurrentLevelData()
        {
            var levelFiles = Directory.GetFiles(LevelsPath, "*.json");
            var jsonFile = File.ReadAllText(levelFiles[User.levelIndex]);
            var levelData = JsonUtility.FromJson<LevelData>(jsonFile);
            return levelData;
        }
    }
    
    [Serializable]
    public class UserData
    {
        public int levelIndex;
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
