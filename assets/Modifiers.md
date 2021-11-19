Modifiers allow you to transform and enable/disable gameobjects in Saber Factory.

Start by downloading and installing [this package](https://raw.githubusercontent.com/ToniMacaroni/SaberFactory/main/assets/SaberFactoryEditor.unitypackage) into your unity project.  
After that add the `SaberModifierCollection` script to one of your sabers.  
Within the script you can either add visibility or transform modifiers.  
Within the specific modifiers you can add any amount of objects.  
Keep in mind if you add multiple objects to the transform modifier, you will scale each object individualy.  
To scale objects around the center make sure to attach them to a parent gameobject and add that gameobject to the modifier.  

**Make sure to click the `Save` button everytime you make changes to the modifier**.  
Changes to the gameobjects don't need the save button to be clicked again.

**In order for modifiers to copy from the left saber to the right make sure they have the same name**

[Showcase Video](https://raw.githubusercontent.com/ToniMacaroni/SaberFactory/main/assets/ModifiersVideo.mp4)  

*Disclaimer: Component modifiers aren't implemented yet but are coming soon*