package com.gardify.android.ui.settings;

import android.app.Activity;
import android.content.res.Resources;
import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

import androidx.annotation.NonNull;

import com.android.volley.Request;
import com.gardify.android.R;
import com.gardify.android.data.misc.RequestData;
import com.gardify.android.data.settings.UserInfo;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.RequestQueueSingleton;
import com.gardify.android.utils.RequestType;
import com.google.android.material.bottomsheet.BottomSheetDialogFragment;

import org.json.JSONObject;

import java.util.HashMap;

import static com.gardify.android.utils.UiUtils.navigateToFragment;

public class EditUserdataBottomSheetDialog extends BottomSheetDialogFragment implements View.OnClickListener {

    private EditText editTextFirstName;
    private EditText editTextLastName;
    private EditText editTextUserName;
    private EditText editTextStreet;
    private EditText editTextNumber;
    private EditText editTextPlz;
    private EditText editTextCity;
    private EditText editTextCountry;
    private Button buttonSave;
    private Button buttonClose;

    public View onCreateView(@NonNull LayoutInflater layoutInflater, ViewGroup container, Bundle savedInstanceState) {

        View root = layoutInflater.inflate(R.layout.fragment_settings_userdata, container, false);
        init(root);

        String apiUrl = APP_URL.ACCOUNT_API + "userinfo/";
        RequestQueueSingleton.getInstance(getContext()).typedRequest(apiUrl, this::onSuccessUser, this::onErrorUser, UserInfo.class, new RequestData(RequestType.UserInfo));

        return root;
    }

    public void init(View root) {
        editTextFirstName = root.findViewById(R.id.editText_settings_userdata_firstname);
        editTextLastName = root.findViewById(R.id.editText_settings_userdata_lastname);
        editTextUserName = root.findViewById(R.id.editText_settings_userdata_username);
        editTextStreet = root.findViewById(R.id.editText_settings_userdata_street);
        editTextNumber = root.findViewById(R.id.editText_settings_userdata_number);
        editTextPlz = root.findViewById(R.id.editText_settings_userdata_plz);
        editTextCity = root.findViewById(R.id.editText_settings_userdata_city);
        editTextCountry = root.findViewById(R.id.editText_settings_userdata_country);
        buttonSave = root.findViewById(R.id.button_settings_userdata_save);
        buttonClose = root.findViewById(R.id.button_settings_userdata_close);

        buttonSave.setOnClickListener(this);
        buttonClose.setOnClickListener(this);
    }

    @Override
    public void onClick(View v) {
        switch (v.getId()) {
            case R.id.button_settings_userdata_save:
                //String apiUrl = APP_URL.ACCOUNT_API + "data/";
                //RequestQueueSingleton.getInstance(getContext()).typedRequest(apiUrl, this::onSuccessSave, this::onErrorUser, UserInfo.class, new RequestData(RequestType.UserInfo));
                onSuccessSave();
                break;
            case R.id.button_settings_userdata_close:
                dismiss();
                break;
        }
    }

    private void onSuccessUser(UserInfo model, RequestData data) {
        editTextFirstName.setText(model.getFirstName());
        editTextLastName.setText(model.getLastName());
        editTextUserName.setText(model.getUserName());
        editTextStreet.setText(model.getStreet());
        editTextNumber.setText(String.valueOf(model.getHouseNr()));
        editTextPlz.setText(model.getZip());
        editTextCity.setText(model.getCity());
        editTextCountry.setText(model.getCountry());
    }

    public void onErrorUser(Exception ex, RequestData data) {
        if (isVisible()) {
            Resources res = getContext().getResources();
            String network = res.getString(R.string.all_network);
            String anErrorOccurred = res.getString(R.string.all_anErrorOccurred);
            Log.e(network, anErrorOccurred + ex.toString());
        }
    }

    public void onSuccessSave() {
        String textFirstName = editTextFirstName.getText().toString();
        String textLastName = editTextLastName.getText().toString();
        String textUserName = editTextUserName.getText().toString();
        String textStreet = editTextStreet.getText().toString();
        String textNumber = editTextNumber.getText().toString();
        String textPlz = editTextPlz.getText().toString();
        String textCity = editTextCity.getText().toString();
        String textCountry = editTextCountry.getText().toString();

        HashMap<String, String> params = new HashMap<>();
        params.put("FirstName", textFirstName);
        params.put("LastName", textLastName);
        params.put("UserName", textUserName);
        params.put("Street", textStreet);
        params.put("HouseNr", textNumber);
        params.put("Zip", textPlz);
        params.put("City", textCity);
        params.put("Country", textCountry);

        try {

            JSONObject jsonObject = new JSONObject(params);
            String apiUrlPost = APP_URL.ACCOUNT_API + "data";

            RequestQueueSingleton.getInstance(getContext()).stringRequest(apiUrlPost, Request.Method.PUT, this::onSuccessRefresh, null, jsonObject);


        } catch (Exception e) {
            Toast.makeText(getContext(), e.toString(), Toast.LENGTH_LONG).show();
        }

    }

    public void onSuccessRefresh(String s) {
        Toast.makeText(getContext(), "Daten Aktualisiert", Toast.LENGTH_SHORT).show();
        navigateToFragment(R.id.nav_controller_settings, (Activity) getContext(), true, null);

        dismiss();
    }

    @Override
    public int getTheme() {
        return R.style.BaseBottomSheetDialog;
    }
}
