---
layout: post
title: Easily show files as cards using Microsoft Graph Toolkit and hTWOo
slug: easily-show-files-cards-microsoft-graph-toolkit-htwoo
excerpt: 'With a recent update to Microsoft Graph Toolkit, showing your files as cards became even easier. Check it out.'
image: /assets/images/2021/06/mgt-htwoo-banner.png
image_webp: /assets/images/2021/06/mgt-htwoo-banner.webp
date: '2022-01-05 09:28:31'
tags:
  - microsoft-graph-toolkit
  - htwoo
  - microsoft-graph
  - microsoft-365-development
  - microsoft-365
featured: false
hidden: false
---

With a recent update to Microsoft Graph Toolkit, showing your files as cards became even easier. Check it out.

## Show files as cards using Microsoft Graph Toolkit and hTWOo

Recently, I showed you [how you can use the Microsoft Graph Toolkit to show files stored in Microsoft 365 as cards](/show-files-cards-microsoft-graph-toolkit-htwoo/).

<p><picture>
  <source srcset="/assets/images/2021/06/mgt-htwoo-cards-thumb.webp" type="image/webp">
  <img src="/assets/images/2021/06/mgt-htwoo-cards-thumb.png" alt="List of files displayed as document cards in the browser. Cards are showing the file's thumbnail, location and the avatar of the person who last modified the file">
</picture></p>

