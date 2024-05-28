# StardustGD
 This is a Godot port of the [Stardust Board Game](https://www.thegamecrafter.com/games/stardust2) owned by [CaptainHoers](https://captainhoers.tumblr.com/).
 Developed with [Godot Mono](https://godotengine.org/) [v4.2](https://godotengine.org/download/archive/4.2-stable/) (The .NET version)
 
 The Images found in the Textures/CaptainHoers directory belong to [CaptainHoers](https://captainhoers.tumblr.com/).
 The Models found in the Meshes/Pawns directory also belong to [CaptainHoers](https://captainhoers.tumblr.com/).

 There used to be a Unity version, but I'm rewriting it in Godot and making the code available for all to see.
 Why? Multiple reasons. Some more obvious, some less. Mostly because nobody stopped me.

 Code is in the Scripts directory.
 Scripts/Core - Main Game Logic lives here. Pawns, Actions, Rooms, etc. Code in this folder should not know about Godot.
 Scripts/2D - Godot Rendering for 2D. How it handles showing rooms as sprites, pawns, interactions between the Core code and the Player.
 Scripts/3D - ^^ same, but for 3D. Largely unfinished.
 Scripts/UI - Code for both 2D and 3D UI shared in here, and the main menu. Usually scripts inherit the Control class.

 If you've any questions, my discord is the same as my username.

 This project is under the [CC BY-NC-SA](https://creativecommons.org/licenses/by-nc-sa/3.0/) License.
 The planetn font in the Fonts directory has it's own License.