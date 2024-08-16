import { config } from './Config';
const readline = require('readline');

function askQuestion(query) {
    const rl = readline.createInterface({
        input: process.stdin,
        output: process.stdout,
    });

    return new Promise(resolve => rl.question(query, ans => {
        rl.close();
        resolve(ans);
    }))
}

async function openBrowser() {  
    //console.log(`Open this link and click on "Grant admin consent for ..."\nhttps://entra.microsoft.com/#view/Microsoft_AAD_RegisteredApps/ApplicationMenuBlade/~/CallAnAPI/appId/${config.clientId}/isMSAApp~/false`);
    console.log(`https://login.microsoftonline.com/common/adminconsent?client_id=${config.clientId}`);

}

async function main() {
    try {
      await openBrowser();
      await askQuestion("When completed, press any key to continue");
    }
    catch (e) {
      console.error(e);
    }
  }
  
  main();
