using UnityEngine;
using System;

[Serializable] // This attribute is MANDATORY for JSON to work
public class SaveData
{
    public int score;
    public float playerHealth;

    // Unity's JsonUtility cannot save Vector3 directly, so we use floats
    public float posX, posY, posZ;

    // A constructor to make creating save objects easy
    public SaveData(int _score, float _health, Vector3 _pos)
    {
        score = _score;
        playerHealth = _health;
        posX = _pos.x;
        posY = _pos.y;
        posZ = _pos.z;
    }
}