resgen ThetisCoreRes.resx
al /t:lib /embed:ThetisCoreRes.resources /culture:en /out:ThetisCore.resources.dll
move ThetisCore.resources.dll bin/Debug/en
al /t:lib /embed:ThetisCoreRes.resources /culture:ja /out:ThetisCore.resources.dll
move ThetisCore.resources.dll bin/Debug/ja
al /t:lib /embed:ThetisCoreRes.resources /culture:de /out:ThetisCore.resources.dll
move ThetisCore.resources.dll bin/Debug/de
