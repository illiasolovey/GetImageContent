# Get Image Content Lambda Function
Serverless Lambda solution to get content of any image via API request using AWS Rekognition service.

The following function is a part of [serverless-object-recognition](https://github.com/illiasolovey/serverless-object-recognition) project.

## Getting started from the command line

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
        "function-handler": "{GetImageContent::GetImageContent.Function::FunctionHandler}"
    }
    ```
    Function logic code is stored inside "GetImageContent::GetImageContent.Function::FunctionHandler", you can change its location by changing `"function-handler"` property
    - Configure S3 bucket in appsettings.json

    ```
    {
        "LambdaConfiguration": {
            "BucketName": "{S3 bucket name}"
        }
    }
    ```

1.  Deploy function to AWS Lambda

    ```
        dotnet lambda deploy-function
    ```
