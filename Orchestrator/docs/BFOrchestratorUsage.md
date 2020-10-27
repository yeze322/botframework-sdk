# BF Orchestrator Command Usage



[BF Orchestrator Command ][1] is used to manage Orchestrator language understanding assets - retrieve, create, evaluate, and tune Orchestrator models to be used in bot development. Typical use is for intent routing to the appropriate language service, LUIS or QnA Maker for additional language tasks such as entity extraction or answer retrieval. This use was previously enabled by use of the legacy *dispatch* CLI. In addition, intent routing can be used to delegate conversations to a backend bot skill. Entity recognition is on the roadmap as a future capability.

## Primary Scenario

The mainstream bot language recognition development cycle with Orchestrator is generally as follows:

1. Create Intent label / utterance examples file in [.lu format][2]  (will be referred to as the *label file*). 
2. Download Natural Language Representation ([NLR][4]) base model (will be referred to as the *base model*).
3. Combine the label file .lu from (1) with the base model from (2) to create a .blu file (will be referred to as the *snapshot* file). 
4. Test and refine quality of utterance to intent recognition.
5. Integrate Orchestrator language recognizer in your bot.

## Steps

We will use the primary workflow to illustrate how to use Orchestrator commands for full development cycle.

### 1. Prepare label files

If you are developing a new language model, simply refer to [Language understanding][8] documentation to author label files. Depending on how you plan to design your bot, you may use a single label file corresponding to a single snapshot file for the entire bot or multiple pairs, each for every adaptive dialog where you plan to use Orchestrator as a recognizer. 

In case of migration from legacy dispatch, you may need to retrieve your LUIS application language model using the ```bf luis:version:export``` command to .lu file or QnA Maker knowledgebase using ```bf qnamaker:kb:export```  to .qna format. 

**TBD**: See sample (or example line) here...

### 2. Create snapshot files

Create a new folder, say *models*, and download the default base model using: 

```
bf orchestrator:basemodel:get
```

Next use the  ```bf orchestrator:create``` command to combine the base model with the label file(s) to create snapshot file(s) for use by the orchestrator recognizer. If using a single folder, create it prior, say *generated*, and specify it in --out parameter:

```
bf orchestrator:create --model <base model folder> --in <label file folder> --out <generated folder>
```

**TBD** explain other variations on create e.g. fullEmbeddings, hierarchical

If you already have an adaptive dialogs solution with .lu label files in different dialog folders for which you would like to create a top dispatcher, you need to use the  ```bf orchestrator:build``` to process the folder hierarchy and create snapshot files for each dialog (optionally a corresponding scaffold .dialog file for declarative scenario)

**TBD** Explain add & add build command

```
bf orchestrator:build ...
```

See also the ```orchestrator:basemodel:list``` command if you wish to experiment with different base models (see descriptions [here][4] ).

### 3. Evaluate language model

Create a label .lu file with test data set of utterances. Run the following command to generate report for your language model

```
bf orchestrator:test --in <snapshot file> --out <report folder> --test <test data file>
```

A few parameters that could be effective in further tuning of language recognition are as follows: 

**TBD**: which, how to use, how are those specified in Composer or adaptive dialogs?

See also [bf orchestrator test](https://github.com/microsoft/botframework-cli/tree/beta/packages/orchestrator#bf-orchestratortest) for full command line options.

See also [Report Interpretation][6] for how to use the report to fine tune your language model. 

### 4. Use Orchestrator language model

Once satisfied with your language model performance, it is time to integrate the model in your botby specifying Orchestrator as the recognizer. Depending on the flavor of solution there are several methods to hook up Orchestrator. 

See the specific variations for your solution below.

##### Composer Scenario

Once the feature flag is enabled in  [Bot Framework Composer][5] it is possible to specify  Orchestrator as a dialog recognizer and supply the resulting label file from above or use language data as you would normally for LUIS. This enables the basic  intent recognition. For more advanced scenarios follow the steps above to import and tune up routing quality. See more about Composer Recognizers [here](https://docs.microsoft.com/en-us/composer/concept-dialog#recognizer).

At the moment only the default base model is available to Orchestrator solutions.

**TBD**: See sample (or example line) here...



##### Non-Adaptive (V4) Scenario

**TBD**: is this waterfall?

Once the language model is ready integrate 

**TBD**: See sample (or example line) here...



##### Adaptive Dialog Scenario

**TBD** 



##### Declarative Dialog Scenario

**TBD**

## Advanced Command

The advanced language recognition development cycle assumes some level understand of machine learning concepts and interactive iterations over the language example definition and potentially evaluation of different models.

For the advanced scenario please refer to the [Interactive Command][7] page.

## Report Interpretation

A particularly important step in tuning your language model is testing and evaluating your language model performance. Orchestrator command produces detailed report to assist in optimizing the language model.

See [Report Interpretation][6] for more.

## References

- [Orchestrator](https://aka.ms/bf-orchestrator)
- [Language Understanding](https://docs.microsoft.com/en-us/composer/concept-language-understanding)
- [Composer](https://docs.microsoft.com/en-us/composer/introduction)
- [Natural Language Representation Models](https://github.com/microsoft/botframework-cli/blob/main/specs/nlrmodels.md)
- [Wikipedia: Training, validation, and test sets](https://en.wikipedia.org/wiki/Training,_validation,_and_test_sets)
- [Machine Learning Mastery](https://machinelearningmastery.com/difference-test-validation-datasets/).



[1]:https://aka.ms/bforchestratorcli	"BF Orchestrator CLI"
[2]:https://docs.microsoft.com/en-us/azure/bot-service/file-format/bot-builder-lu-file-format?view=azure-bot-service-4.0 "LU File Format"
[3]:https://docs.microsoft.com/en-us/composer/concept-language-understanding "Language Understanding"
[4]:https://aka.ms/NLRModels "NLR Models"
[5]:https://docs.microsoft.com/en-us/composer/introduction "Composer"
[6]:https://aka.ms/bforchestratorreport "Orchestrator Report"
[7]:https://aka.ms/bforchestratorinteractive "Orchestrator Interactive Command"
[8]:https://docs.microsoft.com/en-us/composer/concept-language-understanding "Language understanding"

[9]:https://en.wikipedia.org/wiki/Training,_validation,_and_test_sets "ML testing"







