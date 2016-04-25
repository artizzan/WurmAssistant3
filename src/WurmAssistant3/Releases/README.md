Subdirectories of this folder are used to:

* control most significant version numbers (Major, Minor) for the publishing process.
* contain pages describing major changes in the new versions.

Publish robot parses subdirectory names into Version and takes the highest for the current version.dat.
It is important for build server to correctly deploy packages.

Each subdirectory must contain at least one file, set as content and copy to output directory. Else directories will not appear in build output.

Respecting standard html convention, the main html file of every subdirectory should be named *index.html*

This folder can be ignored for local builds.