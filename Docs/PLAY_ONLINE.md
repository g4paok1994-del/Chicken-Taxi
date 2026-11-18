# Play Chicken Taxi online in your browser

The static HTML5 game lives in `Android/app/src/main/assets/`.

The workflow `.github/workflows/deploy-web.yml` copies `index.html`, `style.css` and `game.js` and deploys them to GitHub Pages without Unity or Android installation.

## First-time setup required by repository owner

The first deployment attempt on November 18, 2026, failed at **Configure GitHub Pages**, with "Get Pages site failed: Not Found." GitHub Pages is not yet enabled.

1. Sign in to GitHub.
2. Open [Chicken Taxi → Settings → Pages](https://github.com/g4paok1994-del/Chicken-Taxi/settings/pages).
3. Under **Build and deployment**, set **Source → GitHub Actions**.
4. Save the selection, if prompted.
5. Open [Actions](https://github.com/g4paok1994-del/Chicken-Taxi/actions/workflows/deploy-web.yml), select **Play Chicken Taxi Online - GitHub Pages**, and click **Run workflow** on `main`.
6. Wait for a **green successful** run. Only then try the public game URL: https://g4paok1994-del.github.io/Chicken-Taxi/

If the deployment fails, open the run logs and check whether Pages is enabled and GitHub Actions has permission to deploy. A link should not be described as live until a successful deployment or webpage test confirms it.

## Hosting model

- No backend, paid hosting, Unity license, APK or internet API is needed to play once the site is published.
- The game loads locally in a browser, with coins and upgrades saved in that browser's local storage.
- No AdMob, analytics or account sign-in are included.
- GitHub Pages workflow redeploys after changes to the game web files on `main`.
