# Report Interpretation

Orchestrator CLI can evaluate the performance of a language understanding application along with the back-end base language model it uses. After a user runs the bf orchestrator:test command, it creates report files in HTML format along with some auxiliary output files. This document describes what is in an evaluation report.

For the "test" and "evaluation" model, an evaluation report contains the following sections:

<img src="pictures\evaluation-sessions.PNG" alt="sections"  />

- **Intent/Utterance Statistics**: descriptive statistics of labels and utterances for an evaluation set
- **Utterance Duplicates**: utterances with duplicate or multiple labels.
- **Ambiguous**: ambiguous predictions
- **Misclassified**: misclassified predictions
- **Low Confidence**: low-confidence predictions
- **Metrics**: machine learning metrics of the evaluation

## Intent/Utterance Statistics

This section contains the descriptive statistics regarding to the evaluation set.
It has two statistical sections, one for labels, the other utterances:

- Label statistics
- Utterance statistics

### Label statistics

Label statistics lists the number of utterances labeled to each label. Additional metrics include utterance prevalence (ratio) for every label.
The distributions can give Orchestrator users an overall view of the labels and utterances, and whether the distributions are skewed and emphasize too much on some labels, but not others.

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

- Utterance         -- the utterance
- Labels            -- the true labels for the utterance
- Predictions       -- the labels predicted by the Orchestrator model
- Close Predictions -- some other labels predicted with a close high score to that of the predicted label. 

Besides the prediction score, the report also shows the closest example to the utterance
within the label's utterance set.

## Misclassified

For an evaluation utterance, if an Orchestrator model falsely predicts its intent label, then
this prediction is a mis-classified case.
Usually the label with the highest prediction score is chosen as the predicted label, but
it can be different from the ground-truth label for the utterance.

Similar to the last section, the report also lists the prediction and ground-truth labels with
their prediction scores and closest examples.

## Low Confidence

Sometimes a prediction may be predicted correctly with the highest scores among all labels, but
the score is very lower, lower than a threshold. We call such predictions low confidence.

Just like the last sections, the report lists the prediction and ground-truth labels with
their prediction scores and closest examples.

## Metrics

For machine-learning practitioners, they likely want to know the overall model performance
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
        - #False Negatives (FN)
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
to compare based on a consistent formula. Please reference the [BF Orchestrator CLI][1] readme page for details.

## References

- [BF Orchestrator CLI](https://aka.ms/bforchestratorcli)
- [Wikipedia: Confusion matrix](https://en.wikipedia.org/wiki/Confusion_matrix)

## Links

[1]:https://aka.ms/bforchestratorcli	"BF Orchestrator CLI"
[2]:https://en.wikipedia.org/wiki/Confusion_matrix	"Wikipedia: Confusion matrix"
