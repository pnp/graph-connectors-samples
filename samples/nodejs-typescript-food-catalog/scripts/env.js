const fs = require("fs");
const path = require("path");

console.log("Ensuring env files exist...");

const envPath = path.join(__dirname, "..", "env");
const envs = [
  {
    name: ".env.local",
    content: `TEAMSFX_ENV=local\nAPP_NAME=Foodsie\nTUNNEL_ID=\nNOTIFICATION_ENDPOINT=\nNOTIFICATION_DOMAIN=`,
  },
  {
    name: ".env.local.user",
    content: `SECRET_AAD_APP_CLIENT_SECRET=`,
  },
  {
    name: ".env.dev",
    content: `TEAMSFX_ENV=dev\nAPP_NAME=Foodsie\nTUNNEL_ID=\nNOTIFICATION_ENDPOINT=\nNOTIFICATION_DOMAIN=`,
  },
  {
    name: ".env.dev.user",
    content: `SECRET_AAD_APP_CLIENT_SECRET=`,
  },
  {
    name: ".env.testtool",
    content: `TEAMSFX_ENV=testtool`,
  },
];

envs.forEach((env) => {
  const envFilePath = path.join(envPath, env.name);
  if (!fs.existsSync(envFilePath)) {
    fs.mkdirSync(envPath, { recursive: true });
    fs.writeFileSync(envFilePath, env.content);
  }
});

console.log("Done!");
