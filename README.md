<p align="center">
    <h1 align="center">Saber Factory 2</h1>
</p>

<p align="center">
    <a href="https://github.com/ToniMacaroni/SaberFactory/actions/workflows/integrate.yml" alt="build status">
        <img src="https://img.shields.io/github/workflow/status/ToniMacaroni/SaberFactory/Build%20Saber%20Factory" /></a>
    <a href="#backers" alt="total downloads">
        <img src="https://img.shields.io/github/downloads/ToniMacaroni/SaberFactory/total" /></a>
    <a href="https://github.com/ToniMacaroni/SaberFactory/releases" alt="latest version">
        <img src="https://img.shields.io/github/v/tag/ToniMacaroni/SaberFactory?label=version" /></a>
</p>

<p align="center">
    <h4 align="center">A highly customizable saber mod for Beat Saber</h4>
</p>

</br>
</br>

|   |  |
| ------------- | ------------- |
| Mod Download  | **[Latest Version](https://github.com/ToniMacaroni/SaberFactory/releases)**  |
| Website  | **[SaberFactory.com](https://saberfactory.com)**  |

To get more content and help with the mod or creation of content  
join the the **[Saber Factory Discord](https://discord.gg/PjD7WcChH3)** server.

Or if you want to help the project grow:

| [:heart: Donate](https://ko-fi.com/tonimacaroni)  |
| ------------- |

## What is Saber Factory?
Simply said: An all-rounder when it comes to sabers.

Combine different saber parts like lego pieces.  
Everything is built around customization.  
Change the shape, shaders, material properties, textures and more of parts and sabers.

**You can use and customize both parts and custom sabers in saber factory**

## How do I install it
1) Download the first zip from [Here](https://github.com/ToniMacaroni/SaberFactoryV2/releases)
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