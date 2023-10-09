---
layout: post
title: Work apps need work data
slug: work-apps-work-data
excerpt: When you use apps for work, they rarely show you all the data that you need to get the full context of your work. And that's a shame because that information is often readily available to you, and could be integrated into your app.
image: /assets/images/2022/08/banner.png
image_webp: /assets/images/2022/08/banner.webp
date: 2022-08-19 16:15:35
tags:
  - microsoft-365-development
  - microsoft-graph
  - microsoft-365
  - microsoft-teams
  - sharepoint
featured: false
hidden: false
---

When you use apps for work, they rarely show you all the data that you need to get the full context of your work. And that's a shame because that information is often readily available to you, and could be integrated into your app.

## It's all about the context

Say you're asked to build an app to manage projects for your organization. You build a web app, and host it in the cloud. Using the app you can store information about projects, customers, and project teams. Your organization uses another app for communication, and another for storing files, so rather than duplicate the functionality you point to existing apps for easy access.

Your app works, but is it convenient to use? Way too often you see your colleagues switch back and forth between your app, and other apps to get the full context of their work: information about the project, recent emails to the customer, project team communication, project files, and people who work on the project. What could you have done differently?

## Bring work data into your app

Say, your organization uses Microsoft 365. The project information is stored in your app. To communicate with the project team, they need to go to Teams. To get the project files, they need to go to SharePoint. Communication with the customer is in email in Outlook, and information about people is accessible via Teams, Outlook, or SharePoint, depending on where they are at the moment. Instead of switching between all these different apps, why not bring the relevant data to your app?

Work apps need work data. Even though your app has a specific purpose, like managing projects, people using it will need more information to get the full context of their work. Projects are commissioned by a customer and run by a project team. While you work on projects, you create files, and communicate about them. The same goes for managing orders, or any other business scenario. So why should that work context be fragmented? Why not offer your colleagues all the information they need right where they need it: **in your app**?

If you use Microsoft 365, information about people, and files is readily available to you, and accessible using [Microsoft Graph](https://developer.microsoft.com/graph/rest-api) - the API for Microsoft 365. Instead of requiring your users to jump between the different apps, you can use Graph to bring contextually relevant information to your app. You can bring in emails sent to, and received from the customer. You can bring in project files that the project team has recently worked on. You can show information about the project team members, their location, and time zones right in your app. You can have all this information, and more available in the context of your app so that your colleagues spend more time working, and less time switching contexts.

The great thing about using Microsoft Graph is that it allows you to tap into data, and insights that are stored in Microsoft 365. You get to benefit from all functionality from Microsoft 365, such as storage, access management, or disaster recovery, and at the same time, can integrate it in a contextually relevant way with your app.

Bringing work data to your app doesn't need to come at the cost of a lot of extra work. If you're on Microsoft 365, use Microsoft Graph, and tap into organizational data, and insights stored in Microsoft 365 that are relevant to your app. Check out what's possible using [Microsoft Graph](https://developer.microsoft.com/graph/rest-api).
