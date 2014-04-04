#!/bin/bash

rootpath=$1
#../terra_engine_src

files=`find $rootpath/  -regex .*\.pas$`

#./PreProcessor-v3.exe $files


for i in $files	
do
	echo -n "PARSE FILE $i	"
	./parsefile.sh $i
	
	if [ $? -ne 0 ]; then
		gedit $i &
		read
	fi

done



exit


	grep "(include\|define)" $i > /dev/null
	if [ $? -eq 0 ]; then
		echo "File in C: "$i
		cat $i
#		mv $i  ${i/pas/c}
		continue
	fi





