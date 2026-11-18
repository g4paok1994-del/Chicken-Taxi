package com.chickentaxi.game;

import android.app.Activity;
import android.os.Bundle;
import android.view.View;
import android.view.WindowManager;
import android.webkit.WebSettings;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.graphics.Color;

public final class MainActivity extends Activity {
    private WebView gameWebView;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        getWindow().setFlags(
            WindowManager.LayoutParams.FLAG_FULLSCREEN,
            WindowManager.LayoutParams.FLAG_FULLSCREEN
        );
        getWindow().getDecorView().setSystemUiVisibility(
            View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY
                | View.SYSTEM_UI_FLAG_FULLSCREEN
                | View.SYSTEM_UI_FLAG_HIDE_NAVIGATION
                | View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
                | View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN
                | View.SYSTEM_UI_FLAG_LAYOUT_STABLE
        );

        gameWebView = new WebView(this);
        gameWebView.setBackgroundColor(Color.rgb(10, 22, 37));
        gameWebView.setOverScrollMode(View.OVER_SCROLL_NEVER);
        WebSettings settings = gameWebView.getSettings();
        settings.setJavaScriptEnabled(true);
        settings.setDomStorageEnabled(true);
        settings.setAllowFileAccess(true);
        settings.setDefaultTextEncodingName("UTF-8");
        gameWebView.setWebViewClient(new WebViewClient());
        setContentView(gameWebView);
        gameWebView.loadUrl("file:///android_asset/index.html");
    }

    @Override
    public void onBackPressed() {
        if (gameWebView != null) {
            gameWebView.evaluateJavascript(
                "if (window.chickenTaxiBack) window.chickenTaxiBack();", null);
        } else {
            super.onBackPressed();
        }
    }

    @Override
    protected void onPause() {
        if (gameWebView != null) {
            gameWebView.evaluateJavascript(
                "if (window.chickenTaxiPause) window.chickenTaxiPause();", null);
            gameWebView.onPause();
        }
        super.onPause();
    }

    @Override
    protected void onResume() {
        super.onResume();
        if (gameWebView != null) gameWebView.onResume();
    }

    @Override
    protected void onDestroy() {
        if (gameWebView != null) {
            gameWebView.destroy();
            gameWebView = null;
        }
        super.onDestroy();
    }
}
