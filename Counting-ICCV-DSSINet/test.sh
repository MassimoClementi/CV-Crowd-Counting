#!/bin/bash

# Custom lauch command for network testing 

CUDA_VISIBLE_DEVICES=0 python -u nowtest.py \
	--dataset shanghaiA \
	--model_name CRFVGG_prune \
	--no-preload \
	--no-wait \
	--no-save \
	--gpus 0\
	--test_batch_size 1 \
	--epoch 900 \
	--model_path $1 