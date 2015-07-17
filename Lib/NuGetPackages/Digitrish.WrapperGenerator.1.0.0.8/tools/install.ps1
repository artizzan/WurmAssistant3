param($rootPath, $toolsPath, $package, $project)

# Bail out if scaffolding is disabled (probably because you're running an incompatible version of T4Scaffolding.dll)
if (-not (Get-Module T4Scaffolding)) { return }

if ($project) { $projectName = $project.Name }
# Get-ProjectItem "InstallationDummyFile.txt" -Project $projectName | %{ $_.Delete() }