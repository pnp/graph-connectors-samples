---
layout: post
title: '#55 2 rules to play by in SPFx'
slug: '2-rules-play-spfx'
excerpt: '2 SharePoint Framework rules of thumb that you must keep in mind to play nicely and don''t break the page.'
image: /assets/images/2021/02/sample-sharepoint-portal.png
image_webp: /assets/images/2021/02/sample-sharepoint-portal.webp
date: '2021-07-30 09:07:29'
tags:
  - newsletter
  - sharepoint-framework
  - sharepoint-development
  - teams-development
  - microsoft-365-development
  - microsoft-teams
  - viva-connections
  - microsoft-365
featured: false
hidden: true
---

[SharePoint Framework](https://docs.microsoft.com/sharepoint/dev/spfx/sharepoint-framework-overview?WT.mc_id=m365-36594-wmastyka), also known as SPFx, is a versatile framework for building apps on Microsoft 365. You can use it to extend SharePoint with [web parts](https://docs.microsoft.com/sharepoint/dev/spfx/web-parts/overview-client-side-web-parts?WT.mc_id=m365-36594-wmastyka) and [extensions](https://docs.microsoft.com/sharepoint/dev/spfx/extensions/overview-extensions?WT.mc_id=m365-36594-wmastyka), build [tabs](https://docs.microsoft.com/sharepoint/dev/spfx/build-for-teams-expose-webparts-teams?WT.mc_id=m365-36594-wmastyka) and [messaging extensions](https://docs.microsoft.com/sharepoint/dev/spfx/build-for-teams-expose-webparts-teams?WT.mc_id=m365-36594-wmastyka#expose-web-part-as-microsoft-teams-messaging-extension) for Teams and extend Viva Connections with [Adaptive Card Extensions](https://docs.microsoft.com/sharepoint/dev/spfx/viva/overview-viva-connections?WT.mc_id=m365-36594-wmastyka). What's cool about SPFx is that **it's just web dev**. Using React, or any other web framework, you're building a web app, that will be exposed in Microsoft 365.

If you're building a web part or an extension using SPFx, your app will share the page with other apps. And there are two rules of thumb that you must keep in mind to play nicely and don't break the page.

## 1. Don't use IDs

IDs are meant to uniquely identify DOM elements on pages. But if you're building web parts or extensions, it's possible that the user will add multiple instances of it. If you assume that there is only one instance of your app on the page, where in reality there are multiple, it could break the page. So to avoid this, you should **never use IDs in your SharePoint Framework solutions**. Instead, use CSS classes to identify your elements.

## 2. Query the DOM within your assigned DOM element

When working with the DOM, whether to manipulate what's visible or to handle events, you might need to get a reference to a specific element. Typically, you'll see it done using `document.querySelector` or `document.querySelectorAll`. In the context of SPFx, this is a bad approach, because it could return **elements that you don't own**!

Instead, you should **always query within the DOM element assigned to your web part or extension**, like `this.domElement.querySelector` or `this.myPlaceholder.domElement.querySelector`. That way, you'll be sure that the element you get belongs to the current instance of your web part or extension. Because the DOM query is scoped to your DOM element, you can be even sure that even if you'd want to query the element just using its CSS class name, you won't get back an element with a colliding CSS class name but from a different app on the same page. Oh, and your code will be faster too because instead of traversing the whole page's DOM, it will look for the element just within your root node.

**When building SPFx solutions, keep in mind these two rules of thumb and you will avoid a lot of hard to debug issues!**
