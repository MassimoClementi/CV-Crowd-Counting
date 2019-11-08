'''
    Author: Massimo Clementi
    Date: 06/11/19
    Project: Density map generator from ground-truth head positions
'''

import numpy as np
import cv2
import matplotlib.pyplot as plt
import h5py
import json


''' PARAMETERS '''
json_path = "synthetic_database.json"
n_neighbours = 8
beta = 0.3  # density map generator parameter
enable_plot_flag =  0   # set to 1 to enable matplot window


''' FUNCTIONS '''
def gaussian_kernel(size_x, size_y, sigma):
    x = cv2.getGaussianKernel(size_x, sigma)
    y = cv2.getGaussianKernel(size_y, sigma)
    kernel = y.dot(x.T)
    return kernel


''' MAIN PROGRAM'''
print("\n== DENSITY MAP GENERATOR ==")
print("Opening",json_path,"...\n")
with open(json_path, 'r') as json_file:       
        paths = json.load(json_file)

for path in paths:
    print("Parsing",path)

    # Open image file
    image_size = plt.imread(path).shape

    # Open txt reference head file
    txt_path = path.replace('.png', '.txt').replace('images', 'ground-truth')
    data_file = open(txt_path,"r")

    # Setting up variables
    n_x = image_size[1]
    n_y = image_size[0]
    pad_x = round(n_x/2)
    pad_y = round(n_y/2)
    head_positions = np.loadtxt(data_file)
    distances = np.zeros(len(head_positions))
    density_map = np.zeros((2*n_y,2*n_x)) # padded density map

    print(">",len(head_positions),"reference head points found!")

    if enable_plot_flag==1: plt.subplot(211)
    for head in range(0,len(head_positions)):

        # Show reference head points
        plt.scatter(head_positions[head][0],head_positions[head][1])

        # Computation of average neighbours distance
        for i in range(0,len(head_positions)):
            distances[i] = np.linalg.norm(head_positions[head]-head_positions[i])
        distances = np.sort(distances)
        avg_distance = np.average(distances[1:n_neighbours+1])
        
        # Creating kernel with proper size and variance
        k_x = pad_x+1
        k_y = pad_y+1
        variance = beta * avg_distance
        kernel = gaussian_kernel(k_x,k_y,variance)
        
        # Variable shift + fixed padded map shift for each component
        sh_x = int(head_positions[head][0])     # variable
        sh_x += pad_x - round(k_x/2)+1          # fixed
        sh_y = int(head_positions[head][1])
        sh_y += pad_y - round(k_y/2)+1

        # Add kernel to padded density map
        density_map[sh_y:k_y+sh_y,sh_x:k_x+sh_x] += kernel

    # Cropping and flipping for final map
    final_density_map = density_map[pad_y:n_y+pad_y,pad_x:n_x+pad_x]

    # Estimate the number of heads from density map
    print("> Total head points obtained from the density map: ",\
            np.sum(final_density_map,axis=(0,1)),"\n")

    # Save final density map to .h5 dataset
    h5_path = path.replace('.png', '.h5').replace('images', 'ground-truth')
    outh5 = h5py.File(h5_path,"w")
    dataset = outh5.create_dataset('density',\
                data=np.flip(final_density_map,axis=(0,1)))

    #TODO: generate and save (fake) semantic map

    # Plotting and visualization
    if enable_plot_flag==1:
        plt.xlim((0,n_x)); plt.ylim((0,n_y)); plt.title("Head Positions")
        plt.subplot(212); plt.imshow(final_density_map)
        plt.xlim((0,n_x)); plt.ylim((0,n_y)); plt.title("Final Density Map")

        plt.show()
