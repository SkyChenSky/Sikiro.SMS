# Sikiro.SMS
This is the SMS Service with .net core.

## Getting Started

### Run the SMS.API
The SMS.API will persistent data in the mongodb on first.If timing send,wait the job run.If send timely,send the message to mq
You can run the API Service on the windows and linux with dotnet core runtime.

### API Document
http://hostname/swagger

### Sikiro.SMS.Job
The job is Timing trigger.It will send the message to mq.
If run the windows Server，Please input command install service
```
dotnet Sikiro.SMS.Job.dll action:install
```

### Sikiro.SMS.Bus
The bus is send the sms with multiple operators.
If run the windows Server，Please input command install service
```
dotnet Sikiro.SMS.Bus.dll action:install
```

## End
If you have good suggestions, please feel free to mention to me.

