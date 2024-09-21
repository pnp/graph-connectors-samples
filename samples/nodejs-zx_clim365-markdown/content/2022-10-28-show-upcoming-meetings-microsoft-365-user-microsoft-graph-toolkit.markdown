---
layout: post
title: Show upcoming meetings for a Microsoft 365 user with Microsoft Graph Toolkit
slug: show-upcoming-meetings-microsoft-365-user-microsoft-graph-toolkit
excerpt: Recently, I showed you how you can build in under 10 minutes a simple personal assistant that shows users meetings they have left for the day. Here's an even easier way to do it using the Microsoft Graph Toolkit.
image: /assets/images/2022/10/upcoming-meetings-graph-mgt-banner.png
image_webp: /assets/images/2022/10/upcoming-meetings-graph-mgt-banner.webp
date: 2022-10-28 11:04:59
tags:
  - javascript
  - microsoft-365-development
  - microsoft-graph
  - microsoft-graph-toolkit
featured: true
hidden: true
---

Recently, I showed you how you can build in under 10 minutes a simple personal assistant that shows users meetings they have left for the day. Here's an even easier way to do it using the Microsoft Graph Toolkit.

## Show upcoming meetings for a Microsoft 365 user

Showing upcoming meetings is a common scenario when integrating Microsoft 365 in work applications. Using [Microsoft Graph](https://developer.microsoft.com/graph?WT.mc_id=m365-80553-wmastyka), your app can connect to Microsoft 365 and access a user's calendar. By building a specific query, you can retrieve meetings between now and the end of the day.

Recently, I walked you step by step [how to complete this scenario](https://www.freecodecamp.org/news/how-to-show-upcoming-meetings-for-a-microsoft-365-user/) in under 10 minutes using the Microsoft Graph JavaScript SDK. But there's an even easier way to do it using the Microsoft Graph Toolkit.

## The easiest way to connect to Microsoft 365

[Microsoft Graph Toolkit](https://learn.microsoft.com/graph/toolkit/overview?WT.mc_id=m365-80553-wmastyka) (MGT) is a set of components and authentication providers connected to Microsoft 365. MGT takes away the complexity of implementing authentication, loading data from Microsoft Graph, and showing it in your app. And if anything goes wrong, it also properly handles exceptions. Microsoft Graph Toolkit's components are highly customizable so that you adjust them to your app.

### A quick comparison: signing in to your app

To understand the benefits of using Microsoft Graph Toolkit, let's have a look at an example: let users sign in to your app using their Microsoft 365 account.

Typically, you'd need code similar to the following:

```html
<html>
<head>
  <title>Upcoming meetings</title>
  <script src="https://alcdn.msauth.net/browser/2.28.3/js/msal-browser.min.js"></script>
</head>
<body>
  <h1>Upcoming meetings</h1>
  <div id="auth"></div>
  <script>
    (() => {
      const scopes = ['Calendars.Read'];
      const msalConfig = {
        auth: {
          clientId: '021aa7bb-9aaa-4185-92ad-c7b75a7fb9d2'
        }
      };
      const msalClient = new msal.PublicClientApplication(msalConfig);

      function render() {
        msalClient
          .handleRedirectPromise()
          .then(response => {
            const accounts = msalClient.getAllAccounts();

            if (accounts.length === 0) {
              document.querySelector('#auth').innerHTML = '<button>Login</button>';
              document.querySelector('#auth button').addEventListener('click', login);
            }
            else {
              document.querySelector('#auth').innerHTML = '<button>Logout</button>';
              document.querySelector('#auth button').addEventListener('click', logout);
            }
          });
      }

      function login(e) {
        e.preventDefault();
        msalClient.loginRedirect({
          scopes
        });
      }

      function logout(e) {
        e.preventDefault();
        msalClient.logoutRedirect();
      }

      render();
    })();
  </script>
</body>
</html>
```

In comparison, here's the same functionality built using Microsoft Graph Toolkit:

```html
<html>
<head>
  <title>Upcoming meetings</title>
  <script src="https://unpkg.com/@microsoft/mgt@2/dist/bundle/mgt-loader.js"></script>
</head>
<body>
  <h1>Upcoming meetings</h1>
  <mgt-msal2-provider client-id="021aa7bb-9aaa-4185-92ad-c7b75a7fb9d2" scopes="Calendars.Read"></mgt-msal2-provider>
  <mgt-login></mgt-login>
</body>
</html>
```

See the difference? And we didn't even get to the good part yet: calling Microsoft Graph!

> With Microsoft Graph Toolkit you can focus on building your app and solving problems for your customers. Microsoft Graph Toolkit takes care of the rest.

## Show upcoming meetings with Microsoft Graph Toolkit

To give you a more complete comparison, I [rebuilt the same scenario using Microsoft Graph Toolkit](https://github.com/waldekmastykarz/graph-mgt-upcomingmeetings).

> The best way to check out the app is to run the app locally by following the instructions in the README.

<p><picture>
  <source srcset="/assets/images/2022/10/upcoming-meetings-graph-mgt.webp" type="image/webp">
  <img src="/assets/images/2022/10/upcoming-meetings-graph-mgt.png" alt="Browser window with a web page showing upcoming meetings for a user">
</picture></p>

Because retrieving the data using MGT is so simple, I added some extra UI to differentiate between the different states of the app.

The [MGT Agenda component](https://learn.microsoft.com/graph/toolkit/components/agenda?WT.mc_id=m365-80553-wmastyka), which I use to show the upcoming meetings offers different [templates](https://learn.microsoft.com/graph/toolkit/components/agenda?WT.mc_id=m365-80553-wmastyka#templates) to customize the UI. When loading the data, I show a simple text message:

```html
<mgt-agenda>
  <template data-type="loading">
    <div class="loading">Loading...</div>
  </template>
  <!-- trimmed for brevity -->
</mgt-agenda>
```

When there's no data to show, I take into account the fact that there might be no data because the user hasn't signed in to the app yet, or that the user might have no upcoming meetings:

```html
<mgt-agenda>
  <template data-type="loading">
    <div class="loading">Loading...</div>
  </template>
  <template data-type="no-data">
    <div class="no-data" data-if="mgt.Providers.globalProvider.state === mgt.ProviderState.SignedIn">
      <!-- No upcoming meetings -->
    </div>
    <div class="no-data" data-else>
      <p>Sign in to view your upcoming meetings</p>
    </div>
  </template>
</mgt-agenda>
```

I use for this the [conditional rendering](https://learn.microsoft.com/graph/toolkit/customize-components/templates?WT.mc_id=m365-80553-wmastyka#conditional-rendering) capability in MGT.

To show upcoming meetings, for simplicity, I use the default template provided with the Agenda component.

![Animated gif showing signing in to the app with a Microsoft 365 account and viewing upcoming meetings](/assets/images/2022/10/upcoming-meetings-mgt.gif)

## Summary

Microsoft Graph Toolkit is a great way to build apps that connect to Microsoft 365. It takes away the complexity of connecting to Microsoft 365 and provides a set of components that you can use to build your app and bring in the data and insights from Microsoft 365. Because MGT components are highly customizable, you can seamlessly integrate them in your app.

[Run the sample app locally](https://github.com/waldekmastykarz/graph-mgt-upcomingmeetings) and see for yourself how easy it is to build apps that connect to Microsoft 365 using Microsoft Graph Toolkit.
