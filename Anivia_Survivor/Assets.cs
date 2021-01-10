using R2API;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

public static class Assets
{
    public static AssetBundle MainAssetBundle = null;
    public static AssetBundleResourcesProvider Provider;

    public static Texture charPortrait;
    public static GameObject myCharacter;

    public static Sprite icon1;
    public static Sprite icon2;
    public static Sprite icon3;
    public static Sprite icon4;
    public static GameObject primarySkill;
    public static GameObject specialSkill;

    public static void PopulateAssets()
    {
        if (MainAssetBundle == null)
        {
            using (Stream assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Anivia_Survivor.examplesurvivorbundle"))
            {
                MainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                Provider = new AssetBundleResourcesProvider("@First_skill", MainAssetBundle);
                R2API.ResourcesAPI.AddProvider(Provider);
            }
        }
        //myCharacter = MainAssetBundle.LoadAsset<GameObject>("Anivia Variant");
        myCharacter = MainAssetBundle.LoadAsset<GameObject>("mdlExampleSurvivor");
        charPortrait = MainAssetBundle.LoadAsset<Sprite>("cryophoenix").texture;
        primarySkill = MainAssetBundle.LoadAsset<GameObject>("Capsule");
        primarySkill.transform.localScale = new Vector3(1f, 1f, 1f);
        specialSkill = MainAssetBundle.LoadAsset<GameObject>("iceStorm");
        specialSkill.transform.localScale = new Vector3(2f, 2f, 2f);
        //Rigidbody rb = primarySkill.GetComponent<Rigidbody>();
        //rb.angularVelocity = new Vector3(1f, 0f, 0f);

        icon1 = MainAssetBundle.LoadAsset<Sprite>("anivia_e");
        icon2 = MainAssetBundle.LoadAsset<Sprite>("anivia_w");
        icon3 = MainAssetBundle.LoadAsset<Sprite>("anivia_q");
        icon4 = MainAssetBundle.LoadAsset<Sprite>("anivia_r");
    }
}
