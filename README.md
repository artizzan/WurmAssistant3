Wurm Assistant 3
====================

Wurm Assistant 3 is an extension application to [Wurm Online][] sandbox MMORPG. It provides various tools, that enrich the game experience.


For more information about Wurm Assistant, please visit:
[Wurm Assistant Official Discussion][official thread]

Quick start
--------------

1. (Recommended) Fork this repository.
2. (Recommended) Use SourceTree from Atlassian to clone your fork or this repository. This will handle submodules for you.
2. Might need to open and rebuild WurmApi.sln and AldursLab.sln
3. Build and run WurmAssistant3.sln

Compiles under VS 2013 Community and higher.

Branches, Pull requests
--------------

dev -> beta -> stable-x

Any pulls should be directed at Dev, unless hotfixing issues in lower branches.

About the source
--------------
Project is composed of following parts:

Solutions:

1. WurmAssistant3 - The core standalone, WinForms .NET 4.5 application. It has been built with platform independency in mind and relies only on libraries, that claim to support Windows, Mac and Linux.
2. WurmAssistantWebService - Simple MVC application serving as a web backend to WurmAssistant3. Currently only serves new releases for the launcher. 
3. WurmAssistant.Launcher - Simple WinForms application, which allows downloading latest WA3 version among all of it's flavors.
4. WurmAssistant.PublishRobot - Simple app used by CI server to automatically push new successful builds to the web service.
5. WurmAssistantLite - Obsolete.

Submodules:

1. WurmApi - API library, that abstracts rather complex process of reading and parsing game client log output into manageable events and queries categorized by game characters and server groups.
2. AldursLab - Common utility library.
3. WurmAssistantDataTransfer - data structures shared between WA3 and WA2 (different repository), required for data import from WA2 to WA3.

Major external libraries used:

1. Irrklang sound engine.
2. Ninject dependency injection framework.
3. NUnit, JustMoq (unit testing).
4. ObjectListView.
5. Json.NET.
6. NLog.
6. EntityFramework (web service only)
7. FastZipSharp (publish robot only)

License details
-------------

This project is licensed under GPL v3 license. 

Original author of this repository, reserves the right to relicense this repository, or any of it's parts, under less restrictive license.

Some or all 3rd party components and/or libraries, used within this project, may already be licensed under different, less or more restrictive terms, by their original authors. This project license **does not overwrite** these licenses in any way.

Any and all licensing queries and questions should be sent to aldurcraft@gmail.com

[Wurm Online]:http://www.wurmonline.com/
[official thread]:http://forum.wurmonline.com/index.php?/topic/68031-wurm-assistant-enrich-your-wurm-experience/