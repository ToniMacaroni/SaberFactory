using System.Reflection;
using UnityEngine;

namespace FlowUi;

public class FlowUI
{
    public static bool IsInitialized { get; private set; }
    
    public static void Init()
    {
        if (IsInitialized)
        {
            return;
        }

        IsInitialized = true;
        
        LoadLib();
    }

    private static void LoadLib()
    {
        try
        {
            var data = BeatSaberMarkupLanguage.Utilities.GetResource(Assembly.GetExecutingAssembly(), "FlowUi.Resources.serializer");
            Assembly.Load(data);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}