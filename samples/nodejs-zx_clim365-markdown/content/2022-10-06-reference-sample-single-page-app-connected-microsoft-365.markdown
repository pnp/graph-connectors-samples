---
layout: post
title: "Reference sample: Single Page App connected to Microsoft 365"
slug: reference-sample-single-page-app-connected-microsoft-365
excerpt: When building apps connected to Microsoft 365, before you can bring in data
  from Microsoft 365, you need to setup authentication. Here's a reference
  sample of a Single Page App that shows how to do that.
image: /assets/images/2022/10/banner.png
image_webp: /assets/images/2022/10/banner.webp
date: 2022-10-06 09:19:59
tags:
  - javascript
  - microsoft-365-development
  - microsoft-graph
featured: false
hidden: false
---

When building apps connected to Microsoft 365, before you can bring in data from Microsoft 365, you need to set up authentication. Here's a reference sample of a Single Page App that shows how to do that.

## Work apps need work data

Microsoft 365 is a popular set of applications that organizations across the world use to facilitate collaboration and communications. It's also a platform that you can use to build apps for work and streamline how people work together.

<p><picture>
  <source srcset="/assets/images/2021/02/microsoft-365-types-apps.webp" type="image/webp">
  <img src="/assets/images/2021/02/microsoft-365-types-apps.png" alt="Types of apps that you can build on Microsoft 365 grouped into extensions and custom apps">
</picture></p>

These apps can show up inside Microsoft 365, bringing information from line of business systems into the apps that people use every day. They can also be standalone web-, desktop-, and mobile apps that combine data and insights from Microsoft 365 with data from other systems.

No matter which type of app you choose to build, you need to start with connecting to Microsoft 365, which means you need to set up authentication.

## Reference sample: Single Page App connected to Microsoft 365

Because you can build many types of apps connected to Microsoft 365, there are many ways to set up authentication. And if you're just starting building apps for Microsoft 365, it might not be quite clear for you what it is exactly you need and how to combine the different parts together.

To help you get started, I built a [sample Single Page Application](https://github.com/waldekmastykarz/js-graph-101) that shows you how to:

- setup authentication with Microsoft 365 in a Single Page Application
- let users sign in and -out with their Microsoft 365 accounts to your app
- retrieve data from Microsoft 365 using Microsoft Graph
- register your app with the Microsoft identity platform

The sample app is built using plain JavaScript, which lets you reuse the code in any JavaScript framework. In the repo, you'll find the app built in two ways, using [immediately invoked function expressions (IIFE)](https://github.com/waldekmastykarz/js-graph-101/blob/6532d217f3e33304d2e4de18f1d482987818cfd9/index.html) and using [ES modules (ESM)](https://github.com/waldekmastykarz/js-graph-101/blob/6532d217f3e33304d2e4de18f1d482987818cfd9/index_esm.html). It's largely a matter of preference which one you choose to use, but I wanted to show you both approaches.

If you want to see how I built the app step by step, check out my [recent article on freeCodeCamp](https://www.freecodecamp.org/news/build-microsoft-365-application-in-10-minutes/).
