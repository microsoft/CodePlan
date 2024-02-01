#  Question and Answer API
<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_apis/build/status/Endpoint%20Assessment%20Organisation/das-qna-api?branchName=master)](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_build/latest?definitionId=1654&branchName=master)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SkillsFundingAgency_das-qna-api&metric=alert_status)](https://sonarcloud.io/project/overview?id=SkillsFundingAgency_das-qna-api)
[![Confluence Project](https://img.shields.io/badge/Confluence-Project-blue)](https://skillsfundingagency.atlassian.net/wiki/spaces/NDL/pages/1686274228/QnA+API+-+Developer+Overview)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)

This service is a Web API service which allows question sets to be organised and presented and their answers collected by exposing HTTP REST end points.

Nuget packages that go with this solution include:
* A .Net Core client library for QnA API HTTP REST end points [![QnA API Client](https://buildstats.info/nuget/SFA.DAS.QnA.Api.Client)](https://www.nuget.org/packages/SFA.DAS.QnA.Api.Client)
* Common types to interact with the QnA Api [![QnA API Types](https://buildstats.info/nuget/SFA.DAS.QnA.Api.Types)](https://www.nuget.org/packages/SFA.DAS.QnA.Api.Types)
* Asp.Net Core Views using [GOV UK Design System](https://design-system.service.gov.uk/get-started/) [![QnA API Views](https://buildstats.info/nuget/SFA.DAS.QnA.Api.Views)](https://www.nuget.org/packages/SFA.DAS.QnA.Api.Views)

## Developer Setup
### Requirements

* Install [.NET Core 2.2 SDK](https://www.microsoft.com/net/download)
* Install [Visual Studio 2019](https://www.visualstudio.com/downloads/) with the following workloads:
	* ASP.NET and web development
* Install [SQL Server 2017 Developer Edition](https://go.microsoft.com/fwlink/?linkid=853016)
* Install [SQL Management Studio](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)
* Install [Azure Storage Emulator](https://go.microsoft.com/fwlink/?linkid=717179&clcid=0x409) (Make sure you are on at least v5.3)
* Install [Azure Storage Explorer](http://storageexplorer.com/) 
* Administrator Access

### Environment Setup

* Clone this repository
* Open Visual Studio as an administrator
* **Database** - Build the solution `SFA.DAS.QnA.sln`. Either use Visual Studio's `Publish Database` tool to publish the database project `SFA.DAS.QnA.Database` to the database name `{{database name}}` on `{{local instance name}}`.
	* To include the latest question sets when publishing a database to your local SQL Server, you need to ensure that the `ProjectPath` variable contains the full path in the format  `{{drive}}:\{{project-folders}}\das-qna-api\src\SFA.DAS.QnA.Database\` 
	eg. `C:\Source\Repos\SFA\das-qna-api\src\SFA.DAS.QnA.Database\`
	**Note**: The required trailing backslash on the path in the example above.
* **or** create a database manually named `{{database name}}` on `{{local instance name}}` and run each of the `.sql` scripts in the `SFA.DAS.QnA.Database` project.
* **json file** - Get the `das-qna-api` configuration json file from [das-employer-config](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-qna-api/SFA.DAS.QnA.Api.json) which is a non-public repository.
* **Azure Table Storage Config** - Add the following to your Azure Table Storage Config.

PartitionKey: LOCAL

RowKey: SFA.DAS.QnA.Api_1.0 

Data: 
```
{{The contents of the local config json file}}
```

* Update Configuration `SFA.DAS.QnA.API_1.0` with data `{ "SqlConnectionstring":"Server={{local instance name}};Initial Catalog={{database name}};Trusted_Connection=True;" }`

### Running

JSON configuration was created to work with dotnet run.
* Navigate to `src/SFA.DAS.QnA.API/`
* run `dotnet restore`
* run `dotnet run`

or

* Set `SFA.DAS.QnA.API` as the startup project
* Running the solution will launch the API in your browser.
	
* **NB** - To run a local copy you may also need:
	* To create a JSON structure required to author updates and create new question sets: [Config Tool](https://github.com/SkillsFundingAgency/das-qna-config)
	* To view how the question sets will be presented when integrated into a client application using [GOV UK Design System](https://design-system.service.gov.uk/get-started/) and [Config Preview](https://github.com/SkillsFundingAgency/das-qna-config-preview)

#### `SFA.DAS.QnA.Application`

This project contains all of the application logic to handle requests.

#### `SFA.DAS.QnA.Configuration`

This project enables functionality to store and read configuration from Microsoft Azure Storage.

* AzureActiveDirectoryConfiguration
	*  Azure Managed Identity authentication
	
* FileStorageConfig
	* Information relating to storage of files
	
* QnAConfig
	* Connection string for the QnA Database

#### `SFA.DAS.QnA.Database`
This is the database project containing setup in order to the create the QnA Database.

* projects
	* A subfolder should be created per project.
	
* `projects/{subfolder}/project.json`
	* Contains information on how to setup the Workflow information.
	
* `projects/{subfolder}/ApplicationDataSchema.json`
	* Contains JSON schema to validate ApplicationData.
	
* `projects/{subfolder}/sections`
	* Holds QnAData for each WorkflowSection.
	
#### QnA Structure

A Workflow is made up of multiple sequences. Each sequence may have multiple sections.

The QnAData within each section defines the flow and logic. There are multiple pages consisting of questions and relevant answers. Depending on the answers provided, it will decide which pages are active.

##### Question

Questions should have a unique Id, an optional `QuestionTag` and a particular input type. If the input has different options to select from, you may want to include `FutherQuestions` to allow a related/follow up question (i.e. Yes selected, so now need to provide more details).

##### Question Type

Most types are self explanatory and in most cases have they will have built-in validators.

* `TabularData` is a JSON structure that represents a table storage format (i.e. header and rows).
* `FileUpload` is for uploading files. Note that this type should POST the Answer/Files to the File Upload endpoint and not the Save Answers end point.
* `ComplexRadio` & `ComplexCheckboxList` enable the use of `FutherQuestions` based on the option being selected.

##### Next Conditions

These are the primary mechanism to dictate logic flow within a section. Should a `QuestionId` or `QuestionTag` match the `Next` condition then that specified page will be made active. Any conditions that do not match will make that specified page inactive.

##### Next Actions

`NextPage` - Go to the next page

Any other value can be specified. This is intended to allow the calling application to decide the logic. Other actions could be `ReturnToSection` or `TaskList`.

##### NotRequired Conditions

There are situations where `Next` Conditions cannot control the page flow (i.e. determining the entry point to the first page within a section based on a particular value).

`NotRequiredConditions` is a way for `QnA Api` to remove pages from the response payload back to the user.

### Tests

This codebase includes unit tests.

#### Unit Tests
There are two unit test projects, each named after the project that their tests cover. These tests use Moq, FluentAssertions, NewtonSoft, MediatR, and NUnit.
* `SFA.DAS.QnA.Api.UnitTests`
* `SFA.DAS.QnA.Application.UnitTests`
