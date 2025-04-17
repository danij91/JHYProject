using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterInventory {
    private static CharacterInventory instance = null;
    public static CharacterInventory Instance {
        get {
            if (instance == null) {
                instance = new CharacterInventory();
            }

            return instance;
        }
    }

    private CharacterDatabase characterDatabase;
    private string characterDatabasePath = "Data/CharacterDatabase";
    public string MainCharacterId { get; private set; }

    // 현재 보유한 캐릭터 ID 리스트
    private List<string> validCharacterIds = new List<string>();

    public void Initialize() {
        characterDatabase = ResourceManager.Instance.Load<CharacterDatabase>(characterDatabasePath);
        characterDatabase.Initialize();
        MainCharacterId = LocalDataHelper.GetMainCharacterId();
    }

    public void ResetCharacter() {
        MainCharacterId = EConfig.Character.INITIAL_CHARACTER_ID;
        LocalDataHelper.SaveMainCharacterId(MainCharacterId);
    }

    public void SetValidCharacters(List<string> characterIds) {
        validCharacterIds = characterIds
            .Where(id => characterDatabase.GetCharacterDataById(id) != null)
            .Distinct()
            .ToList();
    }

    public void SelectCharacter(string characterId) {
        if (!IsValid(characterId)) return;
        MainCharacterId = characterId;
        LocalDataHelper.SaveMainCharacterId(characterId);
    }

    public bool IsValid(string characterId) {
        return validCharacterIds.Contains(characterId);
    }

    public void Add(string characterId) {
        if (!validCharacterIds.Contains(characterId)) {
            validCharacterIds.Add(characterId);
            SaveInventory();
        }
    }

    public void Remove(string characterId) {
        if (validCharacterIds.Contains(characterId)) {
            validCharacterIds.Remove(characterId);
            SaveInventory();
        }
    }

    public void Clear() {
        validCharacterIds.Clear();
        SaveInventory();
    }

    private void SaveInventory() {
        UserManager.Instance.UpdateUserData();
    }

    public CharacterData GetSelectedCharacterData() {
        return characterDatabase.GetCharacterDataById(MainCharacterId);
    }

    public List<CharacterData> GetAllOwnedCharacters() {
        return validCharacterIds
            .Select(id => characterDatabase.GetCharacterDataById(id))
            .Where(data => data != null)
            .ToList();
    }

    public List<CharacterData> GetAllCharacters()
    {
        return characterDatabase.GetAll();
    }
}
