<h1 align="center">Better Trakt</h1>
<h3 align="center">An unofficial, drop-in Trakt plugin fork for Jellyfin</h3>

<p align="center">
<img alt="Better Trakt logo" width="180" src="https://raw.githubusercontent.com/selmant/better-trakt/better-trakt/art/better-trakt.svg"/>
<br/>
<br/>
<a href="https://github.com/selmant/better-trakt/actions/workflows/build.yaml">
<img alt="Build status" src="https://github.com/selmant/better-trakt/actions/workflows/build.yaml/badge.svg?branch=better-trakt">
</a>
<a href="https://github.com/selmant/better-trakt">
<img alt="MIT License" src="https://img.shields.io/github/license/selmant/better-trakt.svg"/>
</a>
<a href="https://github.com/selmant/better-trakt/releases">
<img alt="Current release" src="https://img.shields.io/github/v/release/selmant/better-trakt.svg"/>
</a>
</p>

## About

Better Trakt synchronizes Jellyfin watch states with Trakt and adds per-user self-service plus an optional, administrator-controlled integration surface for applications such as Foreseer.

This project is independently maintained and is not an official Jellyfin or Trakt project. It keeps the upstream plugin GUID, assembly name, routes, and `Trakt.xml` configuration file so it replaces the official Trakt plugin without losing existing users or authorization tokens. Do not run Better Trakt and the official Trakt plugin together.

## User self-service

Each Jellyfin user can link their own trakt.tv account and manage scrobble/sync preferences without admin access.

### Open the user page

Share or bookmark:

```text
/Trakt/SelfService
```

Example: `https://jellyfin.example.com/Trakt/SelfService`

The same link is shown (with a copy button) on **Dashboard → Plugins → Better Trakt**. Users must already be logged into Jellyfin in that browser (the page reads the session from `localStorage` and calls the `/Trakt/me*` APIs).

Do **not** use jellyfin-web `#/configurationpage` for this UI — that route is restricted to administrators and sends normal users home.

Stock jellyfin-web does not list plugin pages in the sidebar for non-admin users. Discovery is optional:

1. **Share the URL** (works for any logged-in user), or
2. **Optional sidebar entry** via jellyfin-web [`menuLinks`](https://jellyfin.org/docs/general/clients/web-config/) in the web `config.json`:

```json
"menuLinks": [
  {
    "name": "Better Trakt",
    "icon": "tv",
    "url": "/Trakt/SelfService"
  }
]
```

The plugin never modifies `config.json`. Admins can still configure any user from **Dashboard → Plugins → Better Trakt**.

### Self-service API

Authenticated as a Jellyfin user (except the HTML page, which is anonymous so the document can load):

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/Trakt/SelfService` | Standalone self-service HTML |
| `GET` | `/Trakt/me` | Link status and editable preferences (no tokens) |
| `POST` | `/Trakt/me/Authorize` | Start device auth for the caller |
| `GET` | `/Trakt/me/PollAuthorizationStatus` | Wait for device auth completion |
| `POST` | `/Trakt/me/Deauthorize` | Unlink Trakt (keeps preferences) |
| `PUT` | `/Trakt/me/Settings` | Update the caller's preferences |
| `GET` | `/Trakt/me/Token` | Access token + expiry (see below) |

`PUT /Trakt/me/Settings` does **not** change admin-only fields: debug logging, library folder exclusions, or token export.

Routes under `/Trakt/Users/{userGuid}/...` allow the **same user** or an **administrator**. Deauthorize there also unlinks while keeping preferences (same as `/Trakt/me/Deauthorize`).

### Token export for other apps

`GET /Trakt/me/Token` returns `{ "accessToken": "...", "accessTokenExpiration": "..." }` only when:

1. The user has linked Trakt, and
2. An **administrator** enabled **Allow other apps to read this user's Trakt access token** for that user on **Dashboard → Plugins → Better Trakt**.

This is an admin-only policy (users cannot enable it via self-service). Disabled by default per user.
Admins can enable **Default: allow token export for new users** (server-wide) so newly created Trakt configs inherit `AllowExternalTokenAccess = true` (existing users are unchanged; override per user as needed).
The refresh token is never returned; apps should call this endpoint again when the access token is near expiry (Jellyfin refreshes it server-side).

## Installation and updates

Add this repository in **Dashboard → Plugins → Repositories**:

```text
https://raw.githubusercontent.com/selmant/better-trakt/manifest-release/manifest.json
```

The fork uses a `1000.x` version namespace, so Jellyfin treats it as newer than the official plugin with the same GUID. Install **Better Trakt** from the catalog and restart Jellyfin. Existing `Trakt.xml` configuration and linked accounts are retained.

Future releases are delivered by Jellyfin's normal plugin update task through the same repository. To return to the official plugin, uninstall Better Trakt, restart Jellyfin, and install Trakt from the official catalog. Do not delete `Trakt.xml` during that process.

## Build

1. To build this plugin you will need [.NET 9.x](https://dotnet.microsoft.com/download/dotnet/9.0).

2. Build plugin with following command
  ```
  dotnet publish --configuration Release --output bin
  ```

3. Place `Trakt.dll` in a plugin directory and restart Jellyfin.

## Releasing

Update the same four-part version in `Directory.Build.props` and `build.yaml`, then create a GitHub release with the matching `vVERSION` tag. The publish workflow packages the plugin with [JPRM](https://github.com/oddstr13/jellyfin-plugin-repository-manager), attaches checksums, and regenerates `manifest-release/manifest.json` automatically.

The first component remains `1000`. The remaining components identify the year, month/day, and revision, for example `1000.2026.731.2`.

## Contributing

Changes from [`jellyfin/jellyfin-plugin-trakt`](https://github.com/jellyfin/jellyfin-plugin-trakt) are regularly merged into this fork. Bug reports for Better Trakt should be opened here rather than in Jellyfin's official plugin repository.

## Licence

The source retains the upstream MIT License. See [LICENSE](./LICENSE.md) for more information.
