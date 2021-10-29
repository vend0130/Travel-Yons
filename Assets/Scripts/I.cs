using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class I
{
    public static CharacterController c_Controller;
    public static GameManager gm;
    public static GMUI gmui;
    public static AudioManager audioManager;
    public static LevelUI levelUI;
    public static CameraFollow camera;
    public static LevelManager levelManager;
    public static AdMobManager adMob;
    public static SelectLevel selectLevel;
    public static MainmenuUI mainMenu;

    private static string path = "/gamedata.dat";

    public static bool full = false;
    public static bool firstRun = false;
    public static bool sfx = true;
    public static bool music = true;
    public static int coins = 0;
    public static int currentLevel = 0;
    public static DataLevels[] dataLevels;
    public static int yon = 0;
    public static bool[] yons;

    public static bool loadMenu = false;

    public static string instagramWebLink = "http://instagram.com/uniquetouch75/";
    public static string instagramAppLink = "instagram://user?username=uniquetouch75";
    public static string storeWebLink = "https://play.google.com/store/apps/developer?id=Unique+touch";
    public static string storeAppLink = "market://developer?=Unique+touch";
    public static string youtubeWebLink = "https://youtube.com/channel/UCZBkoJA2BnNpjCKidL53sUA";

    public static void Save()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = File.Create(Application.persistentDataPath + I.path);

        binaryFormatter.Serialize(fileStream, new GameData
        {
            full = I.full,
            firstRun = I.firstRun,
            sfx = I.sfx,
            music = I.music,
            coins = I.coins,
            currentLevel = I.currentLevel,
            dataLevels = I.dataLevels,
            yon = I.yon,
            yons = I.yons
        });

        fileStream.Close();
    }

    public static void Load()
    {
        if (File.Exists(Application.persistentDataPath + I.path))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = File.Open(Application.persistentDataPath + I.path, FileMode.OpenOrCreate);
            GameData gameData = (GameData)binaryFormatter.Deserialize(fileStream);

            fileStream.Close();

            I.full = gameData.full;
            I.firstRun = gameData.firstRun;
            I.sfx = gameData.sfx;
            I.music = gameData.music;
            I.coins = gameData.coins;
            I.currentLevel = gameData.currentLevel;
            I.dataLevels = gameData.dataLevels;
            I.yon = gameData.yon;
            I.yons = gameData.yons;
        }
    }

    public static void ClearSave()
    {
        using (Stream stream = File.Open(Application.persistentDataPath + I.path, FileMode.Create))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            GameData gameData = new GameData();
            binaryFormatter.Serialize(stream, gameData);
        }

        I.full = false;
        I.firstRun = false;
        I.sfx = true;
        I.music = true;
        I.coins = 0;
        I.currentLevel = 0;
        I.dataLevels = null;
        I.yon = 0;
        I.yons = null;
    }
}

[System.Serializable]
public class GameData
{
    public bool full = false;
    public bool firstRun = false;
    public bool sfx = true;
    public bool music = true;
    public int coins = 0;
    public int currentLevel = 0;
    public DataLevels[] dataLevels;
    public int yon = 0;
    public bool[] yons;
}

[System.Serializable]
public class DataLevels
{
    public int stars;
    public bool unlock = false;
    public List<string> coins = new List<string>();
}