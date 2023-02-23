# Bully Algorithm

## Introduction
This project contains two programs, the `server` and `process`, that use TCP communication to simulate the Bully algorithm without using Threading.

## Running the Project

In order to run this project, you must first start the server application. Then, you can run as many instances of the process application as you like, to create a network of communicating processes.

Here are the steps to run the project:

1. Start the `server` application.
2. Run one or more instances of the `process` application.
\
By running these applications, you will be simulating the Bully algorithm and seeing the communication between the server and process instances.

## Instructions you should know

Once you have run the server application, you can run as many process applications as you want to simulate the bully algorithm. These process applications will communicate with each other and the server via TCP communication.

When a process application connects to the server, you will be presented with three options:

1. Terminate Coordinator: If you press 1, the process will terminate the coordinator.

2. Listen to Server: If you press 2, the process will listen to the server and update itself with the rest of the processes. Please note that once you press 2, the process will continue to listen to the server until it is closed. You can terminate the coordinator from another process later.
3. If you would like to close a process, press 0.
#### Please note that it is recommended to press 2 in order to listen to the server and keep the processes updated with each other. Terminating the coordinator can only be done through one of the connected processes. Additionally, you can create new processes and terminate the coordinator from one of those as well.

In future updates, there will also be a fourth option available in the coordinator process, which will allow you to perform various tasks with the rest of the processes. 

It's worth mentioning that every action taken by each process will be logged in the server console, although the format of the log might not be the most organized at the moment. However, the program will be improved and refined in future updates to provide a better user experience.

#### Additionally, it is important to note that the server is designed to detect duplicate process IDs. If the server detects a process with the same ID as itself, it will close the new process instantly. The server will also detect if any existing process has been closed and take necessary actions accordingly, but this detection will occur when a new process connects to the server. This ensures that the communication between the server and the processes remains stable and efficient.

