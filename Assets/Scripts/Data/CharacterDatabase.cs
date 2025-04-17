using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/CharacterDatabase")]
public class CharacterDatabase : ScriptableObject {
    [SerializeField] private List<CharacterData> characterList;

    private Dictionary<string, CharacterData> characterMap;

    public void Initialize() {
        characterMap = new Dictionary<string, CharacterData>();
        Debug.Log("Initialized CharacterDatabase");
        foreach (var data in characterList) {
            if (!string.IsNullOrEmpty(data.characterId)) {
                characterMap[data.characterId] = data;
            }
        }
    }

    public CharacterData GetCharacterDataById(string id) {
        characterMap.TryGetValue(id, out var data);
        return data;
    }

    public List<CharacterData> GetAll()
    {
        return characterList;
    }
    public bool Has(string id) => characterMap.ContainsKey(id);
}