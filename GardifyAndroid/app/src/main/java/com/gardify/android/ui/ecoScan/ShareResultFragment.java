package com.gardify.android.ui.ecoScan;

import android.os.Bundle;
import android.text.Editable;
import android.text.TextWatcher;
import android.util.Base64;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ProgressBar;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;

import com.android.volley.Request;
import com.android.volley.VolleyError;
import com.gardify.android.R;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.PreferencesUtility;
import com.gardify.android.utils.RequestQueueSingleton;

import org.jetbrains.annotations.NotNull;

import java.util.HashMap;
import java.util.Map;

import static com.gardify.android.ui.ecoScan.EcoScanFragment.SCREENSHOT_ECOSCAN_KEY;
import static com.gardify.android.utils.UiUtils.displayAlertDialog;
import static com.gardify.android.utils.UiUtils.showErrorDialogNetworkParsed;
import static com.gardify.android.utils.UiUtils.navigateToFragment;


public class ShareResultFragment extends Fragment implements View.OnClickListener {

    private static final String SHARE_RESULT_FRAGMENT = "ShareResultFragment";
    private Button sendButton;
    private byte[] byteArrayImage;

    private EditText emailsEditText, yourEmailEditText, fromEditText, toEditText, messageEditText;
    private TextView messagePrefix, messageSuffix;
    private ProgressBar progressBar;
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        if (getArguments() != null) {
            byteArrayImage = getArguments().getByteArray(SCREENSHOT_ECOSCAN_KEY);
        }
    }

    public View onCreateView(@NonNull LayoutInflater inflater,
                             ViewGroup container, Bundle savedInstanceState) {

        View root = inflater.inflate(R.layout.fragment_eco_scan_share_result, container, false);

        authenticationCheck();

        init(root);
        setupInitialUI();

        return root;
    }


    public void init(View root) {

        //EditTexts
        emailsEditText = root.findViewById(R.id.edit_text_share_result_email);
        yourEmailEditText = root.findViewById(R.id.edit_text_share_result_your_email);
        fromEditText = root.findViewById(R.id.edit_text_share_result_from);
        toEditText = root.findViewById(R.id.edit_text_share_result_to);
        messagePrefix = root.findViewById(R.id.textView_ecoScanShareResult_messagePrefix);
        messageSuffix = root.findViewById(R.id.textView_ecoScanShareResult_messageSuffix);
        messageEditText = root.findViewById(R.id.edit_text_share_result_email_message);
        progressBar = root.findViewById(R.id.progressBar_ecoScanShareResult);

        //Buttons
        sendButton = root.findViewById(R.id.button_share_fragment_send);
        sendButton.setOnClickListener(this);

        fromEditText.addTextChangedListener(toNameTextWatcher());
        toEditText.addTextChangedListener(fromNameTextWatcher());

    }

    @NotNull
    private TextWatcher fromNameTextWatcher() {
        return new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {

            }

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {

            }

            @Override
            public void afterTextChanged(Editable s) {
                messagePrefix.setText(getResources().getString(R.string.all_hello) + " " + s.toString());
            }
        };
    }
    @NotNull
    private TextWatcher toNameTextWatcher() {
        return new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {

            }

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {

            }

            @Override
            public void afterTextChanged(Editable s) {
                messageSuffix.setText(getResources().getString(R.string.ecoScan_messageClosingSignature) +"\n" + s.toString());
            }
        };
    }

    private void setupInitialUI() {
        String defaultMessage = "es ist jetzt ganz einfach, selbst aktiv etwas gegen das Insekten- und Bienensterben und für mehr Biodiversität zu tun. Privaten Gärten und Balkonen kommt dabei eine sehr wichtige Aufgabe zu, z. B. durch die richtige Auswahl der Pflanzen. Im Anhang siehst du den Ökoscan von meinem Garten, der auf www.gardify.de erstellt wurde. Hier kannst Du ganz einfach in der Pflanzensuche insektenfreundliche Pflanzen finden und Pflanzen per Foto bestimmen. Die Anwendung ist übrigens kostenlos.\nVielleicht hast du ja Lust, deinen eigenen Garten auch mal zu checken.\n";
        messageEditText.setText(defaultMessage);
    }

    @Override
    public void onClick(View view) {

        switch (view.getId()) {

            case R.id.button_share_fragment_send:

                SendScanEmail();
                progressBar.setVisibility(View.VISIBLE);

                break;

        }

    }


    /**
     * Check if user entered info (either by authenticating or by entering the data manually)
     * exists. If it doesn't, redirect to LoginFragment.
     */
    @Override
    public void onResume() {
        super.onResume();
        authenticationCheck();
    }

    private void authenticationCheck() {
        if (!PreferencesUtility.getLoggedIn(getActivity())) {
            navigateToFragment(R.id.nav_controller_login, getActivity(), true, null);
        }
    }

    private void SendScanEmail() {
        String scanEmailUrl = APP_URL.ACCOUNT_API +"sendScanMail";
        //String scanEmailUrl = "http://httpbin.org/post";

        if (validateUserInput()) {
            Map<String, String> params = new HashMap<>();
            params.put("email", emailsEditText.getText().toString());
            params.put("fromMail", yourEmailEditText.getText().toString());
            params.put("fromName", fromEditText.getText().toString());
            params.put("toName", toEditText.getText().toString());
            String bodyMessage = messageEditText.getText().toString();
            params.put("emailText", bodyMessage);
            String encodedImage = Base64.encodeToString(byteArrayImage, Base64.NO_WRAP);
            encodedImage = "data:image/jpeg;base64," + encodedImage;
            params.put("image", encodedImage);

            RequestQueueSingleton.getInstance(getContext()).stringRequestFormData(scanEmailUrl, Request.Method.POST, this::ShareSuccess, this::OnError, params);

        }
    }

    private void ShareSuccess(String response) {
        if (response.contains("success")) {
            displayAlertDialog(getContext(), "Dein Ökoscan-Ergebnis wurde erfolgreich versendet!");
            emailsEditText.getText().clear();
            yourEmailEditText.getText().clear();
            fromEditText.getText().clear();
            toEditText.getText().clear();

            progressBar.setVisibility(View.GONE);
        }
    }

    private void OnError(VolleyError error) {
        Log.e(SHARE_RESULT_FRAGMENT, "AddDeviceFragment: " + error.getMessage());

        showErrorDialogNetworkParsed(getContext(), error);
    }

    public Boolean validateUserInput() {

        boolean validFlag = true;

        if (emailsEditText.getText().toString().trim().length() == 0) {
            validFlag = false;
            emailsEditText.setError(getContext().getResources().getString(R.string.all_required));
        }
        if (yourEmailEditText.getText().toString().trim().length() == 0) {
            validFlag = false;
            yourEmailEditText.setError(getContext().getResources().getString(R.string.all_required));
        }
        if (messageEditText.getText().toString().trim().length() == 0) {
            validFlag = false;
            messageEditText.setError(getContext().getResources().getString(R.string.all_required));
        }
        return validFlag;
    }

}