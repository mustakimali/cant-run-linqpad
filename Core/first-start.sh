#!/bin/bash

# first start
#
git clone git@github.com:mustakimali/cant-run-linqpad.git
cd cant-run-linqpad
git update-index --assume-unchanged Program.cs
git update-index --assume-unchanged cant-run-linqpad.csproj
code .

# start.sh
#
chmod +x ./start.sh;
./start.sh