This tool is obsolete due to we provide a new visualization editor (./../editor/KissEditor.exe).
If you persist in using this, below are 2 ways to use RIDL.
1: Click '.\OldTools\KissGennerateRIDL.exe' manually after you add or modified any '*.RIDL' files.
2: Config your project setting for automatically after you add or modified any '*.RIDL' files.
	e. g. modify 'KissServerFramework.csproj' like this:
<Project Sdk="Microsoft.NET.Sdk">
  <Target Name="PreBuild" BeforeTargets="PreBuildEvent">
    <Exec Command="dotnet $(ProjectDir)OldTools\KissGennerateRIDL.dll" />
  </Target>
</Project>