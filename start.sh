#!/bin/bash

git pull
dotnet build -c Release
# Using cd rather than just running dotnet because of working directory breaking where config files are loaded from,
# and I'm too lazy to figure out how to run `dotnet` such that it also changes the working directory
cd MorphineBot/bin/Release/net5.0
dotnet MorphineBot.dll