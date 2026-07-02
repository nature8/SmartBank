# SmartBank.Web

A server-rendered **ASP.NET Core Razor Pages** front end for the SmartBank microservices, styled
with **Bootstrap 5** and using built-in **jQuery unobtrusive validation** (client-side) backed by
the same Data Annotations used for server-side validation.

It does **not** call any microservice directly — every request goes through the existing **Gateway**
(YARP) at `http://localhost:5202/`, exactly like Swagger/Postman would.

## How it fits together

```
Browser  -->  SmartBank.Web (Razor Pages, cookie auth)  -->  Gateway (YARP, :5202)  -->  AuthService / CustomerService / AccountService / TransactionService / NotificationService
```

- The UI keeps its **own** login session using an ASP.NET Core auth **cookie**.
- On login, it calls `POST /auth/api/Auth/login` on the Gateway, gets back the JWT + role from
  AuthService, and stores the user's `UserId / FullName / Email / Role` as cookie claims, plus the
  raw JWT in a claim so it can be forwarded as `Authorization: Bearer <token>` on every outgoing
  Gateway call (via `Services/BearerTokenHandler.cs`). Today only AuthService's `/profile` endpoint
  actually checks that header, but this means the moment `[Authorize]` gets added to any other
  service, the UI keeps working with no changes.

## Role-based access control

Roles come straight from AuthService's seeded `Role` table: **Admin** (1), **Manager** (2),
**Customer** (3).

| Area | Admin | Manager | Customer |
|---|---|---|---|
| Dashboard | ✅ (staff view) | ✅ (staff view) | ✅ (personal view) |
| Customers – list/search/create/edit | ✅ | ✅ | ❌ |
| Customers – delete | ✅ | ❌ | ❌ |
| Customers – **My profile** | ❌ | ❌ | ✅ (own record only) |
| Accounts – open / close / look up any customer | ✅ | ✅ | ❌ |
| Accounts – **My accounts** | ❌ | ❌ | ✅ (own accounts only) |
| Deposit / Withdraw / Transfer / Statement | ✅ | ✅ | ✅ |
| Notifications – look up any customer / send | ✅ | ✅ | ❌ |
| Notifications – **My notifications** | ❌ | ❌ | ✅ (own only) |
| Registering staff accounts (Admin/Manager) | ✅ (only Admin sees the role picker) | ❌ | ❌ |

This is enforced with:
1. A **global baseline** in `Program.cs` (`AuthorizeFolder("/")`) — every page requires sign-in
   unless explicitly marked anonymous (Login/Register/AccessDenied/Error).
2. Per-page **`[Authorize(Roles = "...")]`** attributes on each `PageModel`, kept next to the
   handler they protect (important because folders like `/Customers` mix staff-only pages with the
   customer-only `MyProfile` page).
3. The navbar (`Pages/Shared/_Layout.cshtml`) only *renders* links the current role can use —
   this is convenience, not the actual security boundary (the PageModel attributes are).

> **Note on the microservices themselves:** only `AuthService` currently enforces JWT
> authentication (`[Authorize]` on `/profile`). `CustomerService`, `AccountService`,
> `TransactionService` and `NotificationService` have no `[Authorize]` attributes in this codebase,
> so all real access control for those currently lives in this Razor Pages UI. If you later lock
> down those services with `[Authorize(Roles = "...")]`, no UI changes are needed — the bearer
> token is already being forwarded on every call.

## Client-side + server-side validation

Every form's `[BindProperty]` model (`Models/*.cs`) carries Data Annotations
(`[Required]`, `[EmailAddress]`, `[Range]`, `[Compare]`, `[Phone]`, `[StringLength]`, ...). Razor's
`asp-for` tag helpers emit the matching `data-val-*` attributes, and jQuery Validate +
jQuery Validate Unobtrusive (loaded in `_Layout.cshtml`) turn those into live client-side checks —
no full postback needed to see a validation error. The server still re-checks `ModelState.IsValid`
on every `OnPost`, since client-side validation is only a UX nicety.

## Running it

1. Start SQL Server + RabbitMQ, then start each microservice (`AuthService`, `CustomerService`,
   `AccountService`, `TransactionService`, `NotificationService`) and the `Gateway`, in that order,
   using the ports already defined in each service's `launchSettings.json`.
2. Run `SmartBank.Web` (defaults to `http://localhost:5100`). It reads the Gateway's address from
   `appsettings.json -> ApiSettings:GatewayBaseUrl` (`http://localhost:5202/` by default).
3. Open the site, click **Open an account** to self-register (created as a `Customer`), or ask an
   existing Admin to sign in and use **Register** to create `Manager`/`Admin` staff accounts.
4. After registering, a **Customer** won't see any accounts/notifications until an Admin/Manager
   creates a matching `Customer` record (`Customers -> Add customer`) with the same `UserId` that
   was returned when the person registered — CustomerService has no endpoint to do this
   automatically today, only a fire-and-forget RabbitMQ event that CustomerService's background
   consumer picks up. If that consumer is running, the customer record is created automatically a
   few seconds after registration; refresh the dashboard to see it appear.

## Project layout

```
SmartBank.Web/
  Models/            DTOs mirroring each microservice's DTOs, with validation attributes
  Services/          Typed HttpClients (one per microservice) + the bearer-token handler
  Pages/
    Account/         Login, Register, Logout, AccessDenied
    Customers/        Admin/Manager CRUD + customer-only MyProfile
    Accounts/         Admin/Manager open/close/lookup + customer-only MyAccounts
    Transactions/     Deposit, Withdraw, Transfer, Statement (all signed-in roles)
    Notifications/    Admin/Manager send/lookup + customer-only MyNotifications
    Shared/_Layout.cshtml   Bootstrap 5 navbar/layout, role-aware menu
  wwwroot/css, wwwroot/js    Small custom styling + UX helpers
```

## Known limitations / next steps

- Ownership is not enforced server-side for Transactions (any signed-in user can, e.g., deposit
  into any account ID) because the underlying `TransactionService`/`AccountService` don't check
  ownership either. If you want that hardened, add an `[Authorize]` + ownership check in those
  services first.
- There's no endpoint to create a `Customer` record straight from `AuthService.Register` — it only
  happens async via RabbitMQ, or manually by an Admin/Manager on the **Add customer** page.
- Bootstrap, Bootstrap Icons and jQuery (+ validation plugins) are loaded from CDNs for simplicity.
  If you need a fully offline dev environment, vendor them into `wwwroot/lib` and swap the
  `<script>`/`<link>` tags in `_Layout.cshtml`.
