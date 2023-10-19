---
layout: post
title: "#66 Will your app fail when throttled?"
slug: app-fail-throttled
excerpt: "How will your app react when throttled: will it fail gracefully or break with exception? Are you sure?"
image: /assets/images/2022/12/banner-proxy.png
image_webp: /assets/images/2022/12/banner-proxy.webp
date: 2022-12-09 11:03:00
tags:
  - microsoft-365-development
  - microsoft-graph
  - microsoft-graph-developer-proxy
  - newsletter
featured: false
hidden: true
---

On a tenant of one, your app never fails. Calling Microsoft Graph and other APIs on Microsoft 365 _just works_. But when it gets used at scale in production, only then you get to see if you built it correctly.

When you build a great application, and it gets widely adopted, it's used a lot. Depending on the size of your organization, your app might be used by hundreds or thousands of simultaneous users, issuing even more requests to Microsoft Graph and other APIs. Microsoft Graph can handle a lot, but even it has its limits, and when you reach them, by calling Graph APIs too often, instead of a response with data, you'll get errors like `429 Too Many Requests`. **You're throttled**.

**Throttling is temporary.** It's a mechanism to help servers resume regular service operation. By instructing clients to back off for a bit, servers hosting cloud APIs get a chance to get back to normal operation. That's the good news. The bad news is, that, **unless you use an SDK that does it for you, you need to handle throttling yourself.**

The problem with throttling is that it's elusive. It occurs only in certain conditions. This makes it really hard to test how your app will react when throttled: **will it back off and wait as instructed, or will it break with an exception**?

This is exactly why we built the [Microsoft Graph Developer Proxy](https://github.com/microsoftgraph/msgraph-developer-proxy): a tool to help you **test how your apps respond to API errors**. Seeing is believing for a reason. With the Graph Developer Proxy, you no longer need to hope for the best. You can verify, even on your tenant of one, how your app will react when suddenly Graph APIs will return errors.

🙋‍♂️ [Download the Graph Developer Proxy](https://github.com/microsoftgraph/msgraph-developer-proxy), test your apps, and I'm looking forward to hearing what you learned.

_PS. You can use Microsoft Graph Developer Proxy with any type of app: client-side apps running in the browser or back-end services running on a server._
