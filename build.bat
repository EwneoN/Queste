set MSBUILDDISABLENODEREUSE=1
echo Restoring NuGet Packages...
"c:\nuget\nuget.exe" restore "Queste.sln"
echo NuGet Packages restored
echo Release build...
"C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe" /consoleloggerparameters:ErrorsOnly /maxcpucount /nologo /property:Configuration=Release /verbosity:quiet /m:4 /nr:false "Queste.sln"
echo Build completed
echo Tests cleaned