#!/bin/bash
 
ALLOWDEFAULT=false
APPNAME="Throwback"

DIRECTORY="${PWD}/release"
PROJECT_PATH="${PWD}/throwback"
 
echo -e "\x1b[1;34m | Creating directories | \x1b[0m"
 
#will make all the req directories
mkdir -p ${DIRECTORY}/Mac
mkdir -p ${DIRECTORY}/Windows/${APPNAME}
mkdir -p ${DIRECTORY}/Windows64/${APPNAME}
mkdir -p ${DIRECTORY}/Linux/${APPNAME}
mkdir -p ${DIRECTORY}/WebGL/${APPNAME}
mkdir -p ${DIRECTORY}/Archives
 
 
#################################################
# unity build command
 
 
LOGFILE="${DIRECTORY}/UnityBuildLog.txt"
UNITY="/Applications/Unity/Unity.app/Contents/MacOS/Unity"
OSX="${DIRECTORY}/Mac/${APPNAME}.app"
WIN="${DIRECTORY}/Windows/${APPNAME}/${APPNAME}.exe"
WIN64="${DIRECTORY}/Windows64/${APPNAME}/${APPNAME}.exe"
LIN="${DIRECTORY}/Linux/${APPNAME}/${APPNAME}"
WGL="${DIRECTORY}/WebGL/${APPNAME}"
FLAGS="-quit -batchmode -logfile ${LOGFILE} -projectPath ${PROJECT_PATH}"
 
echo -e "\x1b[1;34m | Starting builds | \x1b[0m"

#run the command
echo -e "\x1b[1;32m -> Building Mac... \x1b[0m"
$UNITY $FLAGS -executeMethod Builder.Build $OSX OSX
echo -e "\x1b[1;32m -> Building Windows... \x1b[0m"
$UNITY $FLAGS -executeMethod Builder.Build $WIN Win
echo -e "\x1b[1;32m -> Building Windows 64... \x1b[0m"
$UNITY $FLAGS -executeMethod Builder.Build $WIN64 Win64
echo -e "\x1b[1;32m -> Building Linux... \x1b[0m"
$UNITY $FLAGS -executeMethod Builder.Build $LIN Linux
echo -e "\x1b[1;32m -> Building WebGL... \x1b[0m"
$UNITY $FLAGS -executeMethod Builder.Build $WGL WebGL
 
 
#################################################
# zip that stuff up

echo -e "\x1b[1;34m | Creating archives... | \x1b[0m"
 
#pushes directories up the 'stack'
#piping to > /dev/null to suppress messages
pushd ${DIRECTORY}/Linux > /dev/null
pushd ${DIRECTORY}/Windows > /dev/null
pushd ${DIRECTORY}/Windows64 > /dev/null
pushd ${DIRECTORY}/Mac > /dev/null
pushd ${DIRECTORY}/WebGL > /dev/null
 
#zip then pop the next one
zip -r ${DIRECTORY}/Archives/${APPNAME}\(WebGL\) * >> ${LOGFILE}
popd >/dev/null
zip -r ${DIRECTORY}/Archives/${APPNAME}\(Mac\) * >> ${LOGFILE}
popd > /dev/null
zip -r ${DIRECTORY}/Archives/${APPNAME}\(Windows64\) * >> ${LOGFILE}
popd > /dev/null
zip -r ${DIRECTORY}/Archives/${APPNAME}\(Windows\) * >> ${LOGFILE}
popd > /dev/null
zip -r ${DIRECTORY}/Archives/${APPNAME}\(Linux\) * >> ${LOGFILE}
popd >/dev/null
 
echo -e "\x1b[1;34m | DONE! | \x1b[0m"
