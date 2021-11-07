package com.gardify.android.ui;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.ImageView;

import com.gardify.android.R;

public class RegisterSplashActivity extends AppCompatActivity {

    Button buttonRegister, buttonLogin;
    ImageView imageViewCross;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_register_splash);

        init();
    }

    private void init() {
        buttonRegister = findViewById(R.id.button_splashScreen_register);
        buttonLogin = findViewById(R.id.button_splashScreen_login);
        imageViewCross = findViewById(R.id.imageView_splashScreen_cross);
        imageViewCross.setOnClickListener(onClickListener);
        buttonRegister.setOnClickListener(onClickListener);
        buttonLogin.setOnClickListener(onClickListener);
    }

    private View.OnClickListener onClickListener = v -> {
        Intent intent = new Intent(RegisterSplashActivity.this, MainActivity.class);
        switch (v.getId()) {
            case R.id.imageView_splashScreen_cross:
                startActivity(intent);
                finish();
                break;
            case R.id.button_splashScreen_register:
                intent.putExtra("splashRegisterClick", true);
                startActivity(intent);
                finish();
                break;
            case R.id.button_splashScreen_login:
                intent.putExtra("splashRegisterClick", false);
                startActivity(intent);
                finish();
                break;
        }
    };


}