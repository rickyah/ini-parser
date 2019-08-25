#!/bin/sh

echo "Running install script for BUILD_TYPE: $BUILD_TYPE"
echo

if [ "$BUILD_TYPE" = "mono" ]; then
    set -x
    nuget restore src/IniParser.sln
    echo
    nuget install NUnit.Console -Version 3.10.0 -OutputDirectory testrunner
    echo
elif [ "$BUILD_TYPE" = "dotnetcore" ]; then
    set -x
    dotnet restore src/IniParser.sln
    echo
else
    echo "ERROR: Unknown build type value: $BUILD_TYPE"
    exit -1
fi
