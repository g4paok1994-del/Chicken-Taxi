# Play Chicken Taxi online — GitHub Pages setup

The complete offline HTML5 game is ready in the **lowercase `docs/` folder on branch `main`**:

- `docs/index.html` — game page
- `docs/style.css` — responsive appearance
- `docs/game.js` — endless chicken-taxi gameplay

These are copies of the existing Android WebView game assets. You do **not** need an APK, Unity, a paid host or any secret to publish them on GitHub Pages.

## Why the old link did not work

The automatic Pages deployment runs on November 18, 2026 failed at **Configure GitHub Pages** with a 404: **Get Pages site failed**. The GitHub repository did not yet have Pages activated. The files themselves were successfully checked for presence and JavaScript syntax.

## Easiest activation — only one GitHub setting

1. Sign in to GitHub.
2. Open **[Chicken Taxi → Settings → Pages](https://github.com/g4paok1994-del/Chicken-Taxi/settings/pages)**.
3. Under **Build and deployment**, set **Source → Deploy from a branch** (not GitHub Actions).
4. Under **Branch**, choose **main**.
5. Under **Folder**, choose **/docs** (lowercase).
6. Click **Save**.
7. In GitHub Pages settings, wait until GitHub indicates a successful deployment. Only then try the public URL:

   **https://g4paok1994-del.github.io/Chicken-Taxi/**

GitHub Pages will serve `docs/index.html` from the repository root URL automatically. The CSS and JS paths are relative.

## Troubleshooting

- **404 / Site not found:** The Pages setting was not saved, publication has not finished, or the Pages feature is not available for this repository/account.
- **GitHub Actions deploy error:** The old Actions-based deployment cannot activate Pages with its default GitHub token. The **main /docs** branch method is the recommended route.
- **Page opens but no game:** Open browser Developer Tools and check for missing `style.css`, `game.js` or JavaScript errors.
- **Old version in browser:** Refresh the tab or clear the site's browser cache.
- **Browser state:** Coins and upgrades are stored locally for that browser/profile only, not synced online.

## Deployment maintenance

If you edit the game source in `Android/app/src/main/assets/`, copy the revised game web files to `docs/` before expecting GitHub Pages to reflect the changes. The Android APK build remains a separate workflow.

The site cannot be described as publicly live until the branch Pages setting has been enabled and a successful Pages deployment has been confirmed.
