package com.gardify.android.ui.settings;

import android.app.Activity;
import android.content.res.Resources;
import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;

import com.android.volley.Request;
import com.android.volley.VolleyError;
import com.gardify.android.R;
import com.gardify.android.data.account.ApplicationUser;
import com.gardify.android.data.misc.RequestData;
import com.gardify.android.data.settings.Location;
import com.gardify.android.data.settings.ProfileImage;
import com.gardify.android.data.settings.Settings;
import com.gardify.android.data.settings.UserInfo;
import com.gardify.android.generic.GenericDialog;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.ApiUtils;
import com.gardify.android.utils.PreferencesUtility;
import com.gardify.android.utils.RequestQueueSingleton;
import com.gardify.android.utils.RequestType;
import com.google.android.material.switchmaterial.SwitchMaterial;

import org.json.JSONObject;

import java.util.HashMap;

import static com.gardify.android.ui.settings.EditLocalizationBottomSheetDialog.USER_INFO_ARG;
import static com.gardify.android.utils.UiUtils.displayAlertDialog;
import static com.gardify.android.utils.UiUtils.loadImageUsingGlide;
import static com.gardify.android.utils.UiUtils.navigateToFragment;
import static com.gardify.android.utils.UiUtils.setToolbarName;

public class SettingsFragment extends Fragment {

    private TextView textViewUsername, textViewFrostDegree, textViewEmail, textViewPassword, textViewLocalization, textViewUserData;
    private EditText editTextEmail, editTextPassword, editTextLocation, editTextUserData;
    private ProgressBar progressBar;
    private SwitchMaterial frostWarningSwitch, stormWarningSwitch, newsletterSwitch, pushNotificationSwitch, emailNotificationSwitch, colorSchemeSwitch;
    private ImageView imageViewFrostWarningMinus, imageViewFrostWarningPlus, imageViewProfileImage, imageViewContact, imageViewImprint, imageViewPrivacy;
    private CheckBox frostWarningCheckBox, stormWarningCheckBox;
    private Button buttonDelete;
    private String apiUpdateSettings = APP_URL.ACCOUNT_API + "updatesettings";

    private Location userLocation;

    public View onCreateView(@NonNull LayoutInflater inflater,
                             ViewGroup container, Bundle savedInstanceState) {
        View root = inflater.inflate(R.layout.fragment_settings, container, false);

        //initializing views
        init(root);

        if (!PreferencesUtility.getLoggedIn(getActivity())) {
            navigateToFragment(R.id.nav_controller_login, (Activity) getContext(), true, null);

        } else {

            String apiImage = APP_URL.ACCOUNT_API + "profilImg";
            RequestQueueSingleton.getInstance(getContext()).typedRequest(apiImage, this::onSuccessImage, null, ProfileImage.class, new RequestData(RequestType.Settings));

            String apiSettings = APP_URL.ACCOUNT_API + "settings";
            RequestQueueSingleton.getInstance(getContext()).typedRequest(apiSettings, this::onSuccessSettings, null, Settings.class, new RequestData(RequestType.Settings));

            String apiUser = APP_URL.ACCOUNT_API + "userinfo/";
            RequestQueueSingleton.getInstance(getContext()).typedRequest(apiUser, this::onSuccessUser, this::onErrorUser, UserInfo.class, new RequestData(RequestType.UserInfo));

            String apiLocation = APP_URL.USER_GARDEN_API + "location";
            RequestQueueSingleton.getInstance(getContext()).typedRequest(apiLocation, this::onSuccessLocation, this::onErrorUser, Location.class, new RequestData(RequestType.UserInfo));


        }

        setToolbarName(getActivity(), "EINSTELLUNGEN", true);

        imageViewProfileImage.setOnClickListener(v -> {
            EditProfileImageBottomSheetDialog editProfileImageBottomSheet = new EditProfileImageBottomSheetDialog();
            editProfileImageBottomSheet.show((getActivity()).getSupportFragmentManager(),
                    "ModalBottomSheet");
            editProfileImageBottomSheet.setCancelable(false);
        });

        textViewEmail.setOnClickListener(v -> {
            EditEmailBottomSheetDialog editEmailBottomSheet = new EditEmailBottomSheetDialog();
            editEmailBottomSheet.show((getActivity()).getSupportFragmentManager(),
                    "ModalBottomSheet");
            editEmailBottomSheet.setCancelable(false);
        });

        textViewPassword.setOnClickListener(v -> {
            EditPasswordBottomSheetDialog editPasswordBottomSheet = new EditPasswordBottomSheetDialog();
            editPasswordBottomSheet.show((getActivity()).getSupportFragmentManager(),
                    "ModalBottomSheet");
            editPasswordBottomSheet.setCancelable(false);
        });

        textViewLocalization.setOnClickListener(v -> {
            Bundle bundle = new Bundle();
            String userLocationJsonString = ApiUtils.getGsonParser().toJson(userLocation);
            bundle.putString(USER_INFO_ARG, userLocationJsonString);
            EditLocalizationBottomSheetDialog editLocalizationBottomSheet = new EditLocalizationBottomSheetDialog();
            editLocalizationBottomSheet.setArguments(bundle);
            editLocalizationBottomSheet.show((getActivity()).getSupportFragmentManager(),
                    "ModalBottomSheet");
            editLocalizationBottomSheet.setCancelable(false);
        });

        textViewUserData.setOnClickListener(v -> {
            EditUserdataBottomSheetDialog editUserdataBottomSheet = new EditUserdataBottomSheetDialog();
            editUserdataBottomSheet.show((getActivity()).getSupportFragmentManager(),
                    "ModalBottomSheet");
            editUserdataBottomSheet.setCancelable(false);
        });

        imageViewFrostWarningMinus.setOnClickListener(v -> {

            Integer counter = Integer.parseInt(textViewFrostDegree.getText().toString());
            counter--;
            textViewFrostDegree.setText(Integer.toString(counter));
            onSuccessUpdateSettings();
        });

        imageViewFrostWarningPlus.setOnClickListener(v -> {
            Integer counter = Integer.parseInt(textViewFrostDegree.getText().toString());
            counter++;
            textViewFrostDegree.setText(Integer.toString(counter));
            onSuccessUpdateSettings();
        });

        frostWarningSwitch.setOnCheckedChangeListener((buttonView, isChecked) -> onSuccessUpdateSettings());

        stormWarningSwitch.setOnCheckedChangeListener((buttonView, isChecked) -> onSuccessUpdateSettings());

        buttonDelete.setOnClickListener(v -> {
            showDeleteAccountDialog();
        });

        imageViewContact.setOnClickListener(v -> {
            navigateToFragment(R.id.nav_controller_contact, getActivity(), false, null);
        });
        imageViewImprint.setOnClickListener(v -> {
            navigateToFragment(R.id.nav_controller_imprint, getActivity(), false, null);
        });
        imageViewPrivacy.setOnClickListener(v -> {
            navigateToFragment(R.id.nav_controller_privacy_policy, getActivity(), false, null);
        });

        return root;
    }


