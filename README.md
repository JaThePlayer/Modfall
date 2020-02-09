# Modfall
A mod loader for the game TowerFall Ascension, powered by Monomod (https://github.com/MonoMod/MonoMod)

# Features
- Loading code mods at run-time
- Events for every single function thanks to monomod
- Adding new graphics
- Adding new arrow types
- Adding new pickups
- Adding new Co-Op maps
- Converting .tower maps to .oel for Co-Op maps

# Installation

Installer:

Download the newest release of Modfall.CmdInstaller (https://github.com/JaThePlayer/Modfall.CmdInstaller/releases), run the .exe and follow the instructions that appear.

Manual:

Drop the files from the MonoMod directory and the TowerFall.ModLoader.mm.dll into your Towerfall directory, then in cmd/terminal/whatever type in these two commands:

MonoMod.exe TowerFall.exe

MonoMod.RuntimeDetour.HookGen.exe TowerFall.exe

A MONOMODDED_TowerFall.exe should show up in your towerfall directory. Run it so it can create some files and directories.
An automated installer will be made for this later.

# Installing mods
To install a mod, simply drop the mod's folder into the Towerfall/Mods/ directory.

# Example mods
https://github.com/JaThePlayer/MoreCoOp - Adds more co-op maps to the game, based on the example workshop maps made by devs

Please note that this mod loader is currently in a very early stage of development, so bugs might show up. Please report them so that they can be fixed.
