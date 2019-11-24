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
    filename = strcat('ground_truth/GT_',name,'.mat');
    save(filename,'image_info');
end