using System.Reflection;
using UnityEngine;
using Zenject;

namespace SaberFactory.Helpers;

public static class ZenjectHelpers
{
    public static void BindFromAssetBundle<T>(this DiContainer container, string path)
    {
        container.BindInterfacesAndSelfTo<T>().FromComponentOn(ctx =>
        {
            var ns = Assembly.GetExecutingAssembly().GetName().Name;
            var bundleData = Readers.ReadResource(ns + "." + path);
            var bundle = AssetBundle.LoadFromMemory(bundleData);
            var asset = Object.Instantiate(bundle.LoadAsset<GameObject>("prefab"));
            ctx.Container.InjectGameObject(asset);
            asset.SetActive(true);
            bundle.Unload(false);
            return asset;
        }).AsSingle();
    }
}