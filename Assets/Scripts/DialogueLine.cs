using UnityEngine;

[System.Serializable]
public struct DialogueLine {
    public string line;
    public string characterName;
    public Sprite characterImage;

    public bool useCenterPortrait;

    public bool isSmile;
}
