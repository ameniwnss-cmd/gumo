# GuMo

[![CI](https://github.com/ameniwnss-cmd/gumo/actions/workflows/ci.yml/badge.svg)](https://github.com/ameniwnss-cmd/gumo/actions/workflows/ci.yml)

An ASP.NET Core web app for **GuMo**, a café in Cologne with two locations (Belgisches Viertel and Deutz): online menu, pre-orders with live status tracking, event bookings with QR-code check-in, and an admin dashboard.

> **Live demo, business recommendations and Instagram growth analysis:** https://ameniwnss-cmd.github.io/gumo-strategy/
> The strategy site shows the website in action, a set of ideas worth building next for the business, and a data analysis of GuMo's Instagram growth.

> This is an independent portfolio / student project. It is not affiliated with or endorsed by GuMo. Brand names and menu content belong to their owners.

## Features

- **Menu** by category (coffee, iced drinks, bagels, focaccia, oats, açaí, juices, sweets) with vegan flags and per-location ordering
- **Cart and pre-orders**: customers order ahead; no online payment, they pay in store
- **Order tracking**: each order gets a tracking link whose status updates in real time, plus an email confirmation
- **Events**: public event pages and bookings; confirmation email with a QR code
- **QR check-in**: staff generate short-lived check-in sessions and verify guests' codes
- **Admin dashboard** (login required): manage orders, menu items, events and bookings
- **Locations** page with opening hours and map links

## Tech stack

| Layer | Choice |
|---|---|
| Framework | ASP.NET Core MVC, .NET 10 |
| Data | Entity Framework Core + SQLite, code-first migrations |
| Auth | ASP.NET Core Identity (cookie auth) |
| Email / QR | MailKit, QRCoder |
| Frontend | Razor views, vanilla JS, Bootstrap/jQuery validation |

## Architecture

```
src/
  Gumo.Core            Entities (Order, MenuItem, Event, EventBooking, Location, ...)
  Gumo.Infrastructure  EF Core DbContext, migrations, seeding, email service
  Gumo.Web             MVC controllers, Razor views, static assets
tests/
  Gumo.Tests           xUnit tests
presentation/          Use-case and class diagrams, project slides
```

See [presentation/class-diagram.png](presentation/class-diagram.png) and [presentation/use-case.png](presentation/use-case.png).

## Getting started

Requirements: [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
git clone https://github.com/ameniwnss-cmd/gumo.git
cd gumo/src/Gumo.Web

# Optional: create an admin account (never commit credentials)
dotnet user-secrets init
dotnet user-secrets set "Admin:Email" "you@example.com"
dotnet user-secrets set "Admin:Password" "<a strong password>"

dotnet run
```

On first start the app creates the SQLite database (`gumo.db`, git-ignored), applies migrations and seeds locations, categories and the menu. The admin account is only created if `Admin:Email` and `Admin:Password` are set.

### Email (optional)

Order and event confirmation emails need SMTP settings. Keep them out of source control:

```bash
dotnet user-secrets set "Email:Username" "<smtp user>"
dotnet user-secrets set "Email:Password" "<app password>"
dotnet user-secrets set "Email:FromAddress" "<sender address>"
```

## Tests

```bash
dotnet test
```

xUnit tests run against an in-memory SQLite database and cover order creation (server-side pricing, quantity/email/location validation, unavailable items, tracking tokens) and database seeding (including that no admin exists without configured credentials).

## Security notes

- No credentials are stored in the repository; the admin account and SMTP login come from user-secrets or environment variables.
- Admin login uses account lockout and anti-forgery tokens.
- The local database and uploaded images are git-ignored.

## Roadmap

Business and product ideas, with the reasoning behind them, are on the [strategy site](https://ameniwnss-cmd.github.io/gumo-strategy/).
