density = h5read('./ground-truth/IMG_1.h5','/density');
density = imrotate(density,-90);

semantic = h5read('./ground-truth/IMG_1_mask.h5','/density');
semantic = imrotate(semantic,-90);

image = imread('./images/IMG_1.jpg');

subplot(311);
imshow(image)

subplot(312);
imshow(density,[],'Colormap',jet(256));

subplot(313);
imshow(semantic,[]); title('Semantic map')
