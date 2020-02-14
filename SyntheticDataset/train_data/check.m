pathimg = 'images';
images = dir([pathimg filesep '*.jpg']);
nimages = length(images);

pathgt = 'ground_truth';
gts = dir([pathgt filesep '*.mat']);
ngts = length(gts);

id = 1;
for i = 1:nimages
    filename = strcat(pathimg,'/',images(i).name);
    img = imread(filename);
    filename = strcat(pathgt,'/',gts(i).name);
    load(filename);
    
    imshow(img);
    hold on;
    scatter(image_info{1,1}.location(:,1), image_info{1,1}.location(:,2), 80,'.g');
    hold off;
    pause
end