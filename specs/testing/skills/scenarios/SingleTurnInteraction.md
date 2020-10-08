# Single turn interaction with a skill

> A consumer bot sends an activity to a skill (e.g.: GetWeather) and the skill responds to the user and ends.

## Testing matrix

- Skill: [GetWeather](../SkillsFunctionalTesting.md#getweather-skill)
- Topology: [Simple](../SkillsFunctionalTesting.md#simple)

![Bot SDLC](../media/Simple.jpg)

## Variables

- Auth context: Public Cloud, Gov Cloud, Sandboxed
- Delivery mode: Normal, ExpectReplies

## Variations

1. Negative test, a consumer tries to call a regular bot as a skill (and the bot is not a skill).

**Total test cases:** 96 (not including variations)
