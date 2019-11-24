#!/bin/bash

# Custom lauch command for network training 

CUDA_VISIBLE_DEVICES=0 python -u nowtrain.py \
	 --model CRFVGG_prune \
	 --dataset syntheticDataset \
	 --no-save \
	 --no-visual \
	 --save_interval 6000 \
	 --no-preload \
	 --batch_size 12 \
	 --loss NORMMSSSIM \
	 --lr 0.00001 \
	 --gpus 0 \
	 --epochs 300
