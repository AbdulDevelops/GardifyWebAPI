package com.gardify.android.ui.authentication.recyclerItems;

import android.content.Context;
import android.text.Editable;
import android.text.TextWatcher;
import android.util.Log;
import android.view.View;
import android.widget.ArrayAdapter;

import androidx.annotation.ColorInt;
import androidx.annotation.NonNull;

import com.android.volley.Request;
import com.android.volley.VolleyError;
import com.gardify.android.R;
import com.gardify.android.data.myGarden.UserGarden.UserGarden;
import com.gardify.android.databinding.RecyclerViewAuthFragmentRegisterBinding;
import com.gardify.android.ui.authentication.interfaces.OnUpdate;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.RequestQueueSingleton;
import com.xwray.groupie.databinding.BindableItem;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.regex.Matcher;
import java.util.regex.Pattern;

import static com.gardify.android.utils.UiUtils.displayAlertDialog;
import static com.gardify.android.utils.UiUtils.showErrorDialogNetworkParsed;
import static com.gardify.android.viewModelData.UserDetail.deviceDetail;

/**
 * A card item with a fixed width so it can be used with a horizontal layout manager.
 */
public class RegisterRow extends BindableItem<RecyclerViewAuthFragmentRegisterBinding> {

    private UserGarden userGarden;
    private Context context;
    @ColorInt
    private int colorRes;
    private int stringID;
    private OnUpdate onUpdate;
    private RecyclerViewAuthFragmentRegisterBinding binding;

    public RegisterRow(Context context, int colorRes, int stringID, OnUpdate onUpdate) {
        this.context = context;
        this.colorRes = colorRes;
        this.stringID = stringID;
        this.onUpdate = onUpdate;
    }

    @Override
    public int getLayout() {
        return R.layout.recycler_view_auth_fragment_register;
    }

    @Override
    public void bind(@NonNull RecyclerViewAuthFragmentRegisterBinding _binding, int position) {
        binding = _binding;
        String[] items = context.getResources().getStringArray(R.array.countries);
        ArrayAdapter<String> countryArrayAdapter = new ArrayAdapter<>(context, R.layout.custom_spinner_item, items); //selected item will look like a spinner set from XML
        _binding.spinnerCountry.setAdapter(countryArrayAdapter);

        binding.editTextRegisterPassword.addTextChangedListener(registrationTextWatcher());
        binding.editTextRegisterPasswordConfirm.addTextChangedListener(registrationTextWatcher());
        binding.editTextRegisterEmail.addTextChangedListener(registrationTextWatcher());
        binding.editTextRegisterUsername.addTextChangedListener(registrationTextWatcher());
        binding.editTextRegisterPostalCode.addTextChangedListener(registrationTextWatcher());

        _binding.buttonRegister.setOnClickListener(v -> {
            Login(_binding);
        });
    }

    private void Login(RecyclerViewAuthFragmentRegisterBinding binding) {

        boolean validInput = ValidateUserInput(binding);
        if (validInput) {

            String getEmailText = binding.editTextRegisterEmail.getText().toString();
            String getUserName = binding.editTextRegisterUsername.getText().toString();
            String getCountry = binding.spinnerCountry.getSelectedItem().toString();
            String getPostalCode = binding.editTextRegisterPostalCode.getText().toString();
            String getPassword = binding.editTextRegisterPassword.getText().toString();
            String getPasswordConfirm = binding.editTextRegisterPasswordConfirm.getText().toString();
            boolean subscriptionBool = binding.switchNewsletterSubscription.isChecked();

            JSONObject objParams = new JSONObject();
            try {
                objParams.put("Lastname", " ");
                objParams.put("Firstname", " ");
                objParams.put("Street", " ");
                objParams.put("HouseNr", " ");
                objParams.put("PLZ", getPostalCode);
                objParams.put("City", " ");
                objParams.put("Country", getCountry);
                objParams.put("UserName", getUserName);
                objParams.put("Gender", " ");
                objParams.put("Email", getEmailText);
                objParams.put("Password", getPassword);
                objParams.put("ConfirmPassword", getPasswordConfirm);
                objParams.put("model", deviceDetail());
            } catch (JSONException e) {
                e.printStackTrace();
            }

            String registerUrl = APP_URL.ACCOUNT_API + "register/true/" + subscriptionBool;
            //String registerUrl = "http://httpbin.org/post";
            RequestQueueSingleton.getInstance(context).objectRequest(registerUrl, Request.Method.POST, this::RegisterSuccess, this::RegisterError, objParams);
            binding.progressBarAuthentication.setVisibility(View.VISIBLE);
        }
    }

