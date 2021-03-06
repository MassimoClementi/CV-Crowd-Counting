#!/bin/bash

# ==========================================
# Custom launch command for network training
# ==========================================

# Clear junk in dataset train paths
rm -rf ../SyntheticDataset/train_data/images/._*
rm -rf ../SyntheticDataset/train_data/ground_truth/._*

# Run nowtrain code with custom flags
CUDA_VISIBLE_DEVICES=0 python -u nowtrain.py \
	 --model CRFVGG_prune \
	 --dataset mixedDataset \
	 --no-save \
	 --no-visual \
	 --save_interval 1000 \
	 --no-preload \
	 --batch_size 12 \
	 --patches_per_sample 5 \
	 --loss NORMMSSSIM \
	 --lr 0.00001 \
	 --gpus 0 \
	 --epochs 900
