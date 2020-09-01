const runTracker = require('./src');

const path = require('path');
const ENV_FILE = path.join(__dirname, '.env');
require('dotenv').config({ path: ENV_FILE });
process.env.BotBaseUrl = 'http://localhost:3978';

// The Azure function expects a `context` object, which contains the
// names of all of the bindings in function.json. To process PRs,
// we need to ensure the atEightAm binding exists on console.
console.bindings = { atEightAM: true };

runTracker(console);
