# TODO make this multi-purpose - use parameters
# TODO build first

$ProjectDir = "."
$PackagesDir = "$ProjectDir\packages"
$OutDir = "$ProjectDir\bin\Debug"

# Set nunit path test runner
$xunit = "$ProjectDir\packages\xunit.runner.console.2.4.1\tools\net472\xunit.console.exe"

# Run tests
& $xunit ChinhDo.Transactions.FileManagerTest\bin\Debug\ChinhDo.Transactions.FileManagerTest.dll