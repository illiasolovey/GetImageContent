# Serverless Object Analysis
Serverless solution to analyse the content of an image via API request using AWS Rekognition service.

### Description
Object Analyser is an AWS Lambda function that utilizes AWS Rekognition service to analyze the contents within the given image. It is capable of automated image labeling and drawing bounding boxes around found objects.

The following function is a part of [serverless-object-recognition](https://github.com/illiasolovey/serverless-object-recognition) project.

### Requirements

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [AWS account](https://aws.amazon.com/)
- AWS CLI Tools installed on your machine.

### Getting started from the command line

1. Clone this repository:
    ```
    git clone https://github.com/illiasolovey/ObjectAnalysis.git
    ```

1. Install Amazon.Lambda.Tools Global Tools if not already installed.

    ```
    dotnet tool install -g Amazon.Lambda.Tools
    ```

1. Configure AWS profile, see: [AWS CLI Configuration](https://docs.aws.amazon.com/cli/latest/userguide/cli-chap-configure.html)

1. Add configuration files: `appsettings.json` and `aws-lambda-tools-default.json`
   
   - Inside aws-lambda-tools-default.json add the followings lines and configure your own lambda:
   
        ```
        {
            "profile": "{your configured AWS profile}",
            "region": "{service region}",
            "configuration": "Release",
            "function-architecture": "x86_64",
            "function-runtime": "dotnet6",
            "function-role": "{IAM role for your function}",
            "function-memory-size": 256,
            "function-timeout": 30,
            "function-handler": "ObjectAnalysis::ObjectAnalysis.Function::FunctionHandler"
        }
        ```
        Function logic code is stored inside "ObjectAnalysis::ObjectAnalysis.Function::FunctionHandler", you can change its location by changing `"function-handler"` property
    - Configure S3 bucket in appsettings.json

        ```
        {
            "StorageConfiguration": {
                "GetBucket": {Bucket to get desired object},
                "PutBucket": {Bucket to put updated object}
            }
        }
        ```

1.  Deploy function to AWS Lambda

    ```
        dotnet lambda deploy-function ObjectAnalysis
    ```
    This command will generate new Lambda function on your account, provided that specified name is not already in use.

1.  After successful deployment, access your AWS Lambda console and review your newly created function.