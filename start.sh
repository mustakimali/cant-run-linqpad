#!/bin/bash

git update-index --assume-unchanged Program.cs
git update-index --assume-unchanged cant-run-linqpad.csproj

dotnet watch run