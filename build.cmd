if EXIST TweetzSetup.exe del TweetzSetup.exe
pushd tweetz
msbuild tweetz5.sln "/p:configuration=Release;platform=Any CPU" /t:rebuild
if ERRORLEVEL 1 GOTO END
cd Setup
"%ProgramFiles(x86)%\inno setup 5\iscc" Setup.iss
:END
popd