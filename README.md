## UAV Search and Rescue Environment for MLRL


### Requirements

- Python 3.8 (minimum)
- [OpenAI `baselines`](https://github.com/openai/baselines), version 0.1.6
- [`pytorch`](https://pytorch.org/get-started/locally/), < 1.9.0, >= 1.6.0 (compatible to your CUDA version)
- [`tensorboard`](https://github.com/tensorflow/tensorboard) (compatible to PyTorch version)
- [OpenAI `gym`](https://github.com/openai/gym), version 0.15.7
- [Unity `mlagents`](https://github.com/Unity-Technologies/ml-agents), version 0.27.0 (Release 18)


**Unity Editor**

- Unity Editor, version 2020.3.x (LTS) (minimum)
- Unity ML-Agents Package Release 2.1.0 exp1 (Not compatible with other versions)

<br>

### Getting Started
- install requirements packages (in a virtual-env)
- `git clone https://github.com/dmslab-konkuk/LogisticsEnv.git`
- `cd MAAC` or `cd MADDPG`
- edit parameters in main.py (learning parameters)
- Followed by your OS, select built environment file between `Build_Windows` or `Build_Linux` (give right path)
- if your OS is Linux(Ubuntu), before training grant permission is essential, with `sudo chmod a+xwr /Build_Linux/Logistics.x86_64`
- `python main.py` to run training
- To replay with your trained model, set path to the `model.pt` on `replay.py` and `python replay.py` to replay



<br>

### Scenario
![](https://images.velog.io/images/leehe228/post/8d01796d-133d-41ba-8ec6-bf43f9937032/image.png)
- UAVs need to move parcels(boxes) from **hubs** to **destinations**.
- There are two types of boxes - big boxes and small boxes. A big box can only be moved if two or more UAVs cooperate.
- The size of map and the number of UAVs and obstacles can be customized for various environments.
- When UAV touches the box, the box is connected. If UAV is farther than a certain distance from the box, the box will fall off.

<br>


**Parameter Configurations**
`EnvConfig`

- `width` : Defines the width of the display. (Must be set alongside height)
- `height` : Defines the height of the display. (Must be set alongside width)
- `timescale` : Defines the multiplier for the deltatime in the simulation. If set to a higher value, time will pass faster in the simulation but the physics may perform unpredictably.
- `quality_level` : Defines the quality level of the simulation.
- `target_frame_rate` : Instructs simulation to try to render at a specified frame rate.
- `capture_frame_rate` : Instructs the simulation to consider time between updates to always be constant, regardless of the actual frame rate.
- `name` : path to Unity Built Environment (ex : `../Build_Linux/Logistics`)
- `mapsize` : size of map in virtual environment (x by x)

<br>

### Observation
Observation size for each agent
```
29 + 7 x (nagent - 1) + (27 : ray-cast obs)
```

**This UAV Information**

- 3 : (x, y, z) coordinates of this UAV
- 3 : (x, y, z) velocity of this UAV
- 3 : one hot encoding of box type (not holding, small box, big box)
- 7 x (n - 1) : other UAVs information (3 - coordinates, 1 - distance, 3 - box type one-hot encoding)
- 6 : (x, y, z, x, y, z) big box hub and small box hub coordinates
- 2 : each distance from this to the big box hub and small box hub
- 6 : (x, y, z, x, y, z) nearest big and small box coordinates (if there's no box nearby, zero)
- 2 : each distance from this to the nearest big box and the nearest small box (if there's no box nearby, zero)
- 4 : (x, y, z, d) if UAV holds any box, the coordinates and distances are given. if not, zero

**Raycast Observation** (from [Unity ML-Agents](https://github.com/Unity-Technologies/ml-agents/blob/release_18_docs/docs/Learning-Environment-Design-Agents.md#raycast-observations))

- 1 - distance (0 if nothing is detected)
- 2 - one hot encoding of detected object (nothing, building)
- (1 + 2) x 9 - rays per direction

<br>

### Actions
UAV can move to 6 directions (up, down, forward, backward, left, right) or not move

The action is **discrete action**, and size of action set is 7.

- index 0 : not move
- index 1 : forward
- index 2 : backward
- index 3 : left
- index 4 : right
- index 5 : up
- index 6 : down

<br>

### Reward
**Driving Reward**

```python
(pre distance - current distance) * 0.5
```

To make UAV learn driving forward destination, ***distance penalty*** is given per every step. If UAV holds any parcel, the distance is calculated with a destination where the parcel have to shipped. If UAV have to pick some parcel, distance between UAV and a big box or a small box, whichever is closer to UAV is calculated.

**Shipping Reward**

- +20.0 : Pick up a small box
- +20.0 : complete small box shipping
- +10.0 : First UAV picks up a big box
- +10.0 : First UAV which holds a big box when second UAV picks up
- +20.0 : Second UAV picks up a big box
- +30.0 : complete big box shipping
- -8.0 : when the first UAV dropped a big box
- -15.0 : when tow UAVs dropped a big box

These values are designed to make UAV work efficiently.

**Collision Penalty**

- -10.0 : when UAV collide with another UAV or a building

UAV has to avoid buildings and another UAV with raycast observation.

<br>

### Training Result
We trained model with random-decision model, reinforcement model (SAC, DQN, MADDPG) and MAAC (Multi-Attention-Actor-Critic for Multi-Agent) model. We trained **30k episode** each model.

<br>

### Credit
developed by [Hoeun Lee](https://github.com/leehe228) (in [DMS Lab](https://github.com/dmslab-konkuk) in [Dept. of Computer Science and Engineering](http://cse.konkuk.ac.kr/), [Konkuk University](http://www.konkuk.ac.kr/do/Index.do), Seoul, Korea)

Copyright Hoeun Lee, 2021, All Right Reserved. 
