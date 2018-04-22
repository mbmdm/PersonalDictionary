echo on

del /s %cd%\Debug\*.exe
del /s %cd%\Debug\*.exe.config
del /s %cd%\Debug\*.pdb
del /s %cd%\Debug\*.dll

rmdir /s /q %cd%\.vs
rmdir /s /q %cd%\DictionaryApplet\obj
rmdir /s /q %cd%\PersonalDictionary\obj
rmdir /s /q %cd%\Tests\PersonalDictionary.Test\bin
rmdir /s /q %cd%\Tests\PersonalDictionary.Test\obj