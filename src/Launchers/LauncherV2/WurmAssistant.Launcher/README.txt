# TLDR:

Run AldursLab.WurmAssistantLauncher.exe


# Long story:

Running AldursLab.WurmAssistantLauncher.exe will show GUI with choices of available WA versions.

Alternatively, .bat files can be used to start specific WA version, skipping GUI.

Example bat file descriptions:
stable-windows.bat - runs latest stable windows version of WA for Wurm Online
stable-windows-unlimited.bat - runs latest stable windows version of WA for Wurm Unlimited

A custom .bat file can be created, or launcher can be started from command line.
Below is the description of available command line arguments.

CMD Syntax:

AldursLab.WurmAssistantLauncher.exe [args]

where [args] can be: 

-BuildCode [code]
Use specific release channel, eg: beta, stable-win, stable-mac, stable-lin

-WurmUnlimited
Runs WA in Wurm Unlimited mode. If omitted, runs in Wurm Online mode.

-BuildNumber [number]
Use specific build number for selected release channel. If omitted uses latest build.

-RelativeDataDir
Instructs WA to keep all data inside launcher directory, instead of Local Application Data directory.
With this option, WA launcher is fully portable. 
Note that WA is not going to copy settings or data between Local Application Data and launcher dir. It can be copied manually.
This param is supported since: dev R61, beta R26 and all stable.