# ASP.NET Core MVC + Google Vertex AI Integration

## Overview

This ASP.NET Core MVC application is built to integrate with Google Cloud Vertex AI and interact with a Vertex AI Agent. On the client side, it uses **vanilla JavaScript** within the MVC Core views to send user inputs. At the backend, an **API controller** is implemented which communicates with the Vertex AI Agent using the `Google.Cloud.AIPlatform.V1` SDK. The application also makes use of **Server-Sent Events (SSE)** to stream the AI responses in real time back to the client, providing a smooth and interactive experience. This setup demonstrates how a traditional ASP.NET Core MVC architecture can be effectively combined with modern AI capabilities using Google Cloud services, while keeping the frontend lightweight and responsive.

---

## Features

- ASP.NET Core MVC architecture  
- Integration with Google Vertex AI Agent  
- Real-time streaming responses using Server-Sent Events (SSE)  
- Clean frontend with vanilla JavaScript  
- API communication via `Google.Cloud.AIPlatform.V1` SDK  

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)  
- [Google Cloud account](https://cloud.google.com/)  
- Vertex AI Agent configured in your Google Cloud project  
- Vertex AI API enabled  
- Service account with appropriate IAM roles (e.g., Vertex AI User)  
- JSON key file for the service account  
- Configure the environment variable GOOGLE_APPLICATION_CREDENTIALS by setting it to the file path of the service account’s JSON key

## Folder Structure

- /Controllers

- HomeController.cs
- API/VertexApiController.cs // Handles interaction with Vertex AI

- /Views
- /Home
- Index.cshtml // Contains frontend UI and vanilla JS logic
- /wwwroot
- /css
- /js
- Program.cs
- Startup.cs
- appsettings.json

## Technologies Used

- ASP.NET Core MVC  
- Google Cloud Vertex AI  
- `Google.Cloud.AIPlatform.V1` SDK  
- Vanilla JavaScript  
- Server-Sent Events (SSE)  
- REST APIs  
