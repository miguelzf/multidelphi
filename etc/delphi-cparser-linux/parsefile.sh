#!/bin/bash

if [ "$1" == "" ]
then
	echo File not specified
	exit
fi

./PreProcessor-v3.exe $1 | ./parser-delphi 


