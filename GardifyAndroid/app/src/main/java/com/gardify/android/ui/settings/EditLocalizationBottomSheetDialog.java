package com.gardify.android.ui.settings;

import android.app.Activity;
import android.content.res.Resources;
import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Spinner;
import android.widget.Toast;

import androidx.annotation.NonNull;

import com.android.volley.Request;
import com.android.volley.VolleyError;
import com.gardify.android.data.misc.RequestData;
import com.gardify.android.data.settings.Location;
import com.gardify.android.R;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.ApiUtils;
import com.gardify.android.utils.RequestQueueSingleton;
import com.google.android.material.bottomsheet.BottomSheetDialogFragment;

import org.json.JSONObject;

import java.util.HashMap;

import static com.gardify.android.utils.UiUtils.displayAlertDialog;
import static com.gardify.android.utils.UiUtils.showErrorDialogNetworkParsed;
import static com.gardify.android.utils.UiUtils.navigateToFragment;

public class EditLocalizationBottomSheetDialog extends BottomSheetDialogFragment implements View.OnClickListener {

    public static final String USER_INFO_ARG = "USER_INFO";

    private EditText editTextStreet, editTextPlz, editTextCity;
    private Spinner spinnerCountry;
    private Button buttonSave, buttonClose;
    private Location userLocation;
    private String[] countriesArray;
    private ArrayAdapter<String> countryArrayAdapter;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        if (getArguments() != null) {
            String userInfoJson = getArguments().getString(USER_INFO_ARG);
            userLocation = ApiUtils.getGsonParser().fromJson(userInfoJson, Location.class);
        }
    }

    public View onCreateView(@NonNull LayoutInflater layoutInflater, ViewGroup container, Bundle savedInstanceState) {

        View root = layoutInflater.inflate(R.layout.fragment_settings_localization, container, false);
        init(root);

        return root;
    }

    public void init(View root) {
        editTextStreet = root.findViewById(R.id.editText_settings_localization_street);
        editTextPlz = root.findViewById(R.id.editText_settings_localization_plz);
        editTextCity = root.findViewById(R.id.editText_settings_localization_city);
        spinnerCountry = root.findViewById(R.id.spinner_settings_localization_country);
        buttonSave = root.findViewById(R.id.button_settings_localization_save);
        buttonClose = root.findViewById(R.id.button_settings_localization_close);

        buttonSave.setOnClickListener(this);
        buttonClose.setOnClickListener(this);
        countriesArray = getContext().getResources().getStringArray(R.array.countries);
        countryArrayAdapter = new ArrayAdapter<>(getContext(), R.layout.custom_spinner_item, countriesArray);
        spinnerCountry.setAdapter(countryArrayAdapter);

        PopulateExistingFields();
    }

    private void PopulateExistingFields() {
        editTextStreet.setText(userLocation.getStreet());
        editTextPlz.setText(userLocation.getZip());
        editTextCity.setText(userLocation.getCity());
        spinnerCountry.setSelection(countryArrayAdapter.getPosition(userLocation.getCountry()));
    }

    @Override
    public void onClick(View v) {
        switch (v.getId()) {
            case R.id.button_settings_localization_save:
                UpdateUserData();
                break;
            case R.id.button_settings_localization_close:
                onSuccessClose();
                break;
        }
    }

    public void onErrorUser(Exception ex, RequestData data) {
        Resources res = getContext().getResources();
        String network = res.getString(R.string.all_network);
        String anErrorOccurred = res.getString(R.string.all_anErrorOccurred);
        Log.e(network, anErrorOccurred + ex.toString());
    }

    public void UpdateUserData() {
        String textStreet = editTextStreet.getText().toString();
        String textPlz = editTextPlz.getText().toString();
        String textCity = editTextCity.getText().toString();
        String textCountry = spinnerCountry.getSelectedItem().toString();

        HashMap<String, String> params = new HashMap<>();
        params.put("Street", textStreet);
        params.put("Zip", textPlz);
        params.put("City", textCity);
        params.put("Country", textCountry);

        try {

            JSONObject jsonObject = new JSONObject(params);
            String apiUrlPost = APP_URL.USER_GARDEN_API + "location";

            RequestQueueSingleton.getInstance(getContext()).stringRequest(apiUrlPost, Request.Method.PUT, this::onSuccessRefresh, this::OnError, jsonObject);

        } catch (Exception e) {
            Toast.makeText(getContext(), e.toString(), Toast.LENGTH_LONG).show();
        }
    }

    public void onSuccessRefresh(String s) {
        displayAlertDialog(getContext(), "Daten Aktualisiert");
        navigateToFragment(R.id.nav_controller_settings, (Activity) getContext(), true, null);

        dismiss();
    }

    private void OnError(VolleyError error) {
        showErrorDialogNetworkParsed(getContext(), error);
    }

    public void onSuccessClose() {
        dismiss();
    }


    @Override
    public int getTheme() {
        return R.style.BaseBottomSheetDialog;
    }
}
