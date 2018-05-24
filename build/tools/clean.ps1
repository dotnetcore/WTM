$ErrorActionPreference = 'Stop'
# Environment helpers ------------------------------------
Function Get-MsBuildPath() {
    $msBuildPath = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\msbuild.exe"
    return $msBuildPath
}
# Environment variables ----------------------------------
# $global_buildDirPath = Get-Location
$global_buildDirPath = ".\"
$global_msBuildPath = Get-MsBuildPath
$global_solutionPath = "$global_buildDirPath"
$global_solutionFilePath = "$global_solutionPath\WalkingTec.Mvvm Core.sln"
$global_nugetPath = "$global_buildDirPath\build\tools\.nuget\nuget.exe"

Function Compile-Project() {
    iex -Command "& '$global_msBuildPath' '$global_solutionFilePath' /t:Clean /p:Configuration=Release"
}

Compile-Project