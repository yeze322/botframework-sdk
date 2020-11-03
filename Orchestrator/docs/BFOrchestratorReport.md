# Report Interpretation

Orchestrator CLI can evaluate the performance of a language understanding applicationa
along with the back-end base language model it uses. This document describes what is in a
evaluation report.
After a user runs the bf-orchestrator-cli:test command, it creates report files in HTML format
along with some auxiliary output files.
For the "test" and "evaluation" model, an evaluation report contains the following sections/tabs.

- Intent/Utterancce Statistics  -- descriptive statistics of labels and utterances for an evaluation set
- Utterance Duplicates          -- utterances with duplicate or multiple labels.
- Ambiguous                     -- ambiguous predictions
- Misclassified                 -- misclassified predictions
- Low Confidence                -- low-confidence predictions
- Metrics                       -- machine learning metrics of the evaluation

## Intent/Utterancce Statistics

This section contains the descriptive statistics regarding to the evaluation set.
It has two statistical sections, one for labels, the other utterances:

- Label statistics
- Utterance statistics

### Label statistics

Label statistics lists the number of utterances labeled to each label.
Additional metrics include utterance prevalence (ratio) for every label.
The distributions can give Orchestrator users an overall view of the labels and utterances,
and whether the distributions are skewed and emphasize too much on some labels, but not others.

### Utterance statistics

On the other hand, utterance statistics focus on the #label distribution by the utterances.
Some utterances are labeled with more than one intents, which might not be desirable.
This table reflects the distribution of multi-label utterances.

## Utterance Duplicates

In this section, the report lists the utterances tagged with more than one labels.
Sometimes some dataset might contain utterances tagged with the same labels multiple times.
The report also lists the redundancy.

- Multi-label utterances and their labels
- Duplicate utterance and label pairs

## Ambiguous

For an evaluation utterance, if an Orchestrator model correctly predicts its intent label, then it's
a true positive prediction. However every prediction comes with a score, which is
essentially the probability and confidence for the prediction.
If the Orchestrator model also makes a high-score prediction close to that of the correctly predicted
label, then we call such a prediction "ambiguous."

In this section, the report lists all the utterances with an ambiguous prediction in a table.
The table has several columns:

- Utterance         --
- Labels            -- the true labels for the utterance
- Predictions       -- the labels predicted by the Orchestrator model
- Close Predictions -- some other labels predicted with a close high score to that of the predicted label. 

Besides the prediction score, the report also shows the closest example to the utterance
within the label's utterance set.

## Misclassified

For an evaluation utterance, if an Orchestrator model falsely predicts its intent label, then
this prediction is a mis-classified case.
Usually the label wiht the highest prediction score is chosen as the predicted label, but
it can be different from the ground-truth label for the utterance.

Similar to the last section, the report also lists the prediction and ground-truth labels with
their prediction scores and closest examples.

## Low Confidence

Sometimes a prediction may be predicted correctly with the highest scores among all labels, but
the score is very lower, lower than a threshold. We call such predictions low confidence.

Just like the last sections, the report lists the prediction and ground-truth labels with
their prediction scores and closest examples.

## Metrics

For machine-learning practictioners, they likely want to know the overall model performance
expressed in machine learning metrics. In this section, the report calculates
some common metrics in two sections:

- Confusion matrix metrics
- Average confusion matrix metrics

### Confusion matrix metrics

In this table, the Orchestrator CLI test command reads an evaluation set with ground-truth labels.
An evaluation set contains a collection of utterances and their labels.
It then calls the Orchestrator base model and makes a prediction for every utterance in the set
and generate predicted labels for every utterance.
It then compares the predicted labels against the ground-truth labels and creates a table of per-label
binary confusion matrices.

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

By the way, for entity extraction problem, there can be way too many false negative as an entity label
contains an entity name, the entity offset in an utterance and its length. The latter two can be predicted
with many combination. Therefore, it is a customary not to count true negatives.
Thus, for intent prediction evaluation, all four cells are used to calculate confusion matrix metrics,
but only the first three are used for entity.

Using the four cells, the Orchestrator test command can then calculate some
more sophisticated metrics, including

    - Precision
    - Recall
    - F1

For details of all the confusion matrix metrics, please reference wikipedia.

### Average confusion matrix metrics

Since an Orchestrator can evaluate multiple labels in one confusion matrix each, there can be many
metrics for analysis. For reporting and comparison purpose, it would be great to aggregate all
these metrics for overall performance.

There are many nuanced ways to aggregate confusion matrix metrics. For comparing models, it's critical
to compare based on a consistent formula.

## References

- [Wikipedia: Confusion matrix](https://en.wikipedia.org/wiki/Confusion_matrix)

## Links

- [1]:https://aka.ms/bforchestratorcli	"BF Orchestrator CLI"
