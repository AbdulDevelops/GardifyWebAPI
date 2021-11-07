package com.gardify.android.ui.authentication.recyclerItems;

import android.content.Context;
import android.view.View;
import android.widget.LinearLayout;
import android.widget.Toast;

import androidx.annotation.ColorInt;
import androidx.annotation.NonNull;

import com.android.volley.Request;
import com.android.volley.VolleyError;
import com.gardify.android.R;
import com.gardify.android.databinding.RecyclerViewAuthFragmentLoginBinding;
import com.gardify.android.generic.GenericDialog;
import com.gardify.android.ui.authentication.interfaces.OnUpdate;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.PreferencesUtility;
import com.gardify.android.utils.RequestQueueSingleton;
import com.xwray.groupie.databinding.BindableItem;

import org.json.JSONObject;

import java.util.HashMap;
import java.util.Map;

import static com.gardify.android.utils.UiUtils.showErrorDialogNetworkParsed;
import static com.gardify.android.utils.UiUtils.updateNavFooterBasedOnLoginStatus;
import static com.gardify.android.viewModelData.UserDetail.deviceDetail;

/**
 * A card item with a fixed width so it can be used with a horizontal layout manager.
 */
public class LoginRow extends BindableItem<RecyclerViewAuthFragmentLoginBinding> {

    private Context context;
    @ColorInt
    private int colorRes;
    private int stringID;
    private OnUpdate onUpdate;
    private Boolean forgetPassword = false;
    private RecyclerViewAuthFragmentLoginBinding binding;

    public LoginRow(Context context, int colorRes, int stringID, OnUpdate onUpdate) {
        this.context = context;
        this.colorRes = colorRes;
        this.stringID = stringID;
        this.onUpdate = onUpdate;
    }

    @Override
    public int getLayout() {
        return R.layout.recycler_view_auth_fragment_login;
    }

    @Override
    public void bind(@NonNull RecyclerViewAuthFragmentLoginBinding _binding, int position) {
        binding = _binding;

        _binding.buttonLogin.setOnClickListener(v -> {
            login(forgetPassword);
        });

        _binding.textViewForgotPassword.setOnClickListener(v -> {
            showForgetPasswordUi();
        });
    }

    private void showForgetPasswordUi() {
        forgetPassword = true;
        String confirm = context.getResources().getString(R.string.all_confirm);
        binding.buttonLogin.setText(confirm);
        binding.textViewForgotPassword.setVisibility(View.GONE);
        binding.textInputPasswordLayout.setVisibility(View.GONE);
        binding.linearLayoutRememberMe.setVisibility(View.GONE);
    }

    private void login(boolean forgetPassword) {

        if (!forgetPassword) {
            boolean validInput = ValidateUserInput();
            if (validInput) {

                try {


                    String getEmail = binding.editTextLoginEmail.getText().toString();
                    String getPass = binding.editTextLoginPassword.getText().toString();
                    boolean rememberValue = binding.checkBoxRememberMe.isChecked();

                    JSONObject params = new JSONObject();
                    params.put("Email", getEmail);
                    params.put("Password", getPass);
                    params.put("RememberMe", String.valueOf(rememberValue));
                    params.put("model", deviceDetail());

                    String loginUrl = APP_URL.ACCOUNT_API + "login/true";
                    RequestQueueSingleton.getInstance(context).objectRequest(loginUrl, Request.Method.POST, this::LoginSuccess, this::onError, params);
                    binding.progressBarAuthentication.setVisibility(View.VISIBLE);
                } catch (Exception e) {
                    Toast.makeText(context, e.toString(), Toast.LENGTH_LONG).show();
                }
            }
        } else {

            String getEmail = binding.editTextLoginEmail.getText().toString();
            if (getEmail.trim().length() == 0) {
                binding.editTextLoginEmail.setError(context.getResources().getString(R.string.all_required));
                return;
            }

            Map<String, String> params = new HashMap<>();

            params.put("Email", getEmail);

            JSONObject objParams = new JSONObject(params);

            String forgetPasswordApi = APP_URL.ACCOUNT_API + "forgot";
            RequestQueueSingleton.getInstance(context).objectRequest(forgetPasswordApi, Request.Method.POST, this::forgetPasswordSuccess, this::onError, objParams);
            binding.progressBarAuthentication.setVisibility(View.VISIBLE);
        }
    }

    private void forgetPasswordSuccess(JSONObject jsonObject) {

        showLoginUi();
        binding.progressBarAuthentication.setVisibility(View.GONE);

        new GenericDialog.Builder(context)
                .setTitle("Überprüfe deinen E-Mail Posteingang.")
                .setTitleAppearance(R.color.text_all_gunmetal, R.dimen.textSize_body_medium)
                .setMessage("Keine Email erhalten? Bitte überprüfe deinen Spam-Ordner oder")
                .setMessageAppearance(R.color.text_all_riverBed, R.dimen.textSize_body_small)
                .addNewButton(R.style.TransparentButtonStyle,
                        "klicke-hier", R.dimen.textSize_body_medium, view -> {
                            login(forgetPassword);
                        })
                .setButtonOrientation(LinearLayout.VERTICAL)
                .setCancelable(true)
                .generate();
    }

    private void showLoginUi() {
        forgetPassword = false;
        String login = context.getResources().getString(R.string.all_login);
        binding.buttonLogin.setText(login);
        binding.textViewForgotPassword.setVisibility(View.VISIBLE);
        binding.textInputPasswordLayout.setVisibility(View.VISIBLE);
        binding.linearLayoutRememberMe.setVisibility(View.VISIBLE);
    }

    private void onError(VolleyError error) {
        showErrorDialogNetworkParsed(context, error);
        binding.progressBarAuthentication.setVisibility(View.GONE);
    }

    private void LoginSuccess(JSONObject jsonObject) {
        binding.progressBarAuthentication.setVisibility(View.GONE);
        PreferencesUtility.setUser(context, jsonObject.toString());
        onUpdate.onClick(stringID);

        //update nav footer
        updateNavFooterBasedOnLoginStatus(context);

    }

    public Boolean ValidateUserInput() {

        boolean validFlag = true;

        if (binding.editTextLoginEmail.getText().toString().trim().length() == 0) {
            validFlag = false;
            binding.editTextLoginEmail.setError(context.getResources().getString(R.string.all_required));
        }
        if (binding.editTextLoginPassword.getText().toString().trim().length() == 0) {
            validFlag = false;
            binding.editTextLoginPassword.setError(context.getResources().getString(R.string.all_required));
        }
        return validFlag;
    }
}
