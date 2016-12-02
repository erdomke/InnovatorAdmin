<#  
.SYNOPSIS
  This is a helper function that runs a scriptblock and checks the PS variable $lastexitcode
  to see if an error occcured. If an error is detected then an exception is thrown.
  This function allows you to run command-line programs without having to
  explicitly check the $lastexitcode variable.
.EXAMPLE
  exec { svn info $repository_trunk } "Error executing SVN. Please verify SVN command-line client is installed"
#>

$version = [System.DateTime]::Now.ToString("yyyy.MM.dd.hhmm")

$assyInfo = "using System.Reflection;`r`n`r`n[assembly: AssemblyVersion(""$version"")]`r`n[assembly: AssemblyFileVersion(""$version"")]"

$assyInfo | Out-File Innovator.Client\AssemblyInfo.Version.cs

(Get-Content Innovator.Client/project.json) `
    -replace '"version": "\d{4}\.\d{2}\.\d{2}\.\d{4}",', """version"": ""$version""," |
  Out-File Innovator.Client/project.json

function Exec  
{
    [CmdletBinding()]
    param(
        [Parameter(Position=0,Mandatory=1)][scriptblock]$cmd,
        [Parameter(Position=1,Mandatory=0)][string]$errorMessage = ($msgs.error_bad_command -f $cmd)
    )
    & $cmd
    if ($lastexitcode -ne 0) {
        throw ("Exec: " + $errorMessage)
    }
}

exec { & dotnet restore }
exec { & dotnet pack Innovator.Client/project.json -c Release -o .\artifacts --version-suffix=$revision }  