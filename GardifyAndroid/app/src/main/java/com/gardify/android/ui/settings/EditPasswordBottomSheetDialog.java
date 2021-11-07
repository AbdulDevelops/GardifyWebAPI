package com.gardify.android.ui.settings;

import android.app.Activity;
import android.graphics.Color;
import android.os.Bundle;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.animation.AnimationUtils;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;

import com.android.volley.Request;
import com.android.volley.VolleyError;
import com.gardify.android.data.account.ApplicationUser;
import com.gardify.android.R;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.PreferencesUtility;
import com.gardify.android.utils.RequestQueueSingleton;
import com.google.android.material.bottomsheet.BottomSheetDialogFragment;

import org.json.JSONObject;

import java.util.HashMap;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import static com.gardify.android.utils.UiUtils.displayAlertDialog;
import static com.gardify.android.utils.UiUtils.showErrorDialogNetworkParsed;
import static com.gardify.android.utils.UiUtils.navigateToFragment;

public class EditPasswordBottomSheetDialog extends BottomSheetDialogFragment implements View.OnClickListener {

    private EditText editTextCurrentPassword, editTextNewPassword, editTextConfirmNewPassword;
    private Button buttonSave, buttonClose;
    private TextView passwordValidationHint;

    public View onCreateView(@NonNull LayoutInflater layoutInflater, ViewGroup container, Bundle savedInstanceState) {

        View root = layoutInflater.inflate(R.layout.fragment_settings_password, container, false);
        init(root);

        return root;
    }

    public void init(View root) {
        editTextCurrentPassword = root.findViewById(R.id.editText_settings_password_current);

        editTextNewPassword = root.findViewById(R.id.editText_settings_password_newPassword);
        editTextNewPassword.addTextChangedListener(passwordTextWatcher());

        editTextConfirmNewPassword = root.findViewById(R.id.editText_settings_password_confirmPassword);
        editTextConfirmNewPassword.addTextChangedListener(passwordTextWatcher());

        passwordValidationHint = root.findViewById(R.id.password_validation_hint);
        buttonSave = root.findViewById(R.id.button_settings_password_save);
        buttonClose = root.findViewById(R.id.button_settings_password_close);

        buttonSave.setOnClickListener(this);
        buttonClose.setOnClickListener(this);
    }

    @Override
    public void onClick(View v) {
        switch (v.getId()) {
            case R.id.button_settings_password_save:
                ApplicationUser user = PreferencesUtility.getUser(getContext());
                onSuccessSave(user);
                break;
            case R.id.button_settings_password_close:
                onSuccessClose();
                break;
        }
    }

    public void onSuccessSave(ApplicationUser user) {
        String api = APP_URL.ACCOUNT_API + "update/pass/" + user.getUserId();

        String textOldPassword = editTextCurrentPassword.getText().toString();
        String textNewPassword = editTextNewPassword.getText().toString();
        String textConfirmNewPassword = editTextConfirmNewPassword.getText().toString();

        if (ValidateUserInput()) {

            HashMap<String, String> params = new HashMap<>();
            params.put("Id", user.getUserId());
            params.put("OldPassword", textOldPassword);
            params.put("Password", textNewPassword);
            params.put("ConfirmPassword", textConfirmNewPassword);

            try {
                JSONObject json = new JSONObject(params);
                RequestQueueSingleton.getInstance(getContext()).objectRequest(api, Request.Method.POST, this::onSuccessRefresh, this::OnError, json);
            } catch (Exception e) {
                Toast.makeText(getContext(), e.toString(), Toast.LENGTH_LONG).show();
            }
        }
    }

    public void onSuccessRefresh(JSONObject jsonObject) {
        displayAlertDialog(getContext(), getResources().getString(R.string.settings_passwordUpdated));
        navigateToFragment(R.id.nav_controller_settings, (Activity) getContext(), true, null);

        dismiss();
    }

    private void OnError(VolleyError error) {
        showErrorDialogNetworkParsed(getContext(), error);
    }

    public void onSuccessClose() {
        dismiss();
    }

    public Boolean ValidateUserInput() {

        boolean validFlag = true;
        if (!editTextNewPassword.getText().toString().equals(editTextConfirmNewPassword.getText().toString())) {
            validFlag = false;
            editTextConfirmNewPassword.setError(getResources().getString(R.string.all_passwordDoesNotMatch));
        } else if (!ValidPassword(editTextNewPassword.getText().toString())) {
            validFlag = false;
            passwordValidationHint.startAnimation(AnimationUtils.loadAnimation(getContext(), R.anim.animation_flash_blink));
        } else if (!ValidPassword(editTextConfirmNewPassword.getText().toString())) {
            validFlag = false;
            passwordValidationHint.setAnimation(AnimationUtils.loadAnimation(getContext(), R.anim.animation_flash_blink));
        }
        return validFlag;
    }

    Pattern letter = Pattern.compile("[a-zA-z]");
    Pattern digit = Pattern.compile("[0-9]");
    Pattern special = Pattern.compile("[[.]!@#$%&*()_+=|<>?{}\\[\\]~-]");
    Pattern six = Pattern.compile(".{6}");

    public final boolean ValidPassword(String password) {
        Matcher hasLetter = letter.matcher(password);
        Matcher hasDigit = digit.matcher(password);
        Matcher hasSpecial = special.matcher(password);
        Matcher hasSix = six.matcher(password);
        boolean boolHasLetter = hasLetter.find();
        boolean boolHasDigit = hasDigit.find();
        boolean boolHasSpecial = hasSpecial.find();
        boolean boolHasSix = hasSix.find();

        return boolHasLetter && boolHasDigit && boolHasSpecial
                && boolHasSix;
    }

    @NonNull
    private TextWatcher passwordTextWatcher() {
        return new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {

            }

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {

            }

            @Override
            public void afterTextChanged(Editable s) {
                if (!ValidateUserInput()) {
                    passwordValidationHint.setTextColor(Color.RED);
                } else passwordValidationHint.setTextColor(getResources().getColor(R.color.colorPrimary));
            }
        };
    }

    @Override
    public int getTheme() {
        return R.style.BaseBottomSheetDialog;
    }
}
