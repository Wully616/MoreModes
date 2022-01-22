# GameModeLoader

A generic gamemode creator and loader for Blade and Sorcery

LevelModules can be created which are conditionally loaded on maps as "options" which can be enabled or disabled
To define LevelModules as map options, create a "GameModeOption" json file

GameModes are defined as a collection of LevelModules from both the base game and this mod (and other mods which extend this one)
To define Gamemodes, create a Mode json. The json defines what modules should be included. 
There are various options to exclude particular maps getting the mode and to prevent all GameModeOptions being added to this custom gamemode. 
But it can still allow you to include particular GameModeOptions you want enabled on your gamemode. To give the gamemode some customisability.


# Setup
I would recommened checking this out into your unity BAS SDK project's `\BuildStaging\Plugins` folder. The built mod will be in `\BuildStaging\Catalog`