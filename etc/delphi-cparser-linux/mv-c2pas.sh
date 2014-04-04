#!/bin/bash

if [ "$1" == "" ]
then
	echo File not specified
	exit
fi

file=$1

mv $file ${file/\.pas/.c}



