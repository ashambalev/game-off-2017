#!/bin/bash

GAME="gipzo/throwback"
VERSION_FILE="${PWD}/release/build_version.txt"
ZIPS_DIRECTORY="${PWD}/release/Archives"


echo -e "\x1b[1;34m | Uploading to itch.io | \x1b[0m"

./butler push $ZIPS_DIRECTORY/Throwback\(Linux\).zip $GAME:linux --userversion-file $VERSION_FILE
./butler push $ZIPS_DIRECTORY/Throwback\(Mac\).zip $GAME:osx --userversion-file $VERSION_FILE
./butler push $ZIPS_DIRECTORY/Throwback\(WebGL\).zip $GAME:webgl --userversion-file $VERSION_FILE
./butler push $ZIPS_DIRECTORY/Throwback\(Windows\).zip $GAME:win --userversion-file $VERSION_FILE
./butler push $ZIPS_DIRECTORY/Throwback\(Windows64\).zip $GAME:win64 --userversion-file $VERSION_FILE

echo -e "\x1b[1;34m | DONE! | \x1b[0m"

./butler status $GAME

