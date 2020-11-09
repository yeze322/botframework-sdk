# Report Interpretation

Use the BF CLI Orchestrator command to evaluate the performance of an Orchestrator snapshot file (with .blu extension).  A snapshot is composed of natural language representation base model (see [models][3]) along with a set of examples as provided in a label file (typically a [.lu file][4]). The snapshot file is used in Bot Framework to detect intents from user utterances. 

In order to achieve high quality natural language processing (e.g. intent detection), it is necessary to assess & refine the quality of the model. Although this is much simplified in Orchestrator thanks to its use of pre-trained models, this optimization cycle is still required in order to account for human language variations. 

BF CLI contains several commands that can produce a report, most notably bf [orchestrator:test][5] command.  See more on Machine Learning evaluation methodology in the [References](# references) section below.

Use the following guidance to interpret the report.



# Report Organization

The test command thus produces a folder with HTML report and a few supporting artifacts as follows:

- orchestrator_testing_set_ground_truth_instances.json: **TBD**
- orchestrator_testing_set_labels.txt: **TBD**
- orchestrator_testing_set_prediction_instances.json: **TBD**
- orchestrator_testing_set_scores.txt: **TBD**
- orchestrator_testing_set_summary.html: Report summary in HTML format

The report summary contains several sections as follows:

## Intent / Utterance Statistics  

This section contains descriptive statistics **TBD: Bot audience is not familiar with term descriptive statistics. Use simpler language** of labels and utterances.

It has two statistical sections, one for labels, the other utterances:

- Label statistics
- Utterance statistics

### Label statistics

Label statistics lists the number of utterances labeled to each label. Additional metrics include utterance prevalence (ratio) for every label. The distributions can give Orchestrator users an overall view of the labels and utterances, and whether the distributions are skewed and emphasize too much on some labels, but not others.

### Utterance statistics

On the other hand, utterance statistics focus on the #label distribution by the utterances. Some utterances are labeled with more than one intents, which might not be desirable. This table reflects the distribution of multi-label utterances.

### How to use this section

**TBD**



## Utterance Duplicates

This section reports on utterances with duplicate or multiple labels. A duplicate utterance is detected when it is present more than once. Thus, the report lists the utterances tagged with more than one labels. Sometimes some dataset might contain utterances tagged with the same labels multiple times.

The report also lists the redundancy.

- Multi-label utterances and their labels
- Duplicate utterance and label pairs

### How to use this section

**TBD**



## Ambiguous

This section reports on utterances ambiguous predictions. For an evaluation utterance, if an Orchestrator model correctly predicts its intent label, then it's a true positive prediction. However every prediction comes with a score, which is essentially the probability and confidence for the prediction. If the Orchestrator model also makes a high-score prediction close to that of the correctly predicted label, then we call such a prediction "ambiguous."

In this section, the report lists all the utterances with an ambiguous prediction in a table.
The table has several columns:

- Utterance         --
- Labels            -- the true labels for the utterance
- Predictions       -- the labels predicted by the Orchestrator model
- Close Predictions -- some other labels predicted with a close high score to that of the predicted label. 

Besides the prediction score, the report also shows the closest example to the utterance
within the label's utterance set.

### How to use this section

**TBD**



Sometimes some dataset might contain utterances tagged with the same labels multiple times.
The report also lists this redundancy.

- Multi-label utterances and their labels
- Duplicate utterance and label pairs

### How to use this section

**TBD**

### 

## Misclassified

This section reports on utterances with incorrect predictions. An a misclassified predication is one in which an Orchestrator model falsely predicts its intent label. Usually the label with the highest prediction score is chosen as the predicted label, but
it can be different from the ground-truth label for the utterance.

Similar to the last section, the report also lists the prediction and ground-truth labels with
their prediction scores and closest examples.



### How to use this section

**TBD**



## Low Confidence

This section reports on predictions that scored too low to be considered "confident" intent detection.  

Sometimes a prediction may be predicted correctly with the highest scores among all labels, but the score is very low, lower than the provided threshold (see more on thresholds here **TBD**). We call such predictions low confidence.

Just like the last sections, the report lists the prediction and ground-truth labels with their prediction scores and closest examples.

### How to use this section

**TBD**



## Metrics

The Metrics section is an advanced report that contains analytics that is common in Machine Learning evaluation methodologies.

Advanced machine-learning practitioners may analyze the overall model performance expressed in machine learning metrics. In this section, the report calculates some common metrics in two sections:

- Confusion matrix metrics
- Average confusion matrix metrics

### Confusion matrix metrics

In this table, the Orchestrator CLI test command reads an evaluation set with ground-truth labels. An evaluation set contains a collection of utterances and their labels. It then calls the Orchestrator base model and makes a prediction for every utterance in the set and generate predicted labels for every utterance. It then compares the predicted labels against the ground-truth labels and creates a table of per-label binary confusion matrices.

For a binary confusion matrix, there are four cells:
    - #True Positives (TP)
        - #False Positives (FP)
        - #False Neagatives (FN)
        - #True Negatives (TN)

For a label, if it exists in an utterance's ground-truth label set as well as in the predicted label set,
then that utterance is a TP for the label.
If the label only exists in the utterance's predicted label set, then it's a false positive.
If the label only exists in the utterance's ground-truth set, then it's a false negative.
If the label does not exist in either the ground-truth or predicted set, then it's a true negative.

By the way, for entity extraction, as a label contains an entity name, entity offset in an utterance and its length,
there can be numerous true negatives, since there are limitless combinations of the entity attributes
not in the ground-truth or the predicted sets.
Thus, for intent prediction evaluation, all four cells are used to calculate confusion matrix metrics,
but only the first three are used for entity-extraction evaluation.

Using just the first three cells of a binary confusion matrix,
the Orchestrator "test" command can then calculate some
more sophisticated metrics, including

    - Precision     - TP / (TP + FP)
    - Recall        - TP / (TP + FN)
    - F1            - harmonic mean of precision and recall

These three metrics do not use TN, but the simple accuracy metric need all 4 cells, including TN.

For details of many confusion matrix metrics, please reference [Wikipedia: Confusion matrix][2].

### Average confusion matrix metrics

Since Orchestrator can evaluate multiple labels, one confusion matrix for each, there can be many
metrics for detailed analysis. For reporting and KPI purpose, it would be great to aggregate all
these metrics for an overall metric and model performance.

There are many nuanced ways to aggregate confusion matrix metrics. For comparing models, it's critical
to compare based on a consistent formula. Please reference the [BF Orchestrator CLI][1] readme page for advanced CLI usage details.

## References

- [BF Orchestrator CLI](https://aka.ms/bforchestratorcli)
- [Wikipedia: Confusion matrix](https://en.wikipedia.org/wiki/Confusion_matrix)
- [Wikipedia: Training, validation, and test sets](https://en.wikipedia.org/wiki/Training,_validation,_and_test_sets)
- [Machine Learning Mastery](https://machinelearningmastery.com/difference-test-validation-datasets/).

## Links

[1]:https://aka.ms/bforchestratorcli	"BF Orchestrator CLI"
[2]:https://en.wikipedia.org/wiki/Confusion_matrix	"Wikipedia: Confusion matrix"
[3]:https://aka.ms/nlrmodels	"NLR Models"
[4]:https://docs.microsoft.com/en-us/azure/bot-service/file-format/bot-builder-lu-file-format?view=azure-bot-service-4.0 ".LU format"
[5]: https://github.com/microsoft/botframework-cli/tree/beta/packages/orchestrator#bf-orchestratortest "bf orchestrator:test"