    public void init(View root) {
        /* finding views block */
        textViewUsername = root.findViewById(R.id.editTextView_settings_username);
        editTextPassword = root.findViewById(R.id.editTextView_settings_password);
        editTextEmail = root.findViewById(R.id.editTextView_settings_email);
        editTextLocation = root.findViewById(R.id.editTextView_settings_address);
        textViewFrostDegree = root.findViewById(R.id.textView_settings_frost_warning_puffer_counter);
        imageViewFrostWarningMinus = root.findViewById(R.id.imageView_settings_frost_warning_puffer_minus);
        imageViewFrostWarningPlus = root.findViewById(R.id.imageView_settings_frost_warning_puffer_plus);
        frostWarningSwitch = root.findViewById(R.id.switch_settings_frost_warning1);
        stormWarningSwitch = root.findViewById(R.id.switch_settings_storm_warning1);
        frostWarningCheckBox = root.findViewById(R.id.checkBox_settings_frost_warning);
        stormWarningCheckBox = root.findViewById(R.id.checkBox_settings_storm_warning);
        newsletterSwitch = root.findViewById(R.id.switch_settings_newsletter);
        imageViewContact = root.findViewById(R.id.imageView_settings_contact);
        imageViewImprint = root.findViewById(R.id.imageView_settings_impressum);
        imageViewPrivacy = root.findViewById(R.id.imageView_settings_privacy);
        textViewEmail = root.findViewById(R.id.textView_settings_email);
        imageViewProfileImage = root.findViewById(R.id.imageView_settings_profileImage);
        textViewPassword = root.findViewById(R.id.textView_settings_password);
        textViewLocalization = root.findViewById(R.id.textView_settings_localization);
        textViewUserData = root.findViewById(R.id.textView_settings_userData);
        buttonDelete = root.findViewById(R.id.button_settings_delete);

        progressBar = root.findViewById(R.id.progressBar_settings);

    }

