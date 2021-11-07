package com.gardify.android.ui.myGarden;


import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;
import androidx.lifecycle.ViewModelProviders;

import com.android.volley.Request;
import com.android.volley.VolleyError;
import com.gardify.android.data.account.UserMainGarden;
import com.gardify.android.data.myGarden.UserGarden.UserGarden;
import com.gardify.android.R;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.ApiUtils;
import com.gardify.android.utils.PreferencesUtility;
import com.gardify.android.utils.RequestQueueSingleton;
import com.gardify.android.utils.UiUtils;

import org.json.JSONObject;

import java.util.HashMap;
import java.util.Map;

public class EditUserGardenFragment extends Fragment {

    private static final String EXPANDED_MENU_STATE = "EXPANDED_MENU";
    private static final String USER_GARDEN_DATA = "USER_GARDEN_DATA";
    private static final String ARG_FRAGMENT_NAME = "FRAGMENT_NAME";

    private static final String TAG = "EditUserGarden";
    Button saveBtn, cancelBtn;
    EditText nameEdtText, descEditText;
    TextView headerTxt;
    UserGarden userGarden;
    boolean isEdit = false;

    //argument value
    String userGardenJsonString = null;
    int fragmentNameArg = 0;

    public View onCreateView(@NonNull LayoutInflater inflater,
                             ViewGroup container, Bundle savedInstanceState) {

        View root = inflater.inflate(R.layout.fragment_my_garden_edit_user_garden, container, false);
        init(root);

        // get clicked model object
        Bundle args = getArguments();
        if (args != null) {
            userGardenJsonString = args.getString(USER_GARDEN_DATA);
            fragmentNameArg = args.getInt(ARG_FRAGMENT_NAME);

            if (userGardenJsonString != null) {
                userGarden = ApiUtils.getGsonParser().fromJson(userGardenJsonString, UserGarden.class);
                // update UI
                nameEdtText.setText(userGarden.getName());
                headerTxt.setText(userGarden.getName() + " " + getResources().getString(R.string.all_edit).toLowerCase());
                if (userGarden.getDescription().equals("No Description"))
                    descEditText.setText(getResources().getString(R.string.myGarden_noDescription));
                else descEditText.setText(userGarden.getDescription());
                isEdit = true;
            }
        }

        if (!isEdit) {
            headerTxt.setText(getResources().getString(R.string.myGarden_addNewList));
        }

        saveBtn.setOnClickListener(view -> {
            String updateUserGardenUrl;
            if (isEdit) {
                updateUserGardenUrl = APP_URL.USER_LIST_API + "updatelist";
            } else {
                updateUserGardenUrl = APP_URL.USER_LIST_API + "create";
            }

            Map<String, String> params = new HashMap<>();

            params.put("Name", nameEdtText.getText().toString());
            params.put("Description", descEditText.getText().toString());

            if (isEdit) {
                params.put("Id", userGarden.getId().toString());
                params.put("ListSelected", userGarden.getListSelected().toString());
                params.put("GardenId", userGarden.getGardenId().toString());
            } else {
                UserMainGarden userMainGarden = PreferencesUtility.getUserMainGarden(getContext());
                params.put("GardenId", userMainGarden.getId().toString());
            }

            JSONObject jsonObject = new JSONObject(params);

            if (isEdit) {
                RequestQueueSingleton.getInstance(getContext()).stringRequest(updateUserGardenUrl, Request.Method.PUT, this::onSuccessUpdateGarden, this::onErrorUpdateGarden, jsonObject);

            } else {
                RequestQueueSingleton.getInstance(getContext()).stringRequest(updateUserGardenUrl, Request.Method.POST, this::onSuccessUpdateGarden, this::onErrorUpdateGarden, jsonObject);

            }

        });

        cancelBtn.setOnClickListener(view -> {
            getActivity().onBackPressed();

        });

        return root;
    }

    private void onErrorUpdateGarden(VolleyError volleyError) {
        Toast.makeText(getContext(), volleyError.getMessage(), Toast.LENGTH_SHORT).show();
    }

    private void onSuccessUpdateGarden(String stringResponse) {
        UiUtils.displayAlertDialog(getContext(), getResources().getString(R.string.myGarden_savedList));
        Log.e(TAG, "" + stringResponse);

        if (fragmentNameArg != R.string.all_saveToGarden)
            goBackToMyGarden();
        else
            goBackToSaveToGarden();

    }
    MyGardenPersistDataViewModel persistDataViewModel;

    private void goBackToMyGarden() {
        //pop back stack
        persistDataViewModel.setMyGardenState(R.string.myGarden_addNewList);
        getActivity().onBackPressed();
    }

    private void goBackToSaveToGarden() {
        getActivity().onBackPressed();
    }

    private void init(View root) {

        saveBtn = root.findViewById(R.id.button_my_garden_edit_user_garden_save);
        cancelBtn = root.findViewById(R.id.button_my_garden_edit_user_garden_cancel);
        nameEdtText = root.findViewById(R.id.editText_my_garden_edit_user_garden_name);
        descEditText = root.findViewById(R.id.editText_my_garden_edit_user_garden_desc);
        headerTxt = root.findViewById(R.id.my_garden_edit_header);
    }

    @Override
    public void onActivityCreated(@Nullable Bundle savedInstanceState) {
        super.onActivityCreated(savedInstanceState);
        persistDataViewModel = ViewModelProviders.of(getActivity()).get(MyGardenPersistDataViewModel.class);
    }
}
