---
layout: post
title: The easiest way to test how your app handles API errors
slug: easiest-way-test-app-handles-api-errors
excerpt: Building robust apps that use APIs doesn't have to be hard. There are tools
  that you can use to see for yourself how your app will behave when calling
  APIs that it uses fails.
image: /assets/images/2023/03/banner.png
image_webp: /assets/images/2023/03/banner.webp
date: 2023-03-14 13:03:59
tags:
  - development
  - microsoft-graph
  - microsoft-graph-developer-proxy
  - testing
  - web-development
  - microsoft-365-development
featured: true
hidden: true
---

How would you test this code?

```js
async function getData() {
  try {
    const result = await fetch('https://contoso.com/api/data');
    if (result.ok) {
      return await result.json();
    }
  }
  catch (e) {
    // handle API errors here
  }
}
```

More specifically, **how would you verify that the code in the `catch` clause, responsible for handling errors is working properly**?

What if your API-handling code is more elaborate? What if you want to use rate limiting headers to back off and avoid getting throttled? And when you get throttled, you want to wait for the designated period and retry the request automatically? **How do you verify that your code works correctly?**

## Using APIs is hard

It's hard to imagine an app that's not connected to an API these days. We use APIs for everything: from authentication, to data storage, to integrating AI. APIs are literally everywhere. And yet, it's surprising to see how hard it still is to build robust apps that use APIs.

The APIs we use in our apps are built by different vendors. Each API works differently than the other. But that's fine. Using their docs, we can integrate these APIs in our apps just fine. We can even go as far as to automate testing different scenarios relevant to our app. But not all of them.

While it's easy to get APIs to work, it's hard to break them. Yes, you can easily send an invalid request and see it respond with an error, but that's hardly something you'd see in production, unless you missed something fundamental. What is **really hard** about working with APIs is simulating different error states, like rate limits, throttling or servers getting overloaded.

What makes it even harder, is that vendors who publish APIs, often spend more time in their docs communicating **how their APIs work** rather than **how their APIs behave when they fail**. "It's a matter of perception" - you could say. But if you've worked with APIs a bit, you know that API failure is not a matter of "if" but "when". And having proper docs that describe erroneous behavior makes all the difference.

## How developers handle API errors

Handling API errors is hard. Here's how developers typically deal with it.

### Wrap the whole request in a `try..catch`

Probably the most frequently used solution out there is to wrap the whole web request in a `try..catch` clause. If calling an API fails for **whatever** reason, the app catches the error and shows a generic error. I bet you've seen an error similar to **An error has occurred. Reload the page** more than once. This is exactly, because developers used a generic `try..catch` statement.

Using this approach gives you an easy way out. After all, as a developer, you don't have to care about the exact API error. Something went wrong in an API you don't own, so you can't do nothing about it, right? So you show a generic error to the user, asking them to retry. What else can you do, right?

### Use a dependency

There are dependencies that you can use in your project to help you handle API errors. Their functionality can be as simple as handling basic OData errors to handling throttling with exponential back offs. Thanks to these dependencies you can be smarter and automatically recover when possible without bother the user.

While it sounds great, what makes you so sure that the dependency will work properly with the APIs that you use in your app? Have you actually checked it? How would you though?

### Make the API fail

When developers want to see how their app behaves when it gets throttled, they sometimes overload the API on purpose. They'll keep calling it until they exceed the quota or force a server error due to too high resource consumption.

It's far from perfect for both the API vendor and the developers. API vendor loses precious resources sometimes even leading to service outage, and developers incur additional costs. But how else would you verify how the app actually behaves when it gets one of these elusive errors?

### Simulate API errors with mocks

More experienced developers, especially when they build a product, might go as far as mock APIs they use. Rather than calling the actual API, they simulate API responses in code what allows them to test different edge cases. This isn't trivial to do.

First, you need to have a thorough understanding of the API that you're using and its different behaviors. Can you get throttled? Does the API communicate rating limits? What other error states could the API communicate that you should consider? When do these errors occur and what is the shape of the error response that your app gets?

Once you know all of this, then you need to setup the necessary infrastructure to mock API calls and simulate the different errors. Simulating simple errors isn't hard. But it gets complicated when you get to simulating more complex behaviors like rate limiting that spans a series of requests.

And after you figured it all out, you also need to keep this in sync with the actual behaviors of the APIs that you use in your app.

### Using APIs properly is hard

There's more to using APIs than meets the eye. A 200 OK response is just the tip of the iceberg. If you want to build robust apps that use APIs, you need to properly handle possible errors, and you have to verify that you're doing it correctly. This has nothing to do with your app's functionality, but what's the alternative? Hoping for the best while knowing that it's just a matter of time until your app breaks and loses someone's work?

## See for yourself how your app will behave when APIs fail

The good news is, that there's a better way for you to see how your app will behave when the APIs that it uses fail. It allows you to see not just regular API failures, but rate limiting, throttling and any other type of error that an API might return. And you can do it without changing a single line of code in your app or standing up specific testing infrastructure and elaborate tests. Here's how.

[Microsoft Graph Developer Proxy](https://aka.ms/graph/proxy) - an open-source proxy from Microsoft, allows you to simulate API errors. When you start it on your developer machine, it intercepts web requests from your app and returns simulated errors. You can use the Developer Proxy for simulating simple errors and more complex behaviors like throttling on any API. And because it's a proxy, you can use it with any type of app, build on any technology stack.

Check out this single-page app connected to Microsoft Graph that shows you information about people. At first, it seems to work just fine:

<p><picture>
  <source srcset="/assets/images/2023/03/single-page-app-microsoft-graph-people.webp" type="image/webp">
  <img src="/assets/images/2023/03/single-page-app-microsoft-graph-people.png" alt="Web application showing photos and names of people">
</picture></p>

Let's see what happens, when one of the API calls would fail:

<p><picture>
  <source srcset="/assets/images/2023/03/web-app-microsoft-graph-developer-proxy-simulate-errors.webp" type="image/webp">
  <img src="/assets/images/2023/03/web-app-microsoft-graph-developer-proxy-simulate-errors.png" alt="Empty page in a web browser. Next to it, a terminal window with Microsoft Graph Developer Proxy simulating API errors">
</picture></p>

Because the developer didn't implement any error handling, as soon as an API call failed, the whole app failed leaving the user with an empty screen. And remember what we discussed: it's just a matter of time before calling an API fails, so you better be prepared for it.

See how quickly we were able to find a flaw in this app? Notice, that we didn't have to change anything about the app or stand up any testing infrastructure. All we did was start the Developer Proxy and run the app. That's it.

Using the Developer Proxy, you'll find all kinds of issues related to using APIs in your apps:

- How does your app behave when calling an API fails? Can it handle the error gracefully or will it crash losing the user's work?
- Is the error message that you show to the user relevant to the type of error that occurred?
- Do you even need to show an error message to the user or can you automatically recover, for example by retrying the failed request after some time?
- When loading data takes longer than expected, is it clear to the user that your app is still working?
- Could you cache data to use as fallback in case calling the API fails?

These are just some examples of insights about your app that you'll get when you test it with Microsoft Graph Developer Proxy.

Building robust apps that use APIs doesn't have to be hard. Microsoft Graph Developer Proxy allows you to see for yourself how your app will behave when calling APIs that it uses fails. With Microsoft Graph Developer Proxy, you can test **any kind of app, using any API**. Break your app on your box, handle errors gracefully and deploy your app to production with confidence. Your customers will thank you.

Get started at [https://aka.ms/graph/proxy](https://aka.ms/graph/proxy).
