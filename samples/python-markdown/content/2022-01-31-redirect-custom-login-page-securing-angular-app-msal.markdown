---
layout: post
title: Redirect to a custom login page when securing your Angular app with MSAL
slug: redirect-custom-login-page-securing-angular-app-msal
excerpt: 'Using MSAL Angular and MsalGuard is the easiest way to secure your Angular app with the Microsoft Identity Platform. But if you want to use a custom login page rather than redirecting users directly to Azure Active Directory, there''s one thing you need to consider.'
image: /assets/images/2022/01/banner-angular-aad.png
image_webp: /assets/images/2022/01/banner-angular-aad.webp
date: '2022-01-31 14:39:53'
tags:
  - angular
  - azure-active-directory
  - azure-ad
  - msal
  - web-development
  - microsoft-365-development
featured: false
hidden: false
---

Using MSAL Angular and `MsalGuard` is the easiest way to secure your Angular app with the Microsoft Identity Platform. But if you want to use a custom login page rather than redirecting users directly to Azure Active Directory, there's one thing you need to consider.

## Secure your Angular app with the Microsoft Identity Platform

When you build Angular apps for your organization, you likely need to secure them. They shouldn't be available to just anybody, especially when they're accessible over the internet. Instead, people should sign in first, before they're allowed to access the app.

While you could add a rudimentary user management system to your app, it's rarely a good idea. It has nothing to do with the problem that your app is solving and it's extremely hard to do well. You need to consider managing user accounts, dealing with expired passwords, multi-factor authentication, not to mention more complex things like conditional access. It's not trivial, and like I just mentioned, it has nothing to do with the problem that your app is solving. On top of all that, you require people to create and manage yet another account.

If your organization uses Microsoft 365, you can use the Microsoft Identity Platform to secure your app. You get all of the user management features for free and your colleagues can use your app with the same account they use to access Outlook or Teams!

## Secure your Angular app with the Microsoft Identity Platform using MSAL Angular

