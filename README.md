# CV-Crowd-Counting
Crowd Counting - From Real to Synthetic Database

### TO-DOs:
- [X] Create script to instantiate characters with random gender and clothing
- [X] Deep learning newtork on Python2 or Python3? -> **Python2**
- [ ] [LATER] Add environment elements (i.e.buildings, trees and hydrants) + reintroduce shadows in final Unity simulations
- [ ] Check if the extracted reference coordinates are correct, need to extract them few frames later in the `Update` function?
- [ ] Change script name accordingly (i.e. `SyntheticCrowdGenerator.cs`)
- [ ] Change color distribution accordingly to Raileigh (i.e.) 75% black and blue hues
- [ ] Get number of people and head coordinates for current camera view (**DO SCRIPT**)
- [ ] virtualenv + packets to install in remote PC


---
## Paper and code
Read the following paper as reference, understand how it works, try to run it and understand what kind of input data does it need (characterize the dataset)
> http://visal.cs.cityu.edu.hk/research/residual_regression_counting/

Requirements:
- python2.7
- pytorch 0.4.1

To make it run
- download test set
- download pretrained models
- run python test.py

**N.B:** Use "Colab" by Google to run the simulations, if needed

## Unity and UMA
Install `Unity` (recommended version `2018.4`) and learn C#
> https://unity.com
> https://learn.unity.com/course/getting-started-with-unity

Install `UMA` library for people generation
> https://assetstore.unity.com/packages/3d/characters/uma-2-unity-multipurpose-avatar-35611?aid=1100l355n&gclid=Cj0KCQjw0brtBRDOARIsANMDykaNWyJiJ18jAVqh6TUAzHMUNTEH6-V_ukT6trf6sgWGr2dky--2d9gaAikrEALw_wcB&pubref=UnityAssets%2ADyn09%2A1723478829%2A67594162255%2A336275757769%2Ag%2A1t1%2A%2Ab%2Ac%2Agclid%3DCj0KCQjw0brtBRDOARIsANMDykaNWyJiJ18jAVqh6TUAzHMUNTEH6-V_ukT6trf6sgWGr2dky--2d9gaAikrEALw_wcB&utm_source=aff

Setup `Visual Studio Code` for code auto-completion using THIS guide:
> https://code.visualstudio.com/docs/other/unity

Useful video to create UMA characters:
> https://www.youtube.com/watch?v=KJopeYPw7VI

Useful guide to use instantiation scripts
> https://docs.unity3d.com/Manual/InstantiatingPrefabs.html

Useful guide to successful create lists of GameObjects
> https://answers.unity.com/questions/1396614/how-cound-instantiate-all-objcest-in-a-list.html

---
Give a look to a complete project if we need some particular snippet of code
> https://github.com/albertoxamin/HeterogenousCameraNetwork

## SSH to remote Ubuntu machine
Data
> IP: 192.168.165.4
> user: massimoclementi
> pw: massimo!clementi

Command to run to access to SSH (with `Pulse Secure`):
> ssh -Y massimoclementi@192.168.165.4

To copy from local to remote (-r is for folders):
> scp -r path_to_send massimoclementi@192.168.165.4:/destination_path/






