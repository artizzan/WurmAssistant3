[T4Scaffolding.Scaffolder(Description = "Code generator to extract wrapping class and contract for existing class.")][CmdletBinding()]
param(   
	[parameter(Mandatory=$true, ValueFromPipelinebyPropertyName = $true)][string]$classToWrap,    
	[string]$staticOnly,
	[string]$Project,
	[string]$CodeLanguage,
	[string[]]$TemplateFolders,
	[switch]$Force = $false
)

$className  = $classToWrap

if ($className.Contains(".")) {
	$className = $className.Substring($className.LastIndexOf(".") + 1)
}

$className = $className + "2"

$namespace = (Get-Project $Project).Properties.Item("DefaultNamespace").Value

# Generate Wrapper Interface 
$outputPath = "Wrapper\Contract\" + "I$className"
Add-ProjectItemViaTemplate $outputPath -Template WrapperInterface `
	-Model @{ Namespace = $namespace
		StaticOnly = $staticOnly; `
		ClassFullName = $classToWrap; `
		ClassName = $className; `
		Project = $dte.ActiveSolutionProjects `
	} `
	-SuccessMessage "Added wrapper interface output at {0}" `
	-TemplateFolders $TemplateFolders `
	-Project $Project `
	-CodeLanguage $CodeLanguage `
	-Force:$Force 

# Generate Wrapper Concrete class 
$outputPath = "Wrapper\" + "$className"
Add-ProjectItemViaTemplate $outputPath -Template Wrapper `
	-Model @{ Namespace = $namespace
		StaticOnly = $staticOnly; `
		ClassFullName = $classToWrap; `
		ClassName = $className; `
		Project = $dte.ActiveSolutionProjects `
	} `
	-SuccessMessage "Added wrapper class output at {0}" `
	-TemplateFolders $TemplateFolders `
	-Project $Project `
	-CodeLanguage $CodeLanguage `
	-Force:$Force 


Write-Host "Generation Complete!!!" -foregroundcolor green