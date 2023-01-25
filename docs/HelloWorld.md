![backplane logo](./resources/BackplaneIcon.png)

# Hello World! with Backplane

## Use case
User want a workflow automation were on applying filter in app running in context of finsemble on Desktop A is automaticaly applied to apps running in context of finsemble on Desktop B to avoid hassle of manual copy paste.

## Workflow

Broadcast

Grid App[Finsemble: Desktop A] &harr; Bridge[Finsemble: Desktop A]  &harr; Backplane[Desktop A]  &harr;(Network)&harr; Backplane[Desktop B] &harr; Bridge[Finsemble: Desktop B] &harr; Chart App[Finsemble: Desktop B]

## Set Up

### On Desktop A


Download latest version of backplane:
- go to [backplane-releases](https://github.com/finos/backplane/releases?q=finos-fdc3-backplane).
- Download finos-fdc3-backplane-vx.x.xX.zip from assets.
- Unzip the zip folder.
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
  - Run 'Finos.Fdc3.Backplane.exe'.

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
- Set up the bridge service as described here: [readme](../samples/FinsembleIntegration/README.md)

Note: Both instance of finsemble on desktop A and B must have this configuration to communicate with backplane.

- Launch finsemble on both desktops.

 




