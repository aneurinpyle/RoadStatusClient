# Road Status Client

A simple command line client to get the status of any major road in London.

## Before you start

To use this application, you will need the register for a developer API key from the [TfL API Developer Portal](https://api-portal.tfl.gov.uk/). You should also get the latest URL for the TfL Road API, incase it has been updated since the code was checked in.

## How to build the code

To build this code, you should do the following:

- Checkout the `main` branch from Git using GitHub Desktop or through Visual Studio 2019.
- Open `RoadStatusClient` folder using Visual Studio 2019.
- Double-click `RoadStatusClient.sln` to launch the solution.
- Once the solution has finished loading, right-click on the `RoadStatusClient` project and click `Publish`.
- Choose where to publish the output and click `Publish`.
- This will build and publish the code, creating an executable.

Alternatively, you can also build the application by clicking the `Build` tab then clicking `Build solution`. This will build the application only.

Before running the code, make sure that you open the `appsettings.json` file and add the values for the `TflApiKey` and `TflRoadApiUrl`, mentioned in the previous step.

## How to run the output

To run the code, open `cmd.exe` Command Prompt window and change the directory to the location of your published application, in the previous step, e.g. `cd C:\folder`. Enter the executable name and press `Enter`.

You will then see the application run, with a welcome message and instructions. Enter a road name, such as `A2`, or several roads, such as `A2,A20` using a comma-separated list. This will return the road or roads status information.

If you are not sure of any major roads, you can simply press `Enter`, with no road value entered, and the application will return all major roads, with their statuses.

If you enter a road that does not exist in the API, an error will appear, stating that the road entered is not a valid road.

If the API key or API url is incorrect or invalid, or if the API key used has made too many requests, a warning will appear, asking you to check the values entered in the `appsettings.json` against the [TfL API Developer Portal](https://api-portal.tfl.gov.uk/).

You can also run the application without publishing it using Visual Studio. When the solution file is open, right-click on `RoadStatusClient` project and click `Set as startup project` (It may already be set up as the startup project). You should then see the project name in the toolbar at the top, with a green play button. Click this button, and the application will build and run at the same time. This will also allow you to see the Exit codes once the application has run successfully or not.

## How to run the tests

To run the tests written, ensure that the solution is open within Visual Studio.

Then, click the `Test` tab and click `Test Explorer`, or press `Ctrl+E, T` as a keyboard shortcut. This will open a new window with the Test Explorer. To run all tests, click the `Run All Tests In View` button or press `Ctrl+R, V`.

The application will build and then run all tests, with the test results split into `Passed`, `Failed` and `Not run`.

## Assumptions

- Assumed that the user only wants to make one search, before the application closes. A loop could be added in future, with a `quit` command to break the loop and exit the application.
- Assumed that this application is only for use with the TfL Road API.
- The [TfL Roads API](https://api-portal.tfl.gov.uk/api-details#api=Road&operation=Road_GetByPathIds) documents several properties, but not all are returned. The `RoadStatusDto` contains all properties listed in this webpage to be mapped to, incase these properties are populated in future. They are likely populated using other `GET` requests for Roads.
- It also stipulates that `ids` request parameter is required, but if the user does not enter a value, all major roads' statuses are returned.

## Other relevant information

- Noticed that the main link to the [TfL APIs](https://api.tfl.gov.uk/) refers to using both an `app_id` and an `api_key`. Yet the [TfL API Developer Portal](https://api-portal.tfl.gov.uk/) mentions only using an `app_key`, which is effectively an API key. The documentation on the [TfL APIs](https://api.tfl.gov.uk/) website must be out of date.
- Noticed that the API returns `429` or `TooManyRequests` HTTP Status Code when the AppKey is invalid. Should return HTTP Status Code `401` or `Unauthorized`.
