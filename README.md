# Crowd Counting - From Real to Synthetic Datasets
by Massimo Clementi and Elisa Nicolussi Paolaz, University of Trento

## Abstract
One of the most problematic aspect in neural network applications is the retrieval of annotated samples for the training phase. For crowd counting estimation, to annotate all the head-points may be challenging especially when it is necessary to deal with high density crowds. This leads inevitably to low availability of public datasets for crowd counting NN training and testing.

The goal of this project is therefore to improve the training of neural networks for crowd counting using *synthetic datasets*. The generation of both images and ground-truth data is automatically carried out by the Unity scripts, where many parameters can be tuned to obtain crowds with different properties (i.e. density, orientation, dressing) and different camera properties (i.e. number of cameras, FOV, rotation and tilt).

In this project the [Deep Structured Scale Integration Network](https://github.com/Legion56/Counting-ICCV-DSSINet) will be used to evaluate the effectiveness of the method.


## Configuration

### Virtual environment configuration
First create a new virtual environment:

	virtualenv <name>

Then activate the virtual environment we just created:

	source <name>/bin/activate
	
Install now all the packages required by the python scripts directly from the `requirements.txt` file:

	pip install -r requirements.txt
	
Finally install the CUDA packages:

	pip install torch==1.3.1+cu92 torchvision==0.4.2+cu92 -f https://download.pytorch.org/whl/torch_stable.html
	
To **activate** the enviroment just run `source <name>/bin/activate`.


### Synthetic dataset configuration
The custom synthetic dataset in the repository is made of several images generated with Unity and contains both `.jpg` images and `.txt` ground-truth files, with the heads coordinates.

To test/train with the given dataset first we need to generate the `.mat` ground-truth files running the matlab scripts `generate_gt` in the `train_data` and `test_data` folders.

### Neural network models download
Create `saved_models` and `pretrained_models` folders in the `Counting-ICCV-DSSINet` folder.

The neural network used in this repository requires the download of the [pretrained VGG models](https://www.dropbox.com/sh/wx8ah2c6pavod5p/AACDoJvNHrKJ_YaT_ObrCV-3a?dl=0) into the `pretrained_models` folder.

Download the other public datasets (i.e. Shanghai, UCF-CC-50) directly from the official websites.


## Running
Open `Counting-ICCV-DSSINet` folder in a terminal window.

Be sure that the virtual environment is activated (*brackets* on the left).

Look at the GPU usages with the command `nvidia-smi`.

### Training phase
Customize the training parameters in the `train.sh` script. The default configuration is:

```
CUDA_VISIBLE_DEVICES=0 python -u nowtrain.py \
         --model CRFVGG_prune \
         --dataset shanghaiA \
         --no-save \
         --no-visual \
         --save_interval 2000 \
         --no-preload \
         --batch_size 12 \
         --loss NORMMSSSIM \
         --lr 0.00001 \
         --gpus 0 \
         --epochs 900
```

Adjust batch_size accordingly to your GPU memory availability.

Run the command

	nohup ./train.sh &

to train the network and check `saved_models/<train_name>/log.log` or `nohup.out`.

### Testing phase
Customize the training parameters in the `train.sh` script. The default configuration is:

```
CUDA_VISIBLE_DEVICES=0 python -u nowtest.py \
        --dataset shanghaiA \
        --model_name CRFVGG_prune \
        --no-preload \
        --no-wait \
        --no-save \
        --gpus 0\
        --test_batch_size 12 \
        --model_path $1 
```

Again, adjust the batch_size value accordingly to your GPU memory availability.

Run the command

	./test.sh ./saved_models/<dir_name>/<epoch>.h5

to test the model `model_name`, loading the weights from the <epoch>.h5 train result, onto the specified `dataset` train set.

---

## Documentation, guides and useful knowledge

### Unity and UMA
[`UMA`](https://assetstore.unity.com/packages/3d/characters/uma-2-unity-multipurpose-avatar-35611?aid=1100l355n&gclid=Cj0KCQjw0brtBRDOARIsANMDykaNWyJiJ18jAVqh6TUAzHMUNTEH6-V_ukT6trf6sgWGr2dky--2d9gaAikrEALw_wcB&pubref=UnityAssets%2ADyn09%2A1723478829%2A67594162255%2A336275757769%2Ag%2A1t1%2A%2Ab%2Ac%2Agclid%3DCj0KCQjw0brtBRDOARIsANMDykaNWyJiJ18jAVqh6TUAzHMUNTEH6-V_ukT6trf6sgWGr2dky--2d9gaAikrEALw_wcB&utm_source=aff)library for people generation.

[Guide](https://code.visualstudio.com/docs/other/unity) for `Visual Studio Code` code auto-completion.

Useful [video](https://www.youtube.com/watch?v=KJopeYPw7VI) to create UMA characters.

Useful [manual](https://docs.unity3d.com/Manual/InstantiatingPrefabs.html) to use instantiation scripts.

Useful [forum](https://answers.unity.com/questions/1396614/how-cound-instantiate-all-objcest-in-a-list.html) to successful create lists of GameObjects.


### Automatic login via SSH keys:
- generate a rsa keypar: `ssh-keygen`
- copy the key on the remote machine: `ssh-copy-id userid@hostname`
- now you can login without password


### Virtualenv and running
Virtualenv [guide](https://virtualenv.pypa.io/en/latest/userguide/#usage).

[Forum](https://stackoverflow.com/questions/6628476/renaming-a-virtualenv-folder-without-breaking-it) to move virtualenv and to create requirements file.

[Forum](https://stackoverflow.com/questions/14517930/print-a-comma-except-on-the-last-line-in-awk) for AWK and format printing.