Using just a few lines of code, we built a simple Single-Page App secured with Azure Active Directory, added auth, to let users sign in with their Microsoft 365 account and retrieve their files. We did it using [Microsoft Graph Toolkit](https://docs.microsoft.com/graph/toolkit/overview?WT.mc_id=m365-53122-wmastyka), which is _the easiest way to connect your app to Microsoft 365_. Its [authentication providers](https://docs.microsoft.com/graph/toolkit/overview?WT.mc_id=m365-53122-wmastyka) abstract away all of the auth code to a single line. And thanks to its [components](https://docs.microsoft.com/graph/toolkit/overview?WT.mc_id=m365-53122-wmastyka), you can easily show data from Microsoft 365 in your app.

To show files as cards, we used [hTWOo](https://lab.n8d.studio/htwoo/), which is a community-driven implementation of the Fluent UI design language.

> The complete sample app used in this article is [available on GitHub](https://github.com/waldekmastykarz/mgt-htwoo-files/).

## Inconvenient showing file details

Each card we show in our app contains information about the file, such as its thumbnail, location, name, author and picture, and when the file was last modified. Not all of this information is available to us readily, which is why in our original solution, we had to add quite some JavaScript code to load the necessary information from Microsoft Graph. And this is where things got complicated.

![Screenshot of JavaScript code calling Microsoft Graph](/assets/images/2022/01/javascript-code.png)

Microsoft Graph Toolkit gives you simplicity. Instead of building requests, parsing responses, and handling errors, you add ready-to-use components that do all of that for you and allow you to focus on building your app. But you don't benefit much if you still need to write requests because you can't get all the data that you need, do you?

## Easily show files as cards with Microsoft Graph Toolkit

Recently, Microsoft Graph Toolkit got updated, and if you were to build an app that shows files from Microsoft 365 as cards, you'd no longer need to write custom JavaScript. Here's the code you'd need instead:

```html
<html>
<head>
  <script src="https://unpkg.com/@microsoft/mgt@2/dist/bundle/mgt-loader.js"></script>
  <link rel="stylesheet" href="https://unpkg.com/@n8d/htwoo-core/dist/css/htwoo.min.css">
  <link rel="stylesheet" href="styles.css">
</head>
<body>
  <mgt-msal2-provider client-id="d43f076b-c6a6-4805-97be-f9ef969241c0" authority="https://login.microsoftonline.com/M365x61791022.onmicrosoft.com"></mgt-msal2-provider>
  <mgt-login></mgt-login>
  <mgt-file-list>
    <template>
      <h1>My files</h1>
      <div class="hoo-cardgrid">
        <div data-for="file in files" class="hoo-doccard" data-props="{% raw %}{{@click: openFile}}{% endraw %}">
          <div class="hoo-cardimage">
            <div data-if="file.folder"><img src="./folder.jpg" alt=""></div>
            <mgt-get data-if="!file.folder" resource="/drives/{% raw %}{{file.parentReference.driveId}}{% endraw %}/items/{% raw %}{{file.id}}{% endraw %}/thumbnails/0/c320x180_crop/content" type="image" cache-enabled="true">
              <template data-type="loading">
                <div class="hoo-ph-squared"></div>
              </template>
              <template data-type="error">
                <img src="./otter.jpg" alt="">
              </template>
              <template data-type="no-data">
                <img src="./otter.jpg" alt="">
              </template>
              <template data-type="default">
                <img src="{% raw %}{{image}}{% endraw %}" width="320" height="180" alt="">
              </template>
            </mgt-get>
          </div>
          <div class="hoo-cardlocation">
            <mgt-get resource="/drives/{% raw %}{{file.parentReference.driveId}}{% endraw %}" cache-enabled="true">
              <template data-type="loading">
                <div class="hoo-ph-row"></div>
              </template>
              <template data-type="error">
                <div class="hoo-ph-row"></div>
              </template>
              <template data-type="default">
                {% raw %}{{name}}{% endraw %}
              </template>
            </mgt-get>
          </div>
          <div class="hoo-cardtitle">{% raw %}{{file.name}}{% endraw %}</div>
          <div class="hoo-cardfooter">
            <div class="hoo-avatar">
              <mgt-get resource="/users/{% raw %}{{file.lastModifiedBy.user.id}}{% endraw %}/photo/$value" type="image" cache-enabled="true">
                <template data-type="loading">
                  <div class="hoo-ph-circle"></div>
                </template>
                <template data-type="no-data">
                  <div class="hoo-ph-circle hoo-avatar-img"></div>
                </template>
                <template data-type="default">
                  <img src="{% raw %}{{image}}{% endraw %}" alt="" class="hoo-avatar-img" loading="lazy">
                </template>
              </mgt-get>
            </div>
            <div class="hoo-cardfooter-data">
              <div class="hoo-cardfooter-name">{% raw %}{{file.lastModifiedBy.user.displayName}}{% endraw %}</div>
              <div class="hoo-cardfooter-modified">{% raw %}{{formatDate(file.lastModifiedDateTime)}}{% endraw %}</div>
            </div>
          </div>
        </div>
      </div>
      <button class="hoo-button-primary" data-props="{% raw %}{{@click: loadMore}}{% endraw %}">
        <div class="hoo-button-label">Load more</div>
      </button>
    </template>
  </mgt-file-list>
  <script src="script.js"></script>
</body>
</html>
```

Here's what's changed.

### Load document thumbnail using mgt-get

Originally, we'd use custom JavaScript to load the document thumbnail from Microsoft Graph and add it to the template. Starting from Microsoft Graph Toolkit v2.3.1, we can load images using the [Get component](https://docs.microsoft.com/graph/toolkit/components/get?WT.mc_id=m365-53122-wmastyka):

```html
<mgt-get data-if="!file.folder" resource="/drives/{% raw %}{{file.parentReference.driveId}}{% endraw %}/items/{% raw %}{{file.id}}{% endraw %}/thumbnails/0/c320x180_crop/content" type="image" cache-enabled="true">
  <template data-type="loading">
    <div class="hoo-ph-squared"></div>
  </template>
  <template data-type="error">
    <img src="./otter.jpg" alt="">
  </template>
  <template data-type="no-data">
    <img src="./otter.jpg" alt="">
  </template>
  <template data-type="default">
    <img src="{% raw %}{{image}}{% endraw %}" width="320" height="180" alt="">
  </template>
</mgt-get>
```

In the component, we're using 4 templates:

1. `loading`, which is rendered while the Get component is waiting on a response from Microsoft Graph
1. `error`, in case Microsoft Graph returned an error
1. `no-data`, this is a new template added to the Get component in Microsoft Graph Toolkit v2.3.1 which is rendered when the request returned no data. When you request an image, it's also rendered when the request returns a 404.
1. `default`, which is rendered when retrieving data succeeded

We can now use the Get component, not only to load the document's preview but also the avatar of the user who modified the file most recently.

### Load file location

Another request for which we originally wrote custom JavaScript, was to retrieve the location where the file is stored. In this version, we replaced that code with another instance of the Get component:

```html
<mgt-get resource="/drives/{% raw %}{{file.parentReference.driveId}}{% endraw %}" cache-enabled="true">
  <template data-type="loading">
    <div class="hoo-ph-row"></div>
  </template>
  <template data-type="error">
    <div class="hoo-ph-row"></div>
  </template>
  <template data-type="default">
    {% raw %}{{name}}{% endraw %}
  </template>
</mgt-get>
```

While the data is loading, we show a shimmer - an animated placeholder that communicates to users that content is loading. After we retrieved the file's location from Microsoft Graph, we show it instead.

With these modifications, the only piece of JavaScript that's left is:

```javascript
document.querySelector('mgt-file-list').templateContext = {
  formatDate: date => {
    const d = new Date(date);
    return d.toLocaleString();
  },
  openFile: (e, context, root) => {
    window.open(context.file.webUrl, '_blank');
  },
  loadMore: (e, context, root) => {
    root.parentNode.renderNextPage();
  }
};
```

We use these custom functions to format the file modified date, open the select file in a new tab, and load more files. Notice, that we no longer need to manually call Microsoft Graph to retrieve data, process responses, and handle errors!

## Summary

Microsoft Graph Toolkit is the easiest way to connect your app to Microsoft 365. Because it takes care of authentication and retrieving and showing the data in your app, you can focus on building your app. With a recent change to Microsoft Graph Toolkit, you can build even richer visualizations without having to write custom JavaScript and manually call Microsoft Graph.

If you're new to Microsoft Graph Toolkit, the best place to start is to follow the [Microsoft Graph Toolkit learning path](https://docs.microsoft.com/learn/paths/m365-msgraph-toolkit/?WT.mc_id=m365-53122-wmastyka) on MS Learn.
