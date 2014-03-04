$u = "${Env:ProgramFiles(x86)}" + "\tweetz desktop\unins000.exe"
Uninstall-ChocolateyPackage "tweetz" "exe" "/verysilent" "$u"
