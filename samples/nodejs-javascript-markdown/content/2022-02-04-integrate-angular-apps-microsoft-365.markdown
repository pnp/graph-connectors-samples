---
layout: post
title: "#60 Integrate Angular apps with Microsoft 365"
slug: integrate-angular-apps-microsoft-365
excerpt: Have you built a Teams Tab with SSO in Angular? I'd love to learn more about it!
image: /assets/images/2022/01/notes.jpg
image_webp: /assets/images/2022/01/notes.webp
date: 2022-02-04 10:01:04
tags:
  - microsoft-365-development
  - newsletter
  - microsoft-365
  - angular
  - microsoft-teams
featured: false
hidden: true
---

Recently, I've been working on integrating a fictitious line of business app built using Angular with Microsoft 365. As usual, the [first step is integrating with Azure AD](https://blog.mastykarz.nl/bring-app-microsoft-365-3-steps/) to allow users to sign in with their work account. I must admit, it was cool to see how quickly you can integrate the Microsoft identity platform in an Angular app using MSAL Angular. [I got stuck](https://blog.mastykarz.nl/redirect-custom-login-page-securing-angular-app-msal/) trying to integrate the custom login page in the auth flow but with some help found a solution.

Next, I'm going to connect the app to Microsoft Graph. I'm planning to use MSAL interceptors for this. Stay tuned for updates. My end goal is to have an Angular app exposed in Microsoft Teams with Single Sign On (SSO) enabled.

_Have you built a Teams Tab with SSO in Angular? I'd love to learn more about it!_

What else is on my mind:

1. _Could we make it easier for developers to find the relevant resources for building apps on Microsoft 365 by providing them with quick reference cards?_ I'd appreciate it if you could have a look at the [sample reference cards](https://github.com/waldekmastykarz/m365-app-quickreference/blob/main/cards/teams-bot-sso-nodejs.md) I made and tell me if you'd find them useful and what other cards we could make.
1. _Imagine that you have a new colleague on your team and want to let them experience what's possible on Microsoft 365... What few apps would you let them build?_

I'd love to hear from you.

<small>Photo by <a href="https://unsplash.com/@glenncarstenspeters">Glenn Carstens-Peters</a> on Unsplash</small>
