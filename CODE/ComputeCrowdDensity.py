'''
    Author: Massimo Clementi
    Date: 06/11/19
    Project: Density map generator from ground-truth head positions
'''

import numpy as np
import cv2
import matplotlib.pyplot as plt
import h5py


''' PARAMETERS '''
n_neighbours = 8
beta = 0.3  # density map generator parameter

enable_plot_flag =  1   # set to 1 to enable matplotlib window


''' FUNCTIONS '''
def gaussian_kernel(size_x, size_y, sigma):
    x = cv2.getGaussianKernel(size_x, sigma)
    y = cv2.getGaussianKernel(size_y, sigma)
    kernel = y.dot(x.T)
    return kernel


''' VARIABLES '''
#TODO: implement jsom to load multiple files
data_file = open("./Sample_images/pos_2019-11-07_16-53-56.txt","r")
image_size = plt.imread("./Sample_images/screen_854x429_2019-11-07_16-53-56.png")\
                        .shape
            #N.B: relative path, need to cd in the right directory
n_x = image_size[1]
n_y = image_size[0]
pad_x = round(n_x/2)
pad_y = round(n_y/2)
head_positions = np.loadtxt(data_file)
distances = np.zeros(len(head_positions))
density_map = np.zeros((2*n_y,2*n_x)) # padded density map


''' MAIN PROGRAM'''
print(">",len(head_positions),"reference head points found!\n")

plt.subplot(221)
for head in range(0,len(head_positions)):

    # Show reference head points
    plt.scatter(head_positions[head][0],head_positions[head][1])

    #Computation of average neighbours distance
    for i in range(0,len(head_positions)):
        distances[i] = np.linalg.norm(head_positions[head]-head_positions[i])
    distances = np.sort(distances)
    avg_distance = np.average(distances[1:n_neighbours+1])
    #print(" > Average neighbours head distance",head,"=",avg_distance)
    
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
    density_map[sh_y:k_y+sh_y,sh_x:k_x+sh_x] += kernel

# Cropping and flipping for final map
final_density_map = density_map[pad_y:n_y+pad_y,pad_x:n_x+pad_x]

# Masked Image for debugging
masked = np.multiply(\
    np.flip(plt.imread("./Sample_images/screen_854x429_2019-11-07_09-22-56.png")[:,:,0],axis=0),\
    final_density_map)

# Estimate the number of heads from density map
print("> Total head points obtained from the density map: ",\
        np.sum(final_density_map,axis=(0,1)),"\n")

# Save final density map to .h5 dataset
outh5 = h5py.File("density_map.h5","w")     #TODO: rename accordingly
dataset = outh5.create_dataset('density',\
            data=np.flip(final_density_map,axis=(0,1)))

# Plotting and visualization
if enable_plot_flag==1:
    plt.xlim((0,n_x)); plt.ylim((0,n_y)); plt.title("Head Positions")
    plt.subplot(222); plt.imshow(density_map)
    plt.xlim((0,2*n_x)); plt.ylim((0,2*n_y)); plt.title("Padded Density Map")
    plt.subplot(223); plt.imshow(final_density_map)
    plt.xlim((0,n_x)); plt.ylim((0,n_y)); plt.title("Final Density Map")
    plt.subplot(224); plt.imshow(masked)
    plt.xlim((0,n_x)); plt.ylim((0,n_y)); plt.title("Masked Image")

    plt.show()
