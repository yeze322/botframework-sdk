# Skills: Validating skill functionality (DRAFT) <!-- omit in toc -->

## Summary <!-- omit in toc -->

Functional tests aim to ensure skills and skill consumers function correctly across the breadth of the Bot Framework.

### High level design goals <!-- omit in toc -->

1. Validate existing functionality consistently, identify issues and potential regressions.
2. New functionality can be easily tested, without the need to recreate the complex topologies required when working with skills.
3. The test infrastructure can be used either directly or as a template for support scenarios to repro customer issues.
4. Execute automated functional tests regularly (as part of the CI/CD pipeline, on a regular schedule or triggered manually).
5. Ensure a skill built with any of the languages supported by the SDK will work with any other bot built with a different language SDK.

To support these goals, the testing infrastructure used to validate the functional tests derived from this document must be carefully considered.

## Contents <!-- omit in toc -->

- [Scenarios](#scenarios)
  - [1. Single turn interaction with a skill](#1-single-turn-interaction-with-a-skill)
  - [2. Multi turn interaction with a skill](#2-multi-turn-interaction-with-a-skill)
  - [3. Skill sends a proactive message to consumer](#3-skill-sends-a-proactive-message-to-consumer)
  - [4. Card actions that generate invokes and message activities](#4-card-actions-that-generate-invokes-and-message-activities)
  - [5. The skill needs to authenticate the user with an OAuthCard](#5-the-skill-needs-to-authenticate-the-user-with-an-oauthcard)
  - [6. The consumer authenticates the user and passes OAuth credentials to the skill using SSO](#6-the-consumer-authenticates-the-user-and-passes-oauth-credentials-to-the-skill-using-sso)
  - [7. A skill uses team specific APIs](#7-a-skill-uses-team-specific-apis)
  - [8. Skill calls another skill](#8-skill-calls-another-skill)
  - [9. A skill provides a teams task module](#9-a-skill-provides-a-teams-task-module)
  - [10. A skill receives an attachment](#10-a-skill-receives-an-attachment)
  - [XX. Draft scenarios](#xx-draft-scenarios)
- [Reference](#reference)
  - [Things a skill might want to do](#things-a-skill-might-want-to-do)
  - [Variables](#variables)
  - [Consumer/Skill architecture](#consumerskill-architecture)
    - [Simple](#simple)
    - [Multiple skills](#multiple-skills)
    - [Multiple consumers](#multiple-consumers)
    - [Skill chaining](#skill-chaining)
    - [Complex](#complex)
    - [Circular](#circular)
- [Implementation notes](#implementation-notes)
  - [Consumers](#consumers)
  - [Skill](#skill)
    - [GetWeather skill](#getweather-skill)
    - [Travel skill](#travel-skill)
    - [OAuth skill](#oauth-skill)
    - [Teams skill](#teams-skill)
  - [Infrastructure](#infrastructure)
- [Glossary](#glossary)

## Scenarios

This section describes the testing scenarios for skills, for each one of them we provide a  high level description of the primary test case, the type of consumers used, the skill (or skills) involved and the [consumer/skill architecture](#consumerskill-architecture) used to deploy the testing components.

The different permutations between consumers, skills and their implementation language are represented using a test matrix.

The variables section lists the set of [variables](#variables) that apply to the test case and  need to be configured for each case in the matrix.

Wherever is relevant, we also include a list of alternate flows that describe small variations in the test case (e.g.: state of the consumer, state of the skill, error condition, special considerations, etc.).

Given these elements, the number of test cases for each scenario can be calculated by multiplying the number of permutations in the matrix by the number of values for each variable and then multiplied by the number of alternate flows.

### 1. Single turn interaction with a skill

> A consumer bot sends an activity to a skill (e.g.: GetWeather) and the skill responds to the user and ends.

**Testing matrix**

- Skill: [GetWeather](#getweather-skill)
- Topology: [Simple](#simple)

![Bot SDLC](media/Simple.jpg)

**Variables**

- Auth context: Public Cloud, Gov Cloud, Sandboxed
- Delivery mode: Normal, ExpectReplies

**Alternate flows**

1. Negative test, a consumer tries to call a regular bot as a skill (and the bot is not a skill).

**Total test cases:** 96 (not including alternate flows)


### 2. Multi turn interaction with a skill

> A consumer bot starts a multi turn interaction with a skill (e.g.: book a flight) and handles multiple turns (2 or more) until the skill completes the task.

**Testing matrix**

- Skill: [Travel](#travel-skill)
- Topology: [Simple](#simple)

![Bot SDLC](media/Simple.jpg)

**Variables**

- Auth context: Public Cloud, Gov Cloud, Sandboxed
- Delivery mode: Normal, ExpectReplies

**Alternate flows**

1. The consumer cancels the skill (sends EndOfConversation)
2. The consumer sends parameters to the skill
3. The skill sends a result to the consumer
4. The skill sends an event to the consumer (GetLocation) and the consumer sends an event back to the skill.
5. The skill throws and exception and fails (the consumer gets a 500 error)

**Total test cases:** 96 (not including alternate flows)

### 3. Skill sends a proactive message to consumer

> A consumer calls a _timer skill_ and the skill finishes the conversation but stores some tasks. Then, some time later the skill sends an update to the consumer as a message.

**Testing matrix**

- Skill: TBD
- Topology: [Simple](#simple)

![Bot SDLC](media/Simple.jpg)

**Variables**

- Auth context: Public Cloud, Gov Cloud, Sandboxed
- Delivery mode: Normal, ExpectReplies

**Alternate flows**

1. The skill creates a conversation (in teams) and starts a 1:1 conversation with a user in the group. Note - the 1:1 conversations created persist, and there is no way to delete them. Repeated calls to createConversation will succeed however, and return the appropriate conversationId that can be re-used.

**Total test cases:** 96 (not including alternate flows)

### 4. Card actions that generate invokes and message activities

> The Consumer invokes the skill through a dialog, and the Skill sends an Adaptive Card that collects some information with a "Submit" button using Action.Submit as the action type.

**Testing matrix**

- Skill: TBD
- Topology: [Simple](#simple)

![Bot SDLC](media/Simple.jpg)

**Variables**

- Auth context: Public Cloud, Gov Cloud, Sandboxed
- Delivery mode: Normal, ExpectReplies

**Alternate flows**

1. Skill sends proactive message that updates the card.
2. Skill sends proactive message that deletes the card.

**Total test cases:** 96 (not including alternate flows)

### 5. The skill needs to authenticate the user with an OAuthCard

> A consumer bot starts a multi turn interaction with a skill (e.g.: how does my day look like) and the skill renders an OAuthPrompt to allow the user to log in, once the skill obtains a token it performs an operation, returns a response to the user and logs it out.

**Testing matrix**

- Skill: [OAuthSkill](#oauth-skill)
- Topology: [Simple](#simple)

![Bot SDLC](media/Simple.jpg)

**Variables**

- Auth context: Public Cloud, Gov Cloud, Sandboxed
- Delivery mode: Normal, ExpectReplies

**Alternate flows**

- The Skill sends a proactive OAuthPrompt because a user token has expired.

**Total test cases:** 96 (not including alternate flows)

### 6. The consumer authenticates the user and passes OAuth credentials to the skill using SSO

> A consumer bot starts a multi turn interaction with a skill (e.g.: how does my day look like) and the skill renders an OAuthPrompt to allow the user to log in, once the skill obtains a token it performs an operation, returns a response to the user and logs it out.

**Testing matrix**

- Skill: TBD
- Topology: [Simple](#simple)

![Bot SDLC](media/Simple.jpg)

**Variables**

- Auth context: Public Cloud, Gov Cloud, Sandboxed
- Delivery mode: Normal, ExpectReplies

**Alternate flows**

- TODO

**Total test cases:** 96 (not including alternate flows)

### 7. A skill uses team specific APIs

> TODO: Currently not supported but it would involve things like:
>
> - Retrieve list of channels in a team
> - Get team info
> - Retreive the _paged_ list of uses in a group where the group is large enough to necessitate more than one page.

**Testing matrix**

- Skill: TeamsBot
- Topology: [Simple](#simple)

![Bot SDLC](media/Simple.jpg)

**Variables**

- Auth context: Public Cloud, Gov Cloud, Sandboxed
- Delivery mode: Normal, ExpectReplies

**Alternate flows**

- TODO

**Total test cases:** 96 (not including alternate flows)

### 8. Skill calls another skill

> TODO

**Testing matrix**

- Skill/Consumer: TBD
- Skill: TBD
- Topology: [Skill chaning](#skill-chaining)

![Bot SDLC](media/Chaining.jpg)

**Variables**

- Auth context: Public Cloud, Gov Cloud, Sandboxed
- Delivery mode: Normal, ExpectReplies

**Alternate flows**

- Proactively initiate a multi turn conversation.

**Total test cases:** 192 (not including alternate flows)

### 9. A skill provides a teams task module

> The Skill responds to an action/submit invoke with a `taskInfo` object containing a Task Module with an Adaptive Card
> The Skill responds to the submit action of a TaskModule

**Testing matrix**

- Skill: TeamsBot
- Topology: [Simple](#simple)

![Bot SDLC](media/Simple.jpg)

**Variables**

- Auth context: Public Cloud, Gov Cloud, Sandboxed
- Delivery mode: Normal, ExpectReplies

**Alternate flows**

- TODO

**Total test cases:** 96 (not including alternate flows)

### 10. A skill receives an attachment

> As part of a multi turn conversation a skill is expecting a file that needs to be uploaded consumer bot and then relayed to the skill

**Testing matrix**

- Skill: TBD
- Topology: [Simple](#simple)

![Bot SDLC](media/Simple.jpg)

**Variables**

- Auth context: Public Cloud, Gov Cloud, Sandboxed
- Delivery mode: Normal, ExpectReplies
- Channel: TODO not sure about this yet

**Alternate flows**

- TODO

**Total test cases:** 96? (not including alternate flows)

### XX. Draft scenarios

This section contains raw ideas to be incorporated in the scenarios enumerated above

> The consumer is actively engaged with a skill in an adaptive dialog, the skill uses adaptive dialogs, and the skill wants to send a proactive message.

> The consumer is actively engaged with a _different_ skill in an adaptive dialog, the skill uses adaptive dialogs, and the skill wants to send a proactive message.

> The consumer is not engaged with a skill, the skill uses adaptive dialogs, and the skill wants to send a proactive message.

> The consumer is not engaged with a skill, the skill uses adaptive dialogs, and the skill wants to call `createConversation` in order to send a new proactive message.

> The consumer engages with a skill in a group conversation.

> The consumer engages with a skill in a group conversation. And the skill starts a DM conversation with one of the users

Using those examples, we can extrapolate a template for creating a realistic test scenario:

> The consumer is in `someVariableState`, the skill is in `someVariableState`, and the skill wants to `performSomeAction`.

## Reference

### Things a skill might want to do

- Perform multi-turn dialogs, with child dialog/prompts
- Send proactive messages
- Receive and respond to invoke Activities
- Send cards and respond to card actions
  - Adaptive Card
    - Action.Submit
    - Action.Execute (AC 2.0)
  - Suggested Actions
  - Non-invoke actions (ImBack)
- Retrieve conversation members
  - Single member
  - All members
  - Paged members
- Update messages
- Delete messages
- Create a new conversation
- Use channel-specific functionality
  - Retrieve list of channels in a team
  - Get team info
- Authentication
  - SSO
  - OAuth prompt
  - OAuth card
  - OAuth input

### Variables

- Activity Handling (applies to both the skill and the consumer)
  - Waterfall
  - Adaptive
  - Prompts
  - Raw activity handling
- Consumer sent the Activity to the skill with "expectReplies"
- Skill is currently active
- Skill is currently inactive
- Some _other_ skill is currently active
- Parent bot is engaged in a _different_ dialog
- Authentication context, the skill and consumer are deployed to the public cloud, gov cloud, or a sandboxed environment.
- Network protocol: the consumer is accessed over straight HTTP (webchat) or Web Sockets (streaming clients)
- BotFramework version for the skill: 4.x or 3.x.
- Bot runtime: Composer bot, PVA or SDK coded bot.
- Channel: Emulator, Teams, DirectLine, DirectLine ASE (App Service Extension)
- Bot programming language: C#, JS, Python or Java.
- Bot Adapter: Skill or consumer use a OOTB adapter or custom channel adapter

### Consumer/Skill architecture

This section describes the most common consumer/skill topologies that can exist. The topologies given below are further complicated based on the variables above, as well as the SDK language of any particular bot (consumer or skill) in the topology.

One of the most important things to keep in mind here is that any bot can act as a stand-alone bot, a consumer, or a skill, and may very well fulfill all three models at different times.

#### Simple

In the simplest case there is a single consumer and a single skill.

```
C -----> S
```

#### Multiple skills

A single consumer with multiple skills.

```
      ----> S1
C --<
      ----> S2
```

#### Multiple consumers

A single skill is consumed by multiple consumers.

```
C1 --\
      ------> S
C2 --/
```

#### Skill chaining

A consumer uses a skill, which in turn consumes another skill.

```
C1 -----> C2/S1 ----> S2
```

#### Complex

Combining multiple skills, multiple consumers, and skill chaining.

```
C1 --\                              ----> S3
      ------> C3/S1 ----> C4/S2 --<
C2 --<              \               ----> S4
      ------> S5     -----> S6
```

#### Circular

A consumer uses a skill, which in turn consumes another skill, which in turn consumes the original consumer as a skill. In practice, this topology should probably be avoided, however nothing directly prevents it from occurring.

```
C1/S1 ----> C2/S2 --
   ^                \----> C3/S3 --
   |                               |
   |------------------------------/

```

## Implementation notes

Based on the scenarios described above we will need to build the following artifacts to implement and run functional tests on skills:

### Consumers

- Composer consumer bot (C# only for now)
- VA consumer bot (C# and TS)
- PVA consumer bot (C#)

### Skill

#### GetWeather skill

Composer, C# no dialogs, JS no dialogs, Python no dialogs.

#### Travel skill

Composer, C# waterfall, JS waterfall, Python waterfall.

#### OAuth skill

C#, JS, Python

#### Teams skill

C#, JS, Python

### Infrastructure

- Proactive service (C#)
- Transcript based test runner (C#)

## Glossary

- **Consumer:** A bot that passes Activities to another bot (a skill) for processing.
- **Skill:** A bot that accepts Activities from another bot (a consumer), and passes Activities to users through that consumer.
- **Active skill:** The consumer is currently forwarding Activities to the skill.
- **Inactive skill:** The consumer is not currently forwarding Activities to the skill.
