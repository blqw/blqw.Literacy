del %CD%\unpkgs\*.nupkg
dotnet pack -c release -o %CD%\unpkgs
for /r . %%a in (unpkgs\*.nupkg) do (
 	dotnet nuget push "%%a" -s https://api.nuget.org/v3/index.json -k %1%
)
del %CD%\unpkgs\*.nupkg
