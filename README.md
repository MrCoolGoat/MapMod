# MapMod
MapMod is a custom map loader plugin for CarX Drift Racing Online on PC built using the BepInEx framework.

Supported surface types:
* Asphalt
* Grass
* Kerbs
* Sand
* Snow
* Gravel
* Icy Road
* Dirt

Supported object types:
* Rigidbody (movable)
* No collision

The plugin also supports custom spawnpoints, camera points for cinematic camera mode and custom lights with tweakable color, intensity and angle where appropriate.

Available light types are:
* Sun (only 1 active at a time)
* Spot light
* Point light

Most material settings are supported:
* Albedo (Color) - RGB color or texture
* Metallic
* Specular - Value or texturemap
* Smoothness
* Translucency
* Emission - RGB color or texturemap
* IOR - Value 1-2.5
* Bump map with scale

#### Notice
Public servers are disabled when this mod is enabled, only people with the mod will be able to join lobbies created when this plugin is enabled.
To re-enable public servers move the MapMod.dll out of the plugins folder.

### Keybinds
* F6 to open/close the menu
* F5 to respawn
* F3 to switch respawn
* F10 to teleport to the camera

## Installation
Installation is described in [HOW-TO-INSTALL.md](HOW-TO-INSTALL.md)
