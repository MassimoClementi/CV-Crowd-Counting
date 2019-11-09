#Configuration steps for CV-Crowd-Counting

echo
echo  "=>  CV-Crowd-Counting CONFIGURATION  <="

./generateJSON.sh
python3 -W ignore ComputeCrowdDensity.py

echo " [OK!] Ready to go"
echo
