// update the version number property in appPackage/manifest.json file, the version number uses semver format, the patch number is incremented by 1
// usage: node increment-version.js

const fs = require('fs');
const path = require('path');

const manifest = require('../appPackage/manifest.json');

const version = manifest.version.split('.');
const patch = parseInt(version[2]) + 1;
version[2] = patch;
manifest.version = version.join('.');
fs.writeFileSync(path.join(__dirname, '../appPackage/manifest.json'), JSON.stringify(manifest, null, 2));

console.log(`version updated to ${manifest.version}`);
