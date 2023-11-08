![backplane logo](./resources/BackplaneIcon.png)

## Quick Start Guide

Download latest version of backplane:
- go to [backplane-releases](https://github.com/finos/backplane/releases?q=finos-fdc3-backplane).
- Download finos-fdc3-backplane-vx.x.xX.zip from assets.
- Unzip the zip folder.
- Run 'Finos.Fdc3.Backplane.exe'.

Install npm package of backplane client:

```sh

# npm
npm install @finos/fdc3-backplane-client

#yarn
yarn add @finos/fdc3-backplane-client

```
See [readme](../src/Finos.Fdc3.Backplane.Client.JS/README.md)

### Multi desktop interop through config
- Open appsettings.json file.
- Navigate to key MultiHostConfig and add the address of Desktop B. Ex below

```JSON
- "MultiHostConfig": {
    "MemberNodes": [
      //put your member nodes url here
      //example: 
      "http://Desktop_B:4475"
    ]
  },
  ```

### Finsemble-Backplane Integration

- Download the seed project as zip from https://github.com/Finsemble/finsemble-seed/tree/release/7.3.2

- Install dependencies:
``` sh
cd finsemble-seed
yarn install
```
- Set up the bridge service as described here: [readme](../samples/FinsembleIntegration/README.md)

Note: In case of multi desktop interop, both instance of finsemble on desktop A and B must have this configuration to communicate with backplane.

- Launch finsemble

 




