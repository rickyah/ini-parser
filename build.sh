#!/bin/sh

echo "Running build script for BUILD_TYPE: $BUILD_TYPE"
echo

if [ "$BUILD_TYPE" = "mono" ]; then
    set -x
    msbuild src/IniParser.sln /nologo /p:Configuration=Release /Restore:True
    mono ./testrunner/NUnit.ConsoleRunner.3.10.0/tools/nunit3-console.exe ./src/IniParser.Tests/bin/Release/net461/IniParser.Tests.dll
elif [ "$BUILD_TYPE" = "dotnetcore" ]; then
    dotnet build -c Release $TRAVIS_SOLUTION
    echo
    dotnet test src/IniParser.sln
    echo
else
    echo "ERROR: Unknown build type value: $BUILD_TYPE"
    exit -1
fi

