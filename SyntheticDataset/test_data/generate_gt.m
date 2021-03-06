%
%   Author: Elisa Nicolussi Paolaz
%   
%   Script to generate .mat ground-truth file
%   
%   It reads the head positions from the .txt file, 
%   creates and saves the struct variable which contains in its two fields
%   the array of head positions ('location') 
%   and the total number of people ('number')
%

pathgtmat = 'ground_truth';
if ~exist(pathgtmat, 'dir')
    mkdir(pathgtmat);
end

pathgt = 'ground_truth_txt';
gts = dir([pathgt filesep '*.txt']);
ngts = length(gts);

pathimg = 'images';
images = dir([pathimg filesep '*.jpg']);
nimages = length(images);

for i = 1:ngts
    fileID = fopen(strcat(pathgt, '/', gts(i).name),'r');
    formatSpec = '%f %f';
    sizeA = [2 Inf];
    A = fscanf(fileID,formatSpec,sizeA);
    A = A';
    fclose(fileID);
    
    filename = strcat(pathimg,'/',images(i).name);
    img = imread(filename);
    [r, c, ch] = size(img);
    
    [value2, ~] = size(A);
    
    for row = 1:value2
        A(row,2) = r - A(row,2);
    end
    
    image_info = cell(1);
    field1 = 'location';
    value1 = A;
    field2 = 'number';
    image_info{1,1} = struct(field1, value1, field2, value2);
    
    name = gts(i).name;
    name = strsplit(name,'.');
    name = name{1,1};
    filename = strcat(pathgtmat,'/GT_',name,'.mat');
    save(filename,'image_info');
end