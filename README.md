# AgenceTraining

### Requirements

- [Ml-agents Release 1][1]
- [Unity Version 2019.1.14][2]

[1]: https://github.com/Unity-Technologies/ml-agents/blob/release_1_docs/docs/Installation.md
[2]: https://unity3d.com/get-unity/download/archive

### Running Trainings

To run a training, open the project in unity and create a build, including the game agents scene in the build.

![Build Info][buildInfo]

[buildInfo]: https://raw.githubusercontent.com/transitional-forms-inc/AgenceTrainingEnvironment/master/docs/buildImg.png?token=AGB6HBTIUSXJ4BMSYFIMGGC7NTDUE

Then run your training using ML-agents' _mlagents-learn_ command in your python environment.

_Example Command: 'mlagents-learn config\config.yaml --env Builds\pushAgents\AgenceTrainingUpdate.exe --base-port 6530 --run-id pushAgents --keep-checkpoints 200'_

### Creating New Trainings

The best place to start with creating new trainings is in the planet script in your scene. In the gameAgents scene this is the BasicFiveAgentPlanet script.

This script controls the rewards that are passed to the agent, plus the resetting of the environment. Please modify this script with new rewards or planet reset mechanics to drive new interactions in your agents.

If you aren't recieving the results you'd like, also consider modifying your config file. More info [here](https://github.com/Unity-Technologies/ml-agents/blob/release_1_docs/docs/Training-Configuration-File.md).

Do note, all submitted agents must have an unmodified observation and action space. Brains that extend or contract observations or actions will be rejected.

### Submitting Trainings

If you come across interesting trainings, you can submit them for creative review by filling out this form:
https://forms.gle/7nCBWBUXus3nAXCV7
