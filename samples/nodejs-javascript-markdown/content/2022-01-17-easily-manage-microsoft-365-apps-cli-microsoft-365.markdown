---
layout: post
title: Easily manage your Microsoft 365 apps with CLI for Microsoft 365
slug: easily-manage-microsoft-365-apps-cli-microsoft-365
excerpt: 'All Microsoft 365 apps have one thing in common: they need an Azure AD app registration. And with CLI for Microsoft 365 you''ll be able to manage it without leaving your code editor.'
image: /assets/images/2022/01/banner-aad-app-reg.png
image_webp: /assets/images/2022/01/banner-aad-app-reg.webp
date: '2022-01-17 10:33:43'
tags:
  - azure
  - azure-active-directory
  - azure-ad
  - cli-microsoft-365
  - microsoft-365
  - microsoft-365-development
featured: false
hidden: false
---

All Microsoft 365 apps have one thing in common: they need an Azure AD app registration. And with CLI for Microsoft 365, you'll be able to manage it without leaving your code editor.

## Bring your apps to Microsoft 365

When you choose to [bring your app to Microsoft 365](/bring-app-microsoft-365-3-steps/), you get quite a few benefits. You can use Microsoft's identity platform to remove your custom user store and management and focus on the actual problems that your app is solving. You can also extend the information in your app with work data stored in Microsoft 365. Finally, you can also expose your app inside Microsoft 365 and bring it to where users are, and increase your engagement with them. The opportunity is endless and the best part is, that you can do it gradually.

## Register your app with the Microsoft cloud

When you bring your app to the Microsoft cloud, you first need to register it with the Microsoft cloud. You specify your name's name and type (for example if it's a mobile app or a web app). If it's a web app, you specify the URL where it's hosted. And if you integrate with Microsoft 365 data you specify which APIs your app needs to access. You do all this by creating an [application registration in Azure Active Directory](https://docs.microsoft.com/azure/active-directory/develop/active-directory-how-applications-are-added?WT.mc_id=m365-54657-wmastyka) (Azure AD). Azure AD is where each organization using Microsoft 365 stores information about their users, groups, and apps they use.

When building your app, you'll likely need to adjust the information in your Azure AD app registration. Also, as your app evolves, you might need to add additional API permissions or other configuration elements. Typically, you'd do it from the [Azure Portal](https://portal.azure.com/), but if you prefer to stay in your code editor I've got an alternative for you.

## Manage Azure AD application registrations with CLI for Microsoft 365

[CLI for Microsoft 365](https://aka.ms/cli-m365) is an open-source command-line tool that allows you to manage configuration settings of Microsoft 365 and automate tasks like moving files, creating teams or sites.

Using CLI for Microsoft 365 you can also manage Azure AD app registrations for your Microsoft 365 apps. Here's how.

### Create a new Azure AD app registration and store its information in your project

You start bringing your app to Microsoft 365 by creating an application registration in Azure Active Directory. Using CLI for Microsoft 365, you can do this using the `m365 aad app add` command. For example, if you're building a single-page application, you'd execute:

```sh
m365 aad app add --name 'My single-page app' --platform spa --redirectUris 'https://myspa.azurewebsites.net,http://localhost'
```

With this one command, CLI for Microsoft 365 will create a new Azure AD application registration and configure its authentication mode to a single-page application with the specified two redirect URLs.

> There are many settings that you can configure for Azure AD app registrations, so be sure to check the [documentation for the `m365 aad app add` command](https://pnp.github.io/cli-microsoft365/cmd/aad/app/app-add/) for more examples.

This one-liner is great to share with your dev team so that each developer can create their own app registration that they can manage as they work on the app. If your app's configuration is complex, you can also choose to export the existing manifest and create a new Azure AD app registration from it! But there's more.

### Store reference to the Azure AD app registration in your Microsoft 365 app

As you work with Microsoft 365 apps, you'll be creating quite a few application registrations in Azure AD. Over time, it might be hard for you to keep track of which one is which and where you need to apply changes.

To help you, CLI for Microsoft 365 offers you two things. First, when creating an Azure AD app registration for your Microsoft 365 app, store a reference to it. You do this, by extending the `m365 aad app add` command with the `--save` flag:

```sh
m365 aad app add --name 'My single-page app' --platform spa --redirectUris 'https://myspa.azurewebsites.net,http://localhost' --save
```

When you use the `--save` flag, CLI for Microsoft 365 will create the `.m365rc.json` file in the current working directory and write to it the ID and name of the newly created Azure AD app registration. If the file exists already, CLI for Microsoft 365 will add the new information to it. That way you can track which Azure AD app registration belongs to your project without having to manually locate them in the Azure Portal! And if you're building complex solutions with multiple Azure AD apps, you can keep track of all of them in one place too!

### Check API permissions granted to Azure AD app registrations

When developing Microsoft 365 apps, one of the most frequent issues developers experience, is insufficient permissions to access APIs. Typically, to check which API permissions your app has granted, you'd navigate to Azure Active Directory in the Azure portal, locate the application registration that belongs to your project and check the API permissions.

When using CLI for Microsoft 365, you can accomplish all of that with one command:

```sh
m365 app permission list
```

This command will take the information about Azure AD application registration stored in your project and retrieve information about API permissions granted for it. If you use a code editor with a terminal, like [Visual Studio Code](https://code.visualstudio.com/), you can run this command without ever leaving your code.

## What's next

We've added support for managing Microsoft 365 apps in CLI for Microsoft 365 just recently and would love to hear from you what you think of it. To make it even more helpful, we're thinking about some additional features:

- store information about existing Azure AD app registration in the project [#2939](https://github.com/pnp/cli-microsoft365/issues/2939)
- add API permissions to Azure AD app registrations [#2813](https://github.com/pnp/cli-microsoft365/issues/2813)
- open the Azure portal on the page to manage your Azure AD app registration [#2940](https://github.com/pnp/cli-microsoft365/issues/2940)

Over to you. Thinking about the last time you were working on a Microsoft 365 app: would this help? What other things do you need to do that we could simplify?
