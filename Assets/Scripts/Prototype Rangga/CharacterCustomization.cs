using Main.Gameplay;
using Main.Mainmenu;
using Spine;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCustomization : CharacterSystem
{
    public enum SkinSource
    {
        FromPlayerData,
        CustomManual
    }

    [Header("Settings")]
    public SkinSource skinSource;

    [Header("Manual Selection")]
    [SpineSkin(dataField: "skeletonAnimation")]
    public List<string> manualSkins = new List<string>();

    private void Start()
    {
        ApplySkinsFromData();
    }

    private void OnValidate()
    {
        if (skeletonAnimation == null) return;
        if (skeletonAnimation.skeletonDataAsset == null) return;

        if (!Application.isPlaying)
        {
            RefreshEditorSkin();
        }
    }

    public void ApplySkinsFromData()
    {
        string[] skinsToApply;

        if (skinSource == SkinSource.FromPlayerData && Application.isPlaying)
        {
            if (PlayerLocalData.inventoryData == null) return;
            skinsToApply = PlayerLocalData.inventoryData.SelectedSkins.ToArray();
        }
        else
        {
            skinsToApply = manualSkins.ToArray();
        }

        if (skinsToApply.Length > 0)
        {
            CombineSkins(skinsToApply);
        }
    }

    private void RefreshEditorSkin()
    {
        if (manualSkins != null && manualSkins.Count > 0)
        {
            CombineSkins(manualSkins.ToArray());
        }
    }

    public void CombineSkins(string[] skinNames)
    {
        if (skeletonAnimation.Skeleton == null)
        {
            skeletonAnimation.Initialize(false);
        }

        var skeleton = skeletonAnimation.Skeleton;
        var skeletonData = skeleton.Data;

        Skin combinedSkin = new Skin("Combined");

        foreach (string skinName in skinNames)
        {
            if (string.IsNullOrEmpty(skinName)) continue;

            Skin sourceSkin = skeletonData.FindSkin(skinName);
            if (sourceSkin != null)
            {
                combinedSkin.AddSkin(sourceSkin);
            }
        }

        skeleton.SetSkin(combinedSkin);
        skeleton.SetSlotsToSetupPose();

        skeletonAnimation.AnimationState.Apply(skeleton);
        skeletonAnimation.Update(0);
    }
}