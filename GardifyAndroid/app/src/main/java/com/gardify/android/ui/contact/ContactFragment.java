package com.gardify.android.ui.contact;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;

import com.android.volley.Request;
import com.android.volley.VolleyError;
import com.gardify.android.R;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.PreferencesUtility;
import com.gardify.android.utils.RequestQueueSingleton;

import org.json.JSONObject;

import java.util.HashMap;
import java.util.Map;

import static com.gardify.android.utils.UiUtils.displayAlertDialog;
import static com.gardify.android.utils.UiUtils.showErrorDialogNetworkParsed;
import static com.gardify.android.utils.UiUtils.navigateToFragment;
import static com.gardify.android.utils.UiUtils.setupToolbar;

public class ContactFragment extends Fragment implements View.OnClickListener {

    private EditText emailAddressEdt, messageEdt;
    private TextView subjectLabel;
    private Button sendEmailBtn;
    private Spinner subjectSpinner;

    public View onCreateView(@NonNull LayoutInflater inflater,
                             ViewGroup container, Bundle savedInstanceState) {

        View root = inflater.inflate(R.layout.fragment_contact, container, false);
        setupToolbar(getActivity(), "Kontakt", R.drawable.gardify_icon, R.color.colorPrimary, true);

        authenticationCheck();

        init(root);


        return root;
    }


    public void init(View root) {

        emailAddressEdt = root.findViewById(R.id.editText_fragmentContact_emailAddress);
        messageEdt = root.findViewById(R.id.editText_fragmentContact_message);
        subjectSpinner = root.findViewById(R.id.spinner_fragmentContact_subject);
        sendEmailBtn = root.findViewById(R.id.button_fragmentContact_send);
        subjectLabel = root.findViewById(R.id.textView_fragmentContact_subject_label);

        setupSubjectSpinner();

        sendEmailBtn.setOnClickListener(this);
    }

    private void setupSubjectSpinner() {
        String[] items = {"", "Bestellung aus dem Ausland", "Feedback", "Fehlermeldung"};
        ArrayAdapter<String> countryArrayAdapter = new ArrayAdapter<>(getContext(), R.layout.custom_spinner_item, items); //selected item will look like a spinner set from XML
        subjectSpinner.setAdapter(countryArrayAdapter);
    }


    @Override
    public void onClick(View v) {
        if (v.getId() == sendEmailBtn.getId()) {

            if(validateUserInput()){


                try {

                    Map<String, String> params = new HashMap<>();

                    String subject = subjectSpinner.getSelectedItem().toString();
                    String email = emailAddressEdt.getText().toString();
                    String message = messageEdt.getText().toString();

                    params.put("Subject", subject);
                    params.put("Email", email);
                    params.put("Text", message);

                    JSONObject objParams = new JSONObject(params);

                    String contactUrl = APP_URL.ACCOUNT_API + "contact";
                    RequestQueueSingleton.getInstance(getContext()).objectRequest(contactUrl, Request.Method.POST, this::ContactSuccess, this::ContactError, objParams);


                } catch (Exception e) {
                    Toast.makeText(getContext(), e.toString(), Toast.LENGTH_LONG).show();
                }



            }
        }
    }

    public void ContactSuccess(JSONObject jsonObject) {
        displayAlertDialog(getContext(), "E-Mail gesendet");
        emailAddressEdt.setText("");
        messageEdt.setText("");
    }

    private void ContactError(VolleyError error) {
        showErrorDialogNetworkParsed(getContext(), error);
    }

    public Boolean validateUserInput() {

        boolean validFlag = true;

        if (subjectSpinner.getSelectedItem().toString() == "") {
            validFlag = false;
            subjectLabel.setError(getContext().getResources().getString(R.string.all_required));
        } else if (emailAddressEdt.getText().toString().trim().length() == 0) {
            validFlag = false;
            emailAddressEdt.setError(getContext().getResources().getString(R.string.all_required));
        } else if (messageEdt.getText().toString().trim().length() == 0) {
            validFlag = false;
            messageEdt.setError(getContext().getResources().getString(R.string.all_required));
        }
        return validFlag;
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

}