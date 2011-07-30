@ECHO OFF

IF NOT EXIST "%~1tools\nunit.cmd" GOTO :FileNotFound

ECHO Running unit tests...
"%~1tools\nunit.cmd" %1 %2
GOTO :End

:FileNotFound
ECHO Skipping unit testing, because nunit.cmd is not found.

:End