    public void showDeleteAccountDialog() {
        new GenericDialog.Builder(getContext())
                .setTitle(getResources().getString(R.string.settings_deleteAccount))
                .setTitleAppearance(R.color.text_all_gunmetal, R.dimen.textSize_body_medium)
                .setMessage(getResources().getString(R.string.settings_deleteAccountMessage))
                .addNewButton(R.style.PrimaryWarningButtonStyle,
                        getContext().getResources().getString(R.string.settings_deleteAccount), R.dimen.textSize_body_small, view -> {
                            String userId = PreferencesUtility.getUser(getContext()).getUserId();
                            String accountURL = APP_URL.DELETE_ACCOUNT + userId;
                            Log.d("SettingsFragment", "showDeleteAccountDialog: userId: " + userId);
                            if (userId != null)
                                progressBar.setVisibility(View.VISIBLE);
                                RequestQueueSingleton.getInstance(getContext()).objectRequest(accountURL, Request.Method.DELETE, this::onSuccessDeleteUser, this::onErrorDeleteUser, null);
                        })
                .addNewButton(R.style.ButtonCancel,
                        getContext().getResources().getString(R.string.all_cancel), R.dimen.textSize_body_small, view -> {

                        })
                .setButtonOrientation(LinearLayout.VERTICAL)
                .setCancelable(true)
                .generate();
    }

    private void onSuccessDeleteUser(JSONObject jsonObject) {
        progressBar.setVisibility(View.GONE);
        PreferencesUtility.setLoggedOut(getContext());
        navigateToFragment(R.id.nav_controller_login, getActivity(), true, null);
    }
    private void onErrorDeleteUser(VolleyError volleyError) {
        progressBar.setVisibility(View.GONE);
        displayAlertDialog(getContext(), volleyError.getMessage());
    }

    public void onSuccessImage(ProfileImage profileImage, RequestData requestData) {
        String imageUrl = APP_URL.BASE_ROUTE + profileImage.getImages().get(0).getSrcAttr();
        loadImageUsingGlide(getContext(), imageUrl, imageViewProfileImage);

    }
    private void onSuccessUser(UserInfo model, RequestData data) {
        //logged in user password
        ApplicationUser user = PreferencesUtility.getUser(getContext());
        textViewUsername.setText(model.getUserName());
        editTextEmail.setText(user.getEmail());
    }

    private void onSuccessLocation(Location model, RequestData data) {
        userLocation = model;
        if (!model.getCity().isEmpty()) {
            editTextLocation.setText(model.getCity() + ", " + model.getCountry());
        } else {
            editTextLocation.setText(model.getZip() + ", " + model.getCountry());
        }
    }

    public void onErrorUser(Exception ex, RequestData data) {
        if (getActivity() != null) {
            Resources res = getContext().getResources();
            String network = res.getString(R.string.all_network);
            String anErrorOccurred = res.getString(R.string.all_anErrorOccurred);
            Log.e(network, anErrorOccurred + ex.toString());
            progressBar.setVisibility(View.GONE);
        }
    }

    private void onSuccessSettings(Settings model, RequestData data) {

        textViewFrostDegree.setText(String.valueOf(model.getFrostDegreeBuffer()));
        frostWarningSwitch.setChecked(model.getActiveFrostAlert());
        stormWarningSwitch.setChecked(model.getActiveStormAlert());
        //frostWarningCheckBox.setChecked(model.getAlertByEmail());
        //stormWarningCheckBox.setChecked(model.getAlertByPush());
        //newsletterSwitch.setChecked(model.getActiveNewPlantAlert());
        //pushNotificationSwitch.setChecked(model.getA);

        progressBar.setVisibility(View.GONE);

    }

    private void onSuccessUpdateSettings() {
        ApplicationUser user = PreferencesUtility.getUser(getContext());

        HashMap<String, String> params = new HashMap<>();
        params.put("ActiveFrostAlert", Boolean.toString(frostWarningSwitch.isChecked()));
        params.put("ActiveNewPlantAlert", "false");
        params.put("ActiveStormAlert", Boolean.toString(stormWarningSwitch.isChecked()));
        params.put("AlertByEmail", "false");
        params.put("AlertByPush", "false");
        params.put("FrostDegreeBuffer", textViewFrostDegree.getText().toString());
        params.put("UserId", user.getUserId());

        try {

            JSONObject jsonObject = new JSONObject(params);


            RequestQueueSingleton.getInstance(getContext()).stringRequest(apiUpdateSettings, Request.Method.PUT, this::onSuccessRefresh, null, jsonObject);


        } catch (Exception e) {
            Toast.makeText(getContext(), e.toString(), Toast.LENGTH_LONG).show();
        }
    }

    private void onSuccessRefresh(String s) {

    }

    /**
     * Check if user entered info (either by authenticating or by entering the data manually)
     * exists. If it doesn't, redirect to LoginFragment.
     */
    @Override
    public void onResume() {
        super.onResume();

        if (!PreferencesUtility.getLoggedIn(getActivity())) {
            navigateToFragment(R.id.nav_controller_login, (Activity) getContext(), true, null);

        }
    }

}