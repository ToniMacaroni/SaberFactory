*(for Beat Saber 1.30)*

- Update for Beat Saber 1.30
- Support for BSIPA 4.3.0
- Allow sabers to only show in HMD (thanks to @ModdingPink)
- Migrated to AssetBundleLoadingTools (for safe bundle loading and SPI shader replacement)
- Fixed error when going into settings without any saber selected

**!!Important!!**  
Due to the Beat Saber update (switch to new Unity version) all previous custom sabers (materials) are incompatible.  
This SF update will try to automatically replace all materials with valid ones, but this will only work for public shaders.  
If you are the creator of previous sabers, you will need to re-export them with SPI compatibility.


*this is just a small update for bug fixes in 2.x.x, v3 is still being developed in parallel*  
*Needs `BeatSaberMarkupLanguage`, `SiraUtil`, `CameraUtils` and `AssetBundleLoadingTools` (available on ModAssistant)*
