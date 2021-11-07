package com.gardify.android.ui.settings;

import android.app.Activity;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;
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

import static com.gardify.android.utils.UiUtils.displayAlertDialog;
import static com.gardify.android.utils.UiUtils.showErrorDialogNetworkParsed;
import static com.gardify.android.utils.UiUtils.navigateToFragment;

public class EditEmailBottomSheetDialog extends BottomSheetDialogFragment implements View.OnClickListener {

    private EditText editTextEmail;
    private EditText editTextPassword;
    private Button buttonSave;
    private Button buttonClose;

    public View onCreateView(@NonNull LayoutInflater layoutInflater, ViewGroup container, Bundle savedInstanceState){

        View root = layoutInflater.inflate(R.layout.fragment_settings_email, container, false);
        init(root);

        return root;
    }

    public void init(View root){
        editTextEmail = root.findViewById(R.id.editText_settings_email_email);
        editTextPassword = root.findViewById(R.id.editText_settings_email_password);
        buttonSave = root.findViewById(R.id.button_settings_email_save);
        buttonClose = root.findViewById(R.id.button_settings_email_close);

        buttonSave.setOnClickListener(this);
        buttonClose.setOnClickListener(this);
    }

    @Override
    public void onClick(View v) {
        switch (v.getId()){
            case R.id.button_settings_email_save:
                ApplicationUser user = PreferencesUtility.getUser(getContext());
                onSuccessSave(user);
                break;
            case R.id.button_settings_email_close:
                onSuccessClose();
                break;
        }
    }

    public void onSuccessSave(ApplicationUser user) {
        //ApplicationUser user = new Gson().fromJson(String.valueOf(jsonObject), ApplicationUser.class);
        String api = APP_URL.ACCOUNT_API + "update/" + user.getUserId();

        String textNewEmail = editTextEmail.getText().toString();
        String textPassword = editTextPassword.getText().toString();

        HashMap<String, String> params = new HashMap<>();
        params.put("Id", user.getUserId());
        params.put("NewEmail", textNewEmail);
        params.put("Password", textPassword);

        try {

            JSONObject json = new JSONObject(params);

            RequestQueueSingleton.getInstance(getContext()).objectRequest(api, Request.Method.PUT, this::onSuccessRefresh, this::OnError, json);

        } catch (Exception e) {
            Toast.makeText(getContext(), e.toString(), Toast.LENGTH_LONG).show();
        }
    }

    private void OnError(VolleyError error) {
        showErrorDialogNetworkParsed(getContext(), error);
    }

    public void onSuccessRefresh(JSONObject jsonObject) {
        displayAlertDialog(getContext(), "Email Aktualisiert");
        navigateToFragment(R.id.nav_controller_settings, (Activity) getContext(), true, null);
        dismiss();
    }

    public void onSuccessClose(){
        dismiss();
    }

    @Override
    public int getTheme() {
        return R.style.BaseBottomSheetDialog;
    }
}
