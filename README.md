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


### GameModeOption json

```
{
	"$type": "GameModeLoader.Data.LevelOptionCatalog, GameModeLoader",
	"id": "exampleoption", //This is the ID for the LevelOptionCatalog. It should be Unique
	"version": 3,
	"excludeLevelIds": ["Canyon"], //You can exclude maps from showing this option
	"excludeGameModeNames":  ["Survival"], //You can exclude gamemodes from showing this option
	"levelOption": {          
		"$type": "GameModeLoader.Data.LevelOption, GameModeLoader",
		"name": "exampleoption", //This is the name of the option that will be added to level.options.
		"displayName": "Example", //Display name of the option on the map board
		"description": "An Example Option", //Description shown on the map board
		"type": "Toggle", // Boolean toggle on/off. Stars not supported/Tested yet
		"levelModuleOptional": // The levelModuleOption that should be loaded when this Option is enabled,  you can populate variables here like a normal levelModule
			{ "$type": "GameModeLoader.Component.ExampleOption, GameModeLoader" } 
	}
}
```