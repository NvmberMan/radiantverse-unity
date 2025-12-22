using UnityEngine;

[CreateAssetMenu(menuName = "Menu/CharacterModel")]
public class CharacterModel : ScriptableObject
{
    public string characterName;
    public Sprite portrait;
    public RuntimeAnimatorController animatorController;
    public Vector3 previewPosition;
}
