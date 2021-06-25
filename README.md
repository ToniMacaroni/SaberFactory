# Saber Factory 2 *[SaberFactory.com](https://saberfactory.com)*.  

A highly customizable saber mod for Beat Saber

Mod Download => **[Releases](https://github.com/ToniMacaroni/SaberFactory/releases)**

To get more content and help with the mod or creation of content  
join the the **[Saber Factory Discord](https://discord.gg/PjD7WcChH3)** server

| [:heart: Donate](https://ko-fi.com/tonimacaroni)  |
| ------------- |

## What is Saber Factory?
Simply said: An all-rounder when it comes to sabers.

Combine different saber parts like lego pieces.  
Everything is built around customization.  
Change the shape, shaders, material properties, textures and more of parts and sabers.

**You can use and customize both parts and custom sabers in saber factory**

## How do I install it
1) Download the first zip from [Here](https://github.com/ToniMacaroni/SaberFactory/releases)
2) Unpack it in your Beat Saber directory

## I have made a map and want it to use a specific saber
You can add a "_customSaber" prop to your beatmap data
that tells Saber Factory to use a specific saber for this map like this:
```
"_customData": {
    "_customSaber": "Plasma Katana"
}
```
Make sure to use the actual name of the saber not the file name.
Best is to look in-game at the saber to see what the actual name is.

## I made a mod that needs to create some sabers in a song
If you want to create sabers in a song see https://github.com/Auros/SiraUtil#sabers

## I made a mod that needs to create some sabers in the menu (or other place after the menu)
If you want to create sabers in the menu (like [Custom Menu Pointers](https://github.com/dawnvt/CustomMenuPointers/) does)  
you can request the `MenuSaberProvider` and create sabers with it like this:

```csharp
public class MyMenuManager : IInitializable
{
    private readonly MenuSaberProvider _menuSaberProvider;

    public MyMenuManager(MenuSaberProvider menuSaberProvider)
    {
        _menuSaberProvider = menuSaberProvider;
    }

    public async void Initialize()
    {
        var myLeftSaber = await _menuSaberProvider.CreateSaber(parent:null, saberType:SaberType.SaberA, color:Color.red, createTrail:true);
    }
}
```