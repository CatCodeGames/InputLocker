using CatCode.InteractionLocking;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Pool;

public static class InteractionDomainBitUtility
{
    private const int BitsCount = 32;
    private readonly static bool[] _usedBits = new bool[BitsCount];

    public static int AssignUniqueBit(InteractionLayerAsset asset)
    {
        int bit = GetUniqueBit(asset);
        if (bit == -1)
            return -1;

        var so = new SerializedObject(asset);
        so.FindProperty("_bitIndex").intValue = bit;
        so.ApplyModifiedProperties();

        EditorUtility.SetDirty(asset);
        AssetDatabase.SaveAssets();

        return bit;
    }

    public static int GetUniqueBit(InteractionLayerAsset asset)
    {
        using var listHandle = ListPool<InteractionLayerAsset>.Get(out var assets);

        LoadAllAssets(assets);
        CollectUsedBits(assets, asset, _usedBits);

        if (IsUnique(asset.BitIndex, _usedBits))
            return asset.BitIndex;
        else
            return FindFreeBit(_usedBits);
    }

    private static void LoadAllAssets(List<InteractionLayerAsset> assets)
    {
        assets.Clear();

        var guids = AssetDatabase.FindAssets($"t:{nameof(InteractionLayerAsset)}");

        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadAssetAtPath<InteractionLayerAsset>(path);
            if (asset != null)
                assets.Add(asset);
        }
    }

    private static void CollectUsedBits(List<InteractionLayerAsset> assets, InteractionLayerAsset exclude, bool[] usedBits)
    {
        for (int i = 0; i < usedBits.Length; i++)
            usedBits[i] = false;

        for (int i = 0; i < assets.Count; i++)
        {
            var asset = assets[i];
            if (asset == exclude)
                continue;

            int idx = asset.BitIndex;
            if (idx >= 0 && idx < BitsCount)
                usedBits[idx] = true;
        }
    }

    private static bool IsUnique(int index, bool[] usedBits)
    {
        return index >= 0 && index < BitsCount && usedBits[index] == false;
    }

    private static int FindFreeBit(bool[] usedBits)
    {
        for (int i = 0; i < _usedBits.Length; i++)
            if (!usedBits[i])
                return i;

        return -1;
    }
}