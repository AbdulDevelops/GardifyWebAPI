package com.gardify.android.ui;

import android.content.Intent;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.os.Handler;
import android.widget.TextView;

import androidx.appcompat.app.AppCompatActivity;

import com.gardify.android.R;
import com.gardify.android.utils.PreferencesUtility;

public class SplashScreenActivity extends AppCompatActivity {


    /**
     * Duration of wait
     **/
    private final int SPLASH_DISPLAY_LENGTH = 1000;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_splashscreen);

        TextView appVersionTxt = findViewById(R.id.textView_splash_screen_version_number);
        String versionName = getVersionName();
        appVersionTxt.setText(versionName);


        new Handler().postDelayed(() -> {
            Intent mainIntent;
            /* Create an Intent that will start the Menu-Activity. */
            if (PreferencesUtility.getIsFirstTimeUser(this) || !PreferencesUtility.getLoggedIn(this))
                mainIntent = new Intent(SplashScreenActivity.this, RegisterSplashActivity.class);
            else
                mainIntent = new Intent(SplashScreenActivity.this, MainActivity.class);
            startActivity(mainIntent);
            finish();
        }, SPLASH_DISPLAY_LENGTH);
    }

    private String getVersionName() {
        String versionName = "";
        try {
            PackageInfo pInfo = getPackageManager().getPackageInfo(getPackageName(), 0);
            versionName = pInfo.versionName;//Version Name
        } catch (PackageManager.NameNotFoundException e) {
            e.printStackTrace();
        }
        return versionName;
    }
}