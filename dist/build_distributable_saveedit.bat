@echo off
call "%VS100COMNTOOLS%vsvars32.bat"
svn revert "clone\Gibbed.BorderlandsOz.SaveEdit\Version.cs"
svn update "clone"
subwcrev "clone" "clone\Gibbed.BorderlandsOz.SaveEdit\Version.cs.template" "clone\Gibbed.BorderlandsOz.SaveEdit\Version.cs"
rmdir /s /q "saveedit"
mkdir "saveedit"
msbuild "clone\The Pre-Sequel!.sln" /p:Configuration="SaveEdit Packaging" /p:OutputPath="%cd%\saveedit"
copy /y "Gibbed.BorderlandsOz.SaveEdit.exe.config" "saveedit\Gibbed.BorderlandsOz.SaveEdit.exe.config"
copy /y "clone\license.txt" "saveedit\license.txt"
copy /y "clone\readme.txt" "saveedit\readme.txt"
svn log "clone" > "saveedit\revisions.txt"
cd "saveedit"
rem mkdir pdbs
rem move *.pdb pdbs\
del *.pdb
mkdir assemblies
move *.dll assemblies\
del *.xml
del ..\saveedit.zip
7z a -r -tzip -mx=9 ..\saveedit.zip .
cd ".."
pause