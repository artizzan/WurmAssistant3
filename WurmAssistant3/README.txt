AldursLab.WurmAssistant3.exe can be run with additional command line params:
-WurmUnlimited : starts WA in Wurm Unlimited mode, if omitted, starts in Wurm Online mode.
-RelativeDataDir : will use dir relative to: this .exe if running directly or launcher if running from within launcher

Note about RelativeDataDir:
Even if you run AldursLab.WurmAssistant3.exe directly, as long as it's directory is under launcher directory, 
it will use a launcher-specific data dir.
If you want to trully have a specific data dir for this particular wurm assistant, copy it out of the launcher dir. 
It will then be saved to an immediate subdir.