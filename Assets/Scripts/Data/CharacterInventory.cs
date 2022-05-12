using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInventory
{
    private static CharacterInventory instance = null;
    public static CharacterInventory Instance { get { if (instance == null) { instance = new CharacterInventory(); } return instance; } }
    public ECharacterType MainCharacter { get; private set; }

    private BitFlags VaildCharacter;

    public void Initialize()
    {
        int initailFlags = LocalDataHelper.GetCharacterInven();
        if (initailFlags < 1) 
            initailFlags = (int)EConfig.Character.INITIAL_CHARACTER;
        
        VaildCharacter = new BitFlags(initailFlags);

        MainCharacter = (ECharacterType)LocalDataHelper.GetMainCharacter();
        if (MainCharacter == ECharacterType.None) 
            MainCharacter = EConfig.Character.INITIAL_CHARACTER;
    }

    public void SelectCharacter(ECharacterType type)
    {
        if (!IsVaild(type)) return;
        MainCharacter = type;
        LocalDataHelper.SaveMainCharacter((int)type);
    }

    public bool IsVaild(ECharacterType type)
    {
        return VaildCharacter.Has((byte)type);
    }

    public void Add(ECharacterType type)
    {
        VaildCharacter.Add((byte)type);
        SaveInven();
    }

    public void Remove(ECharacterType type)
    {
        VaildCharacter.Remove((byte)type);
        SaveInven();
    }

    public void Clear()
    {
        VaildCharacter.Clear();
        SaveInven();
    }

    private void SaveInven()
    {
        int flags = VaildCharacter.Value;
        LocalDataHelper.SaveCharacterInven(flags);
    }
}
