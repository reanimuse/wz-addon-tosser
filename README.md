# wzAddonTosser
A lightweight addon tosser for World of Warcraft Addons.

## What it does
This is meant to be a fast and small way to copy and unpack addons from your download folder and automatically expand them 
into your WoW Addons folder.

## What it does NOT do
* It does NOT automatically keep your addons up to date.  If you want that then you should probably look for something like the full (and monolithic) Curse client
* It does not currently keep itself up to-date.  I plan on adding this later on but it isnt there yet.  If you want the latest and greatest rev, you must install it yourself

## How it works
> Initial Setup
* install the application
* edit the .Config File and set SourceDir to the folder it should look for downloaded addons in
* edit the .Config File and set WoWDir to the folder where Warcraft is installed 
* create a shortcut to the wzAddonTosser.exe file on your desktop

> Once Configured
* download whatever addons you want installed to WoW and place them in the folder you listed in the SourceDir
* Double-click on the shortcut to copy the files over

at this point, installing any addon to wow is just the two steps above.  Download the zips from whatever site you use for addons, and the click the shortcut

## Developers Note
I have attempted to put all addon-specific logic in the wzAddonTosser.Core.dll which can be referenced from any .Net program.  All core objects, such as the logic for managing WoW folders as well as for examining addons are implemented as classes in that library.

### Future Ideas (not implemented yet)
* full powershell integration
* class documentation - this is weak even in the source code and needs to be addressed
* support for multiple installations of WoW on the same machine.  This can be done now by installing multiple copies of the program with different config files but that is really clumsy
* switch to INI file as they are much simpler for end users to edit than .config files
* Purge old log files
* purge old backup folders 

#### Release notes
12/25/2018 - Updated for WoW 8.1 folder paths
