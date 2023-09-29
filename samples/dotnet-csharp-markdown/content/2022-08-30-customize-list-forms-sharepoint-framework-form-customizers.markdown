---
layout: post
title: Customize list forms with SharePoint Framework form customizers
slug: customize-list-forms-sharepoint-framework-form-customizers
excerpt: Last week at the Viva Connections & SharePoint Framework Community Call I had
  the pleasure of presenting how to use the SharePoint Framework to customize
  list forms. Here's a quick overview of the topics we covered.
image: /assets/images/2022/08/banner-spfx-form-customizer.png
image_webp: /assets/images/2022/08/banner-spfx-form-customizer.webp
date: 2022-08-30 08:30:26
tags:
  - form-customizers
  - sharepoint-framework
  - microsoft-graph
  - microsoft-365-pnp
  - microsoft-365-development
  - microsoft-365
featured: false
hidden: false
---

Last week at the Viva Connections & SharePoint Framework Community Call I had the pleasure of presenting how to use the [SharePoint Framework](https://aka.ms/spfx-docs) to customize list forms. Here's a quick overview of the topics we covered.

## Form customizers - a new type of SPFx extension

In SharePoint Framework (SPFx) v1.15, which was released in July '22, Microsoft introduced [form customizers](https://docs.microsoft.com/sharepoint/dev/spfx/extensions/get-started/building-form-customizer) - a new type of extension that allows you to extend list forms. Using form customizers, you can build a custom experience for New, Edit, and View forms of a list.

## Anatomy of an SPFx form customizer

Form customizers are in essence SPFx extensions: they have the `onInit` method, to initialize data, and the `render` method to build the UI. Additionally, form customizers have two methods that you need to call, to notify the SPFx page runtime of how the user interacts with the form: `this.formSaved()`, when the data has been changed and saved, and `this.formClosed()` when the user closed the form without modifying the data.

```typescript
export default class HelloWorldFormCustomizer
  extends BaseFormCustomizer<IHelloWorldFormCustomizerProperties> {

  public onInit(): Promise<void> {
    // Add your custom initialization to this method. The framework will wait
    // for the returned promise to resolve before rendering the form.
    Log.info(LOG_SOURCE, 'Activated HelloWorldFormCustomizer with properties:');
    Log.info(LOG_SOURCE, JSON.stringify(this.properties, undefined, 2));
    return Promise.resolve();
  }

  public render(): void {
    // Use this method to perform your custom rendering.
    this.domElement.innerHTML = `<div class="${ styles.helloWorld }"></div>`;
  }

  public onDispose(): void {
    // This method should be used to free any resources that were allocated during rendering.
    super.onDispose();
  }

  private _onSave = (): void => {
    // You MUST call this.formSaved() after you save the form.
    this.formSaved();
  }

  private _onClose = (): void => {
    // You MUST call this.formClosed() after you close the form.
    this.formClosed();
  }
}
```

To allow you to optimize the performance of your solution, form customizers don't load the data for the underlying list item. Instead, you need to load the item yourself and choose which information you want to load in the `onInit` method. When building an Edit form, you also need to load information about the ETag, to ensure that you're not overwriting data that has been changed by another user.

```typescript
export default class BasicsFormCustomizer extends BaseFormCustomizer<IBasicsFormCustomizerProperties> {
  // item to show in the form; use with edit and view form
  private _item: {
    Title?: string;
  };
  // item's etag to ensure the integrity of the update; used with the edit form
  private _etag?: string;

  public onInit(): Promise<void> {
    if (this.displayMode === FormDisplayMode.New) {
      // we're creating a new item so nothing to load
      return Promise.resolve();
    }

    // load item to display on the form
    return this.context.spHttpClient
      .get(this.context.pageContext.web.absoluteUrl + `/_api/web/lists/getbytitle('${this.context.list.title}')/items(${this.context.itemId})`, SPHttpClient.configurations.v1, {
        headers: {
          accept: 'application/json;odata.metadata=none'
        }
      })
      .then(res => {
        if (res.ok) {
          // store etag in case we'll need to update the item
          this._etag = res.headers.get('ETag');
          return res.json();
        }
        else {
          return Promise.reject(res.statusText);
        }
      })
      .then(item => {
        this._item = item;
        return Promise.resolve();
      });
  }

  // trimmed for brevity
}
```

## Previewing a Form Customizer

When you scaffold a Form Customizer, the SharePoint Framework Yeoman generator adds 3 configurations to your `serve.json` file, that allow you to quickly preview your Form Customizer with the New, Edit, and View form.

To preview your Form Customizer, run in the command line:

```sh
gulp serve --config helloWorld_NewForm
```

where `helloWorld` is the alias of your Form Customizer and `NewForm` is the form display mode that you want to preview. For the list of available serve configurations, check the `config/serve.json` file in your project.

## Separate- vs. one form customizer

When building form customizers, you can choose to build a separate Form Customizer for each form display mode, or you can build one Form Customizer that supports all display modes. You can also choose to override only a specific display mode and use the default behaviors for other display modes. This gives you flexibility about how you organize your code and what kind of experience you want to build for your users.

## Deploy your Form Customizer

After you built your Form Customizer, you need to deploy it by associating it with a content type. You can associate it with an instance of a content type in a specific list or a site content type. If you want the Form Customizer to be used with all instances of the content type in your tenant, associate it with the content type in the content type hub site from which it will be synced to other sites in your tenant.

To register a Form Customizer with a content type, set the ID of the Form Customizer component (the value of the `id` property from the Form Customizer's manifest) in the `ContentType.DisplayFormClientSideComponentId` property of the content type. If your Form Customizer is configurable, you can pass its configuration as a JSON string in the `ContentType.DisplayFormClientSideComponentProperties` property. This set of properties is also available for the New- (`NewFormClientSideComponentId`, `NewFormClientSideComponentProperties`) and Edit (`EditFormClientSideComponentId`, `EditFormClientSideComponentProperties`) form.

You can set these properties declaratively using the Feature framework in the SPFx solution, but the preferable way is to do it programmatically using the REST or CSOM APIs, through remote provisioning solutions.

## What can you build with form customizers?

Form customizers are a great way to extend the user experience on top of lists. By combining them with list formatting, you can build applications that are hosted on SharePoint and don't require any additional resources.

Because you own the full canvas of the list form, you've got a lot of flexibility when it comes to rendering the form. And since form customizers are powered by SPFx, you've got access to Microsoft Graph and line of business APIs to bring in additional data.

<p><picture>
  <source srcset="/assets/images/2022/08/spfx-form-customizer.webp" type="image/webp">
  <img src="/assets/images/2022/08/spfx-form-customizer.png" alt="Sample application for approving travel requests, showing trip information and a map with the departure and destination, built using a SharePoint Framework Form Customizer">
</picture></p>

Here's the full recording of the community call starting with the form customizers section.

<iframe width="560" height="315" src="https://www.youtube.com/embed/nHM2rXHs7nY?start=2259" title="YouTube video player" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>

Once again, thanks to everyone for joining us, and don't hesitate to reach out if you have any questions.

_What are you going to build with form customizers?_
