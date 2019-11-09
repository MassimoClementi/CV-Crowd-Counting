# Author: Massimo Clementi
# Date: 07/11/19
# Program: create JSON directly from the terminal

folder="./datasets/SyntheticCrowd/images/"
output="synthetic_database.json"

echo
echo -- JSON GENERATOR --
echo Parsing $folder

ls $folder > temp.txt
	#WARNING: modify here below the variable "folder" too
awk 'BEGIN{
	ORS=""
	print "[";
	folder="./datasets/SyntheticCrowd/images/"
	}{
	printf "%s\"%s%s\"",t,folder,$0;
	t=", "
	} END {
	ORS="";
	print "]";	
	}' temp.txt > $output
rm temp.txt

echo [OK!] Output to $output
echo