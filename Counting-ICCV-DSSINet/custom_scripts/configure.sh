#Configuration steps for CV-Crowd-Counting

echo
echo  "=>  CV-Crowd-Counting CONFIGURATION  <="

# Double-check that the project venv is activated 
source venv/bin/activate

# Generate proper JSON of the synthetic dataset
./generateJSON.sh

# Generate density maps and semantic maps
python3 -W ignore ComputeCrowdDensity.py

echo " [OK!] Ready to go"
echo
