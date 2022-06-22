using System.Collections;
using System.Collections.Generic;
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
    public ECharacterType MainCharacter { get; private set; }

    private List<int> VaildCharacters;

    public void Initialize() {
        MainCharacter = (ECharacterType) LocalDataHelper.GetMainCharacter();
    }

    public void ResetCharacter() {
        LocalDataHelper.SaveMainCharacter((int) EConfig.Character.INITIAL_CHARACTER);
    }

    public void SetValidCharacters(List<int> characters) {
        VaildCharacters = characters;
    }

    public void SelectCharacter(ECharacterType type) {
        if (!IsValid(type)) return;
        MainCharacter = type;
        LocalDataHelper.SaveMainCharacter((int) type);
    }

    public bool IsValid(ECharacterType type) {
        return VaildCharacters.Contains((int) type);
    }

    public void Add(ECharacterType type) {
        VaildCharacters.Add((byte) type);
        SaveInven();
    }

    public void Remove(ECharacterType type) {
        VaildCharacters.Remove((byte) type);
        SaveInven();
    }

    public void Clear() {
        VaildCharacters.Clear();
        SaveInven();
    }

    private void SaveInven() {
        DataManager.Instance.UpdateUserData();
    }
}
