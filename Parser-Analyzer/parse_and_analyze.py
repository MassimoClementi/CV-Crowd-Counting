'''
	Author: Massimo Clementi
	Date: 11/02/2020

	Project: parser and analyzer of NN results

'''


import numpy as np
import matplotlib.pyplot as plt

''' Paths '''
original_path = "original.txt"
mixed_path = "mixed.txt"

''' Open files'''
original = open(original_path)
mixed = open(mixed_path)

''' Strip lines '''
original = [line.strip() for line in original if line.strip()]
mixed = [line.strip() for line in mixed if line.strip()]

ratio = np.zeros(len(original)-1)

print("\n== INDEX CORRESPONDENCIES ==\n")

for i in range(0,len(original)-1):
	or_split = original[i].split(';')
	mix_split = mixed[i].split(';')

	''' Extract gt and et values '''
	img_name = or_split[1][7:]
	gt = int(or_split[2][5:])
	et_shang = float(or_split[3][5:])
	et_cross = float(mix_split[3][5:])

	print("Index",i,"->",img_name)

	''' Compute metric '''
	ratio[i] = 10 * np.log10(np.abs((gt - et_shang)/(gt - et_cross)))


''' Total number of improvements and worsening '''
improvements = 0
worsening = 0
for val in ratio:
	if np.sign(val) == 1:
		improvements += 1
	else:
		worsening += 1
print("\nBetter in",improvements,"cases","\tWorse in", worsening,"cases\n")


''' Stem plot '''
plt.stem(range(0,len(ratio)),ratio,use_line_collection=True)
plt.xlabel("Index")
plt.ylabel(r"$10\times\log10(|\frac{gt-et\_shang}{gt-et\_cross}|)$")	# r -> raw string
plt.show()

''' Histogram '''
plt.hist(ratio,bins=int(len(ratio)/5),color='green',histtype='bar', ec='black')
plt.xlabel("M value")
plt.ylabel("Number of occurences")
plt.text(5.5,24.5,"Bin value = "+str(int(len(ratio)/5)))
plt.show()

