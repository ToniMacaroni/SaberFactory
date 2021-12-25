*(for Beat Saber 1.19)*

- [Alpha] [Patreon] Added new modifier tab (edit visibility of parts and position/scale of transforms on the saber)
- Added option to automatically reload the saber when the file changes (`ReloadOnSaberUpdate` in config)
- Added option to modify saber length (added by @9cxndy)
- Moved .meta and .trail files to seperate folder (`UserData\Saber Factory\Cache`) to not fill up the CustomSabers folder
- Added `_UserColorLeft` and `_UserColorRight` global shader variables that will be assigned to left saber/right saber color
- Fixed trail reverting in some instances
- Fixed particle systems sometimes hiding behind ui
- Many internal code changes

__**!!Presets aren't compatible with previous version!!**__  
__**!!Make sure to delete the `Beat Saber\UserData\Saber Factory\Presets\default` file!!**__

*You can safely delete the .trail and .meta files in your CustomSabers folder*

*Needs `BeatSaberMarkupLanguage` and `SiraUtil` (available on ModAssistant)*