# VideoPlatform
Video Platform Portal
This repository contains the implementation of the Video Platform Portal, which provides subscription-based access to video content. It is built using .NET Core, and integrates with various services such as authentication, subscriptions, and content delivery.

Key Features
Authentication: Users can register, log in, and authenticate using JWT tokens.

Subscription Service: Allows users to subscribe to different plans and manage their subscriptions.

Content Service: Provides video content to users based on their subscription plans.

Notification Service: Sends email notifications to users, integrated with ElasticEmail for email delivery.

Payment Integration: Uses Stripe for handling payments and subscriptions.

Architecture Overview
AuthService: Handles user authentication and registration, using JWT tokens for session management.

SubscriptionService: Manages subscription plans and keeps track of active subscriptions.

ContentService: Serves video content based on active subscriptions.

NotificationService: Sends email notifications using ElasticEmail for services such as account registration, subscription updates, and payment confirmations.

Tech Stack
.NET Core: For building the backend services (Auth, Subscription, Content, Notification).

Stripe: For handling payments and subscriptions.

ElasticEmail: For sending email notifications in the Notification Service.

RabbitMQ: For event-driven communication between services.

JWT: For user authentication and managing user sessions.

Setup and Configuration
Step 1: Clone the Repository
bash
Copy
Edit
git clone https://github.com/your-username/VideoPlatformPortal.git
cd VideoPlatformPortal
Step 2: Configure Services
Update appsettings.json with your configuration for external services such as:
Stripe: Add your Stripe keys under the Stripe section.

ElasticEmail: Add your ElasticEmail API key under the ElasticEmail section.

JWT Secret: Define a secret key for signing JWT tokens.

json
Copy
Edit
{
  "Stripe": {
    "SecretKey": "your-stripe-secret-key",
    "PublishableKey": "your-stripe-publishable-key",
    "WebHookUrl": "your-webhook-url"
  },
  "ElasticEmail": {
    "ApiKey": "your-elasticemail-api-key"
  },
  "AppSettings": {
    "JWTSecret": "your-jwt-secret-key"
  }
}
Step 3: Run the Application
Ensure all services (AuthService, SubscriptionService, ContentService, NotificationService) are running.

Start the frontend and backend applications using Visual Studio or by running dotnet run in the terminal.

Step 4: Access the Portal
Navigate to the appropriate URLs for the frontend and backend services.

Sign up for an account and subscribe to a plan to start accessing video content.

Services Flow
User Registration & Authentication:

Users can register, log in, and authenticate via JWT tokens.

The AuthService manages the authentication and JWT generation.

Subscription Process:

Users can select a subscription plan.

The SubscriptionService handles plan details, and once a payment is completed via Stripe, the user is subscribed.

A payment link is sent to the user's email through the NotificationService (ElasticEmail integration).

Video Content Delivery:

Based on the user's subscription, the ContentService serves video content.

Active subscriptions are tracked and updated in the ClaimsPrincipal upon successful payment.

Email Notifications:

The NotificationService sends various emails using ElasticEmail for:

Registration confirmation

Payment and subscription updates

General notifications.

Conclusion
This project integrates various services to provide a seamless subscription-based video platform experience, from user registration and authentication to payment processing and content delivery. The use of Stripe for payments, ElasticEmail for email notifications, and RabbitMQ for event-driven communication makes it a robust and scalable solution.
