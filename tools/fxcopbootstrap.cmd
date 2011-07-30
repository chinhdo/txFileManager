@ECHO OFF

IF NOT EXIST %1tools\fxcop.cmd GOTO :FileNotFound

ECHO Analyzing code...
%1tools\fxcop.cmd %2
GOTO :End

:FileNotFound
ECHO Skipping code analysis, because fxcop.cmd is not found.

:End
