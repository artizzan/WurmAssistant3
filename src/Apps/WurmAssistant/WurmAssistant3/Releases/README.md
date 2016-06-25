This directory is used by publish robot on the official build server. It can be ignored otherwise.

Subdirectories of this directory are used to:

* control most significant version numbers (Major, Minor) for the publishing process.
* contain pages describing major changes in the new versions.

Publish robot parses subdirectory names into Version and chooses the highest for the packaged version.dat file.
This file is important for build server, in order to correctly deploy packages.

Each subdirectory must contain at least one file, set as content and copy to output directory. Else directories will not appear in build output.

Respecting standard html convention, the main html file of every subdirectory should be named *index.html*