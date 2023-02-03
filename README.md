# Apache Kafka for .Net Developers

This repository is for the **Apache Kafka for .Net Developers** course provided by Confluent Developer.

For full details of the course and exercise instructions, please visit [developer.confluent.io](https://developer.confluent.io/learn-kafka/apache-kafka-for-dotnet-developers/).

## Repo Structure

### exercises

This folder is where you will do your work. Inside the folder you will find an `exercise.sh` and `exercise.bat` script.

You can use this script to setup an exercise by running:

```bash
./exercise.sh stage <exerciseId>
```

or

```bash
exercise.bat stage <exerciseId>
```

You can solve the exercise automatically by running:

```bash
./exercise.sh solve <exerciseId>
```

or

```bash
exercise.bat solve <exerciseId>
```

You can also solve an individual file by running:

```bash
./exercise.sh solve <exerciseId> <filename>
```

or

```bash
exercise.bat solve <exerciseId> <filename>
```

### staging

This folder contains any file necessary to set up for the individual exercises. For the most part, you can probably ignore this folder.

### solutions

This folder contains the solution files for each exercise. You can use it for reference if needed, but we recommend you try and do each exercise on your own.