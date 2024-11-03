# StardustGD
 This is a Godot port of the [Stardust Board Game](https://www.thegamecrafter.com/games/stardust2) which is owned by [CaptainHoers](https://captainhoers.tumblr.com/).
 Developed with [Godot Mono](https://godotengine.org/) [v4.3](https://godotengine.org/download/archive/4.3-stable/)
 
 The Images found in Textures/CaptainHoers directory and Models found in Meshes/Pawns belong to [CaptainHoers](https://captainhoers.tumblr.com/). Some of them have been modified.

 There used to be a Unity version, but I'm rewriting it in Godot and making the code available for all to see.
 Why? Multiple reasons. Some more obvious, some less. Mostly because nobody stopped me.

 Code is in the Scripts directory.
 Scripts/Core - Main Game Logic lives here. Pawns, Actions, Rooms, etc. Code in this folder should not know about Godot.
 Scripts/2D - Godot Rendering for 2D. How it handles showing rooms as sprites, pawns, interactions between the Core code and the Player.
 Scripts/3D - ^^ same, but for 3D. Largely unfinished.
 Scripts/UI - Code for both 2D and 3D UI shared in here, and the main menu. Usually scripts inherit the Control class.

 This project is under the [CC BY-NC-SA](https://creativecommons.org/licenses/by-nc-sa/3.0/) License.
 The planetn font in the Fonts directory has it's own License.