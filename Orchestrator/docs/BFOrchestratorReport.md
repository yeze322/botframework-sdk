# Report Interpretation

Orchestrator CLI can evaluate the performance of a language understanding applicationa
along with the back-end base language model.

After a user runs the bf-orchestrator-cli:test command, it will create report files in HTML format
along with some auxiliary output files.
For the "test" and "evaluation" model, an evaluation report contains following sections/tabs.

- Intent/Utterancce Statistics
- Utterance Duplicates
- Ambiguous
- Misclassified
- Low Confidence
- Metrics

## Intent/Utterancce Statistics

This section contains the descriptive statistics regarding to the evaluation set.
It has two sections, one for labels, the other utterances:

- Label statistics
- Utterance statistics

### Label statistics

Label statistics lists the number of utterances tagged to it. As well as the utterance prevalence (ratio)
for each label.

### Utterance statistics

Utterance statistics focus on the #label distribution by the utterances.
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

- Utterance
- Labels            - the true labels for the utterance
- Predictions       - the labels predicted by the Orchestrator model
- Close Predictions - some other labels predicted with a close high score to that of the predicted label. 

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

### Average confusion matrix metrics

## References

- [Wikipedia: Confusion matrix](https://en.wikipedia.org/wiki/Confusion_matrix)

## Links

- [1]:https://aka.ms/bforchestratorcli	"BF Orchestrator CLI"