    private void RegisterSuccess(JSONObject jsonObject) {
        binding.progressBarAuthentication.setVisibility(View.GONE);

        String message = context.getString(R.string.registerLogin_registrationSuccess);


        displayAlertDialog(context, message);
        clearInputFields();
    }

    private void clearInputFields() {
        binding.editTextRegisterEmail.setText("");
        binding.editTextRegisterUsername.setText("");
        binding.editTextRegisterPostalCode.setText("");
        binding.editTextRegisterPassword.setText("");
        binding.editTextRegisterPasswordConfirm.setText("");

        //set focus to email
        binding.editTextRegisterEmail.requestFocus();

    }

    private void RegisterError(VolleyError error) {
        binding.progressBarAuthentication.setVisibility(View.GONE);
        showErrorDialogNetworkParsed(context, error);
    }

    public Boolean ValidateUserInput(RecyclerViewAuthFragmentRegisterBinding binding) {

        boolean validFlag = true;

        if (binding.editTextRegisterEmail.getText().toString().trim().length() == 0) {
            validFlag = false;
            binding.editTextRegisterEmail.setError(context.getResources().getString(R.string.all_required));
        } else if (binding.editTextRegisterUsername.getText().toString().trim().length() == 0) {
            validFlag = false;
            binding.editTextRegisterUsername.setError(context.getResources().getString(R.string.all_required));
        } else if (binding.editTextRegisterPostalCode.getText().toString().trim().length() == 0) {
            validFlag = false;
            binding.editTextRegisterPostalCode.setError(context.getResources().getString(R.string.all_required));
        }else if (!binding.editTextRegisterPassword.getText().toString().equals(binding.editTextRegisterPasswordConfirm.getText().toString())) {
            validFlag = false;
            binding.editTextRegisterPasswordConfirm.setError(context.getString(R.string.all_passwordDoesNotMatch));
        } else if (!validPasswordStrength(binding.editTextRegisterPassword.getText().toString())) {
            validFlag = false;
            binding.editTextRegisterPassword.setError(context.getResources().getString(R.string.all_passwordValidationHint));
        } else if (!validPasswordStrength(binding.editTextRegisterPasswordConfirm.getText().toString())) {
            validFlag = false;
            binding.editTextRegisterPasswordConfirm.setError(context.getResources().getString(R.string.all_passwordValidationHint));
        }
        return validFlag;
    }

    Pattern letter = Pattern.compile("[a-zA-z]");
    Pattern digit = Pattern.compile("[0-9]");
    Pattern special = Pattern.compile("[[.]!@#$%&*()_+=|<>?{}\\[\\]~-]");
    Pattern minimumLength = Pattern.compile(".{6}");

    @NonNull
    private TextWatcher registrationTextWatcher() {
        return new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {

            }

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {

            }

            @Override
            public void afterTextChanged(Editable s) {
                Log.d("valide dinge", "afterTextChanged: " + ValidateUserInput(binding));
                if(!ValidateUserInput(binding))
                    binding.passwordRegisterValidationHint.setTextColor(context.getResources().getColor(R.color.text_all_jasper));
                else binding.passwordRegisterValidationHint.setTextColor(context.getResources().getColor(R.color.colorPrimary));

            }
        };
    }

    public final boolean validPasswordStrength(String password) {
        Matcher hasLetter = letter.matcher(password);
        Matcher hasDigit = digit.matcher(password);
        Matcher hasSpecial = special.matcher(password);
        Matcher hasSix = minimumLength.matcher(password);
        boolean boolHasLetter = hasLetter.find();
        boolean boolHasDigit = hasDigit.find();
        boolean boolHasSpecial = hasSpecial.find();
        boolean boolHasSix = hasSix.find();

        return boolHasLetter && boolHasDigit && boolHasSpecial
                && boolHasSix;
    }
}
