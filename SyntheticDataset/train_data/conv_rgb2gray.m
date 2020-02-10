pathimg = 'images';
images = dir([pathimg filesep '*.jpg']);
nimages = length(images);

P = 30; % probability of conversion

for i = 1:nimages
    if(rand*100 <= P)
        Irgb = imread(strcat(pathimg, '/', images(i).name)); 
        Igray = rgb2gray(Irgb);
        imwrite(Igray,strcat(pathimg, '/', images(i).name));
    end
end