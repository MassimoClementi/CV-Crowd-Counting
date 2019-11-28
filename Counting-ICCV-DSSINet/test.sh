#!/bin/bash

# =========================================
# Custom launch command for network testing
# =========================================

# Clear junk in dataset test paths
rm -rf ../SyntheticDataset/test_data/images/._*
rm -rf ../SyntheticDataset/test_data/ground_truth/._*

# Run nowtest code with custom flags
CUDA_VISIBLE_DEVICES=0 python -u nowtest.py \
	--dataset syntheticDataset \
	--model_name CRFVGG_prune \
	--no-preload \
	--no-wait \
	--no-save \
	--gpus 0\
	--test_batch_size 12 \
	--model_path $1 
