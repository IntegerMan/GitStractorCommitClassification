# GitStractor Model Trainig

This project is my [Franklin University](https://Franklin.edu) master's project designed to train and evaluate a git commit classification model to classify commits by their metadata and commit message as bugfix commits or non-bugfix commits via binary classification.

Key Notebooks:

- [TrainingAggregation.ipynb](TrainingAggregation.ipynb) - A Jupyter Notebook with Python built to explore overall trends, build a merged dataset, and create a representative proportional sample from the overall data in `data/input` with as little class imbalance as possible
- [LabelledEDA.ipynb](LabelledEDA.ipynb) - a Jupyter Notebook with Python built to explore the labelled dataset and review the manual label correction process on the training data