The easiest way to secure Angular apps with the Microsoft Identity Platform is by using the [MSAL (Microsoft Authentication Library) Angular](https://github.com/AzureAD/microsoft-authentication-library-for-js/tree/dev/lib/msal-angular) package. This package contains Angular-specific building blocks for implementing MSAL in your app.

To secure your Angular app using MSAL Angular, you'll need two building blocks: `MsalGuard` and the `MsalRedirectComponents`.

You add `MsalGuard` to your routes that require users to sign in. For example:

```typescript
import { Routes } from '@angular/router';
import { MsalGuard } from '@azure/msal-angular';

const app_routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: '/customers' },
  { path: 'customers/:id', data: { preload: true }, loadChildren: () => import('./customer/customer.module').then(m => m.CustomerModule), canActivate: [MsalGuard] },
  { path: 'customers', loadChildren: () => import('./customers/customers.module').then(m => m.CustomersModule), canActivate: [MsalGuard] },
  { path: 'orders', data: { preload: true }, loadChildren: () => import('./orders/orders.module').then(m => m.OrdersModule), canActivate: [MsalGuard] },
  { path: 'about', loadChildren: () => import('./about/about.module').then(m => m.AboutModule) },
  { path: '**', pathMatch: 'full', redirectTo: '/customers' } // catch any unfound routes and redirect to home page
];
```

In this example all routes except `/about` require users to sign in.

Next, you need to add the `MsalRedirectComponent` which handles redirects from the Azure AD login page back to your app. The easiest way to do it is by adding an extra route mapped to the `MsalRedirectComponent` and not secured with `MsalGuard`:

```typescript
import { Routes } from '@angular/router';
import { MsalGuard, MsalRedirectComponent } from '@azure/msal-angular';

const app_routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: '/customers' },
  { path: 'customers/:id', data: { preload: true }, loadChildren: () => import('./customer/customer.module').then(m => m.CustomerModule), canActivate: [MsalGuard] },
  { path: 'customers', loadChildren: () => import('./customers/customers.module').then(m => m.CustomersModule), canActivate: [MsalGuard] },
  { path: 'orders', data: { preload: true }, loadChildren: () => import('./orders/orders.module').then(m => m.OrdersModule), canActivate: [MsalGuard] },
  { path: 'about', loadChildren: () => import('./about/about.module').then(m => m.AboutModule) },
  { path: 'auth', component: MsalRedirectComponent },
  { path: '**', pathMatch: 'full', redirectTo: '/customers' } // catch any unfound routes and redirect to home page
];
```

With this configuration in place, if the user didn't sign in and they try to navigate to a route with the `MsalGuard`, `MsalGuard` will redirect them to the Azure AD login page.

<p><picture>
  <source srcset="/assets/images/2022/01/angular-aad-msalguard.webp" type="image/webp">
  <img src="/assets/images/2022/01/angular-aad-msalguard.png" alt="Three blocks that illustrate the user flow with MsalGuard: accessing a secured route, redirecting to the Azure AD login page and redirecting page to the originally requested route">
</picture></p>

This is all you need to do to secure your Angular app. But rather than redirecting people directly to Azure AD, you might want to show them a custom page first.

## Redirect to a custom login page before redirecting users to Azure AD

In the previous example, when users tried to open a route secured with the `MsalGuard`, they were automatically redirected to the Azure AD login page. After signing in with their work account, they were redirected back to the route they requested initially. While this flow does its job, some might argue that it's not quite user-friendly. One moment people are in your app, and the other they're in Azure AD, without any additional information. To improve it, you might want to put a custom login page in between with some additional information on it and a login button that people use to start the login flow:

<p><picture>
  <source srcset="/assets/images/2022/01/angular-aad-custom-guard.webp" type="image/webp">
  <img src="/assets/images/2022/01/angular-aad-custom-guard.png" alt="Four blocks that illustrate the user flow: accessing a secured route, redirecting to a custom login page, navigating to the Azure AD login page and redirecting page to the originally requested route">
</picture></p>

Your first idea to implement it would be to replace `MsalGuard` with a custom guard that checks if the user is signed in and redirect to the login page if they're not:

```typescript
import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { MsalBroadcastService, MsalService } from '@azure/msal-angular';
import { InteractionStatus } from '@azure/msal-browser';
import { filter, Observable, of, switchMap } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable({ providedIn: 'root' })
export class CanActivateGuard implements CanActivate {
  constructor(private msalService: MsalService,
    private authService: AuthService,
    private router: Router,
    private msalBroadcastService: MsalBroadcastService) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    return this.msalBroadcastService.inProgress$
      .pipe(
        filter((status: InteractionStatus) => status === InteractionStatus.None),
        switchMap(() => {
          if (this.msalService.instance.getAllAccounts().length > 0) {
            return of(true);
          }

          this.authService.redirectUrl = state.url;
          this.router.navigate(['/login']);
          return of(false);
        })
      );
  }
}
```

The custom guard subscribes to events raised by the MSAL broadcast service and checks if a user account is available in the MSAL service, which indicates that the user has signed in.

You'd update your route definitions to use your custom guard instead:

```typescript
import { Routes } from '@angular/router';
import { CanActivateGuard } from './core/guards/can-activate.guard';
import { MsalRedirectComponent } from '@azure/msal-angular';

const app_routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: '/customers' },
  { path: 'customers/:id', data: { preload: true }, loadChildren: () => import('./customer/customer.module').then(m => m.CustomerModule), canActivate: [CanActivateGuard] },
  { path: 'customers', loadChildren: () => import('./customers/customers.module').then(m => m.CustomersModule), canActivate: [CanActivateGuard] },
  { path: 'orders', data: { preload: true }, loadChildren: () => import('./orders/orders.module').then(m => m.OrdersModule), canActivate: [CanActivateGuard] },
  { path: 'about', loadChildren: () => import('./about/about.module').then(m => m.AboutModule) },
  { path: 'auth', component: MsalRedirectComponent },
  { path: '**', pathMatch: 'full', redirectTo: '/customers' } // catch any unfound routes and redirect to home page
];
```

Unfortunately, this setup doesn't work quite as you would've thought. If people tried to access a route that requires them to be signed in, they would get redirected to the login page as expected. After clicking the login button, they'd be redirected to the Azure AD login page. But after signing in with their work, when they're redirected back to your app, they wouldn't be signed in!

The reason for that is, that it's not the `MsalRedirectComponent` that's responsible for processing the response from Azure AD and signing the user into your app. It's the `MsalGuard` that does that, and since we've removed it, our app considers the authentication is still in progress. So how to solve it?

## Combine `MsalGuard` with a custom guard

To redirect users to a custom login page and properly handle responses from Azure AD with the **minimal amount of code**, you need to use both your custom guard and the `MsalGuard`. Your custom guard will handle redirecting users to the login page, while `MsalGuard` will handle processing redirects from Azure AD and registering users as signed in with your app:

```typescript
import { Routes } from '@angular/router';
import { CanActivateGuard } from './core/guards/can-activate.guard';
import { MsalGuard, MsalRedirectComponent } from '@azure/msal-angular';

const app_routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: '/customers' },
  { path: 'customers/:id', data: { preload: true }, loadChildren: () => import('./customer/customer.module').then(m => m.CustomerModule), canActivate: [CanActivateGuard, MsalGuard] },
  { path: 'customers', loadChildren: () => import('./customers/customers.module').then(m => m.CustomersModule), canActivate: [CanActivateGuard, MsalGuard] },
  { path: 'orders', data: { preload: true }, loadChildren: () => import('./orders/orders.module').then(m => m.OrdersModule), canActivate: [CanActivateGuard, MsalGuard] },
  { path: 'about', loadChildren: () => import('./about/about.module').then(m => m.AboutModule) },
  { path: 'auth', component: MsalRedirectComponent },
  { path: '**', pathMatch: 'full', redirectTo: '/customers' } // catch any unfound routes and redirect to home page
];
```

With both guards in place, your Angular app will offer users a better user experience clearly managing their expectations. And you'll be able to do it without having to re-implement any code that's already a part of MSAL Angular.

## Summary

Using MSAL Angular is the easiest way to secure Angular apps with the Microsoft Identity Platform. Using the `MsalGuard` and `MsalRedirectComponent` you can specify which routes require users to sign in. By adding a custom guard, you can improve the user experience by redirecting users to a custom login page with additional information, before redirecting them directly to the Azure AD login page.

_Big thanks to my colleague [Wassim Chegham](https://github.com/manekinekko) for helping me figure it out. The code in this article is from a [sample app](https://github.com/waldekmastykarz/Angular-JumpStart/tree/identity) built by my other colleague [Dan Wahlin](https://github.com/DanWahlin)._
