![backplane logo](./resources/BackplaneIcon.png)

# Hello World! with Backplane 
** This is for demonstration only and is not the strategic way of finsemble integration. Such integration services are out of scope of backplane and would be part of vendor implementation compliant to bridging specs.

## Use case
User want a workflow automation where on applying *filter in an app running in context of finsemble on Desktop A is automatically applied to apps running in context of finsemble on Desktop B* to avoid hassle of manual copy paste.

## Workflow

Broadcast

Grid App[Finsemble: Desktop A] &harr; Bridge[Finsemble: Desktop A]  &harr; Backplane[Desktop A]  &harr;(Network)&harr; Backplane[Desktop B] &harr; Bridge[Finsemble: Desktop B] &harr; Chart App[Finsemble: Desktop B]

## Set Up

### On Desktop A


Download latest version of backplane:
1. go to [backplane-releases](https://github.com/finos/backplane/releases?q=finos-fdc3-backplane).
2. Download finos-fdc3-backplane-vx.x.xX.zip from assets.
3. Unzip the zip folder.


*If multi desktop interop is not required, skip and jump to step 6*


4. Open appsettings.json file.
5. Navigate to key MultiHostConfig and add the address of Desktop B. Ex below

```JSON
- "MultiHostConfig": {
    "MemberNodes": [
      //put your member nodes url here
      //example: 
      "http://Desktop_B:4475"
    ]
  },
  ```
  6. Run 'Finos.Fdc3.Backplane.exe'.
  7. If multi desktop interop is not required, jump to [finsemble-backplane bridge setup](#finsemble-backplane-bridge)  

  ### On Desktop B


Download latest version of backplane:
- go to [backplane-releases](https://github.com/finos/backplane/releases?q=finos-fdc3-backplane).
- Download finos-fdc3-backplane-vx.x.xX.zip from assets.
- Unzip the zip folder.
- Open appsettings.json file.
- Navigate to key MultiHostConfig and add the address of Desktop A. Ex below

```JSON
- "MultiHostConfig": {
    "MemberNodes": [
      //put your member nodes url here
      //example: 
      "http://Desktop_A:4475"
    ]
  },
  ```
- Run 'Finos.Fdc3.Backplane.exe'.

### Finsemble-Backplane Bridge

- Download the seed project as zip from https://github.com/Finsemble/finsemble-seed/tree/release/7.3.2

- Install dependencies:
``` NPM
cd finsemble-seed
yarn install
```
- Set up the bridge service and launch finsemble as described here: [readme](../samples/FinsembleIntegration/README.md)

