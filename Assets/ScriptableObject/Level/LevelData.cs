using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    public string levelName; // matches a scene name
    public int wavesToWin;
    public int startingResoruces;
    public int startingLives;

    //public AudioClip backgroundMusic;
}
