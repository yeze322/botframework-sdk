## 									-- DRAFT --



# Prebuilt Language Models

Prebuilt language models have been trained towards more sophisticated tasks for both monolingual as well as multilingual scenarios. In public preview only English models are made available.

## Models
The public preview of Orchestrator includes the following prebuilt language models available in [versions repository][2].

### pretrained.20200924.microsoft.dte.00.03.en.onnx
This is a fast and small base model with sufficient accuracy but if the accuracy and not speed and memory size is critical then consider other options. It is a 3-layer pretrained BERT model optimized for conversation for example-based use ([KNN][3]).

### pretrained.20200924.microsoft.dte.00.06.en.onnx
This is a high quality base model that strikes the balance between size, speed and accuracy. It is a 6-layer pretrained BERT model optimized for conversation for example-based use ([KNN][3]). This is the default model used if none explicitly specified.

### pretrained.20200924.microsoft.dte.00.12.en.onnx
This is a highest quality base model but is larger and slower than other options. It is a 12-layer pretrained BERT model optimized for conversation for example-based use (KNN).

### pretrained.20200924.microsoft.dte.00.12.roberta.en.onnx
This is a high quality base model but it is larger and slower than some other options. It is a 12-layer pretrained RoBERTa model optimized for conversation for example-based use ([KNN][3]).

## References

* [UniLMv2 Paper][1]

* [Base Models Versions Repository][2]

* [KNN (K nearest neighbors algorithm)][3]

[1]: https://arxiv.org/abs/2002.12804 "UniLMv2: Pseudo-Masked Language Models for Unified Language Model Pre-Training"
[2]: https://aka.ms/nlrversions
[3]: https://en.wikipedia.org/wiki/K-nearest_neighbors_algorithm

