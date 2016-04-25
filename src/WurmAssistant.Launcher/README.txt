Running AldursLab.WurmAssistantLauncher.exe will show a window with choices as to what and how to run.

Easy one-clicks, that skip this window:

Start Wurm Assistant using one of .bat files:
stable-windows.bat - to run latest stable windows version of WA for Wurm Online
stable-windows-unlimited.bat - to run latest stable windows version of WA for Wurm Unlimited

Advanced one-clicks:

AldursLab.WurmAssistantLauncher.exe [args]

where [args] can be: 

-BuildCode [code]
Use specific release channel, eg: beta, stable-win, stable-mac, stable-lin

-WurmUnlimited
Run for Wurm Unlimited, if omitted, runs for Wurm Online.

-BuildNumber [number]
Use specific build number for selected release channel, if omitted use latest build.

-RelativeDataDir
Instructs WA to keep all data inside launcher directory, instead of Local Application Data directory.
With this option, WA is fully portable. Data will not automatically move between these directories (but can be copied manually).
This param is supported since: dev R61, beta R26 and all stable.