package com.gardify.android.ui.warning;

import android.app.Activity;
import android.content.Context;
import android.graphics.drawable.ColorDrawable;
import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.LinearLayout;
import android.widget.PopupWindow;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.GridLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.android.volley.Request;
import com.gardify.android.R;
import com.gardify.android.data.misc.RequestData;
import com.gardify.android.data.warning.Warning;
import com.gardify.android.generic.GenericDialog;
import com.gardify.android.ui.MainActivity;
import com.gardify.android.ui.generic.HeaderItemDecoration;
import com.gardify.android.ui.generic.recyclerItem.CardItem;
import com.gardify.android.ui.generic.recyclerItem.GenericButton;
import com.gardify.android.ui.generic.recyclerItem.HeaderTitle;
import com.gardify.android.ui.warning.recyclerItem.ExpandableToggleHeaderItem;
import com.gardify.android.ui.warning.recyclerItem.HeaderIcons;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.PreferencesUtility;
import com.gardify.android.utils.RequestQueueSingleton;
import com.gardify.android.utils.RequestType;
import com.google.android.material.switchmaterial.SwitchMaterial;
import com.xwray.groupie.ExpandableGroup;
import com.xwray.groupie.GroupAdapter;
import com.xwray.groupie.Section;

import org.json.JSONObject;

import java.util.Arrays;
import java.util.List;

import static com.gardify.android.ui.home.HomeFragment.RECYCLE_VIEW_DEFAULT_SPAN_COUNT;
import static com.gardify.android.utils.UiUtils.navigateToFragment;
import static com.gardify.android.utils.UiUtils.setupToolbar;

public class WarningFragment extends Fragment {


    public static final int OBJECT_TYPE_PLANT = 4;
    public static final int OBJECT_TYPE_DEVICE = 19;
    private static final String TAG = "Warning Fragment";
    private RecyclerView recyclerView;
    private ProgressBar progressBar;
    private GroupAdapter groupAdapter;
    private GridLayoutManager layoutManager;
    private List<Warning> warningsList;
    private boolean isFrostWarning = true;

    public View onCreateView(@NonNull LayoutInflater inflater,
                             ViewGroup container, Bundle savedInstanceState) {

        View root = inflater.inflate(R.layout.fragment_general_recycleview, container, false);
        setupToolbar(getActivity(), "WARNUNGEN", R.drawable.gardify_app_icon_news, R.color.colorPrimary, true);

        initializeViews(root);
        initializeGroupAdapter();

        getWarningsListFromApi();

        return root;
    }

    private void getWarningsListFromApi() {
        String warningApiUrl = APP_URL.WARNING_API + "warnings";
        RequestQueueSingleton.getInstance(getContext()).typedRequest(warningApiUrl, this::onSuccessWarning, null, Warning[].class, new RequestData(RequestType.Warning));
        progressBar.setVisibility(View.VISIBLE);
    }

    public void initializeViews(View root) {
        /* finding views block */
        progressBar = root.findViewById(R.id.progressbar_fragment_general);
        recyclerView = root.findViewById(R.id.recycler_view_fragment_general);
    }

    private void onSuccessWarning(Warning[] warning, RequestData data) {

        warningsList = Arrays.asList(warning);
        progressBar.setVisibility(View.GONE);

        populateWarningsList();
    }

    private void initializeGroupAdapter() {
        groupAdapter = new GroupAdapter();
        groupAdapter.setSpanCount(RECYCLE_VIEW_DEFAULT_SPAN_COUNT);
        layoutManager = new GridLayoutManager(getContext(), groupAdapter.getSpanCount());
        layoutManager.setSpanSizeLookup(groupAdapter.getSpanSizeLookup());
        recyclerView.setLayoutManager(layoutManager);
        recyclerView.addItemDecoration(new HeaderItemDecoration(0, 0));
        populateAdapter();
        recyclerView.setAdapter(groupAdapter);
    }

    private ExpandableToggleHeaderItem expandableHeaderWarningPlants, expandableHeaderWarningDevices;
    private ExpandableGroup expandableGroupWarningPlants, expandableGroupWarningDevices;
    private GenericButton genericButton;

    private void populateAdapter() {

        Section headerIcons = new Section(new HeaderIcons("", getContext(), R.drawable.gardify_info_icon, R.drawable.gardify_settings_icon, onIconClickListener));
        groupAdapter.add(headerIcons);

        genericButton = new GenericButton.Builder(getContext())
                .addNewButton(R.style.PrimaryButtonStyle, R.string.all_frostWarning, R.dimen.textSize_body_xsmall, (GenericButton.HeaderButtonClickListener) (buttonString, view) -> {
                    headerButtonClickListener.onClick(buttonString, view);
                    genericButton.setSelectedButton(view);
                    genericButton.notifyChanged();
                })
                .addNewButton(R.style.PrimaryButtonStyle, R.string.all_stormWarning, R.dimen.textSize_body_xsmall, (GenericButton.HeaderButtonClickListener) (buttonString, view) -> {
                    headerButtonClickListener.onClick(buttonString, view);
                    genericButton.setSelectedButton(view);
                    genericButton.notifyChanged();
                })
                .setButtonColorState(R.color.button_background_color_state, R.color.button_text_color_state)
                .build();

        Section headerButtons = new Section(genericButton);
        groupAdapter.add(headerButtons);

    }

    private ExpandableToggleHeaderItem.OnWarningClickListener onWarningClickListener = (newWarningCount) -> {
        TextView warningCounter = getActivity().findViewById(R.id.warning_count_appbar_main) ;
        warningCounter.setText(String.valueOf(newWarningCount));
    };

    Section warningPlantsListSection;

    private void populateWarningsList() {
        warningPlantsListSection = new Section();
        Section headerPlants = new Section(new HeaderTitle(R.string.all_plants));
        warningPlantsListSection.add(headerPlants);

        // Expandable group plants
        for (Warning warning : warningsList) {
            String plantName = warning.getRelatedObjectName();
            if (warning.getObjectType() == OBJECT_TYPE_PLANT) { // 4 plant warning
                expandableHeaderWarningPlants = new ExpandableToggleHeaderItem(getContext(), 0, R.color.expandableHeader_all_white, plantName, warning, isFrostWarning, onExpandableHeaderListener, onExpandableHeaderSwitchListener, onWarningClickListener);
                expandableGroupWarningPlants = new ExpandableGroup(expandableHeaderWarningPlants);
                if (warning.getText() != null) {
                    expandableGroupWarningPlants.add(new CardItem(getContext(), R.color.expandableGroup_all_white, warning.getText()));
                }
                warningPlantsListSection.add(expandableGroupWarningPlants);
            }
        }

        Section headerDevices = new Section(new HeaderTitle(R.string.all_devices));
        warningPlantsListSection.add(headerDevices);

        // Expandable group devices
        for (Warning warning : warningsList) {
            if (warning.getObjectType() == OBJECT_TYPE_DEVICE) { // 19 devices warning
                expandableHeaderWarningDevices = new ExpandableToggleHeaderItem(getContext(), 0, R.color.expandableHeader_all_white, warning.getRelatedObjectName(), warning, isFrostWarning, onExpandableHeaderListener, onExpandableHeaderSwitchListener, onWarningClickListener);
                expandableGroupWarningDevices = new ExpandableGroup(expandableHeaderWarningDevices);
                if (warning.getText() != null) {
                    expandableGroupWarningDevices.add(new CardItem(getContext(), R.color.expandableGroup_all_white, warning.getText()));
                }
                warningPlantsListSection.add(expandableGroupWarningDevices);
            }
        }
        groupAdapter.add(warningPlantsListSection);
    }

    ExpandableToggleHeaderItem.OnExpandableHeaderSwitchListener onExpandableHeaderSwitchListener = (id, isChecked) -> {
        String updateUrl;
        if (isFrostWarning) {
            updateUrl = APP_URL.USER_PLANT_API + "updateUserPlantNotification/" + id + "/true";
        } else {
            updateUrl = APP_URL.USER_PLANT_API + "updateUserPlantNotification/" + id + "/false";
        }
        RequestQueueSingleton.getInstance(getContext()).objectRequest(updateUrl, Request.Method.GET, this::onSuccessSwitchChange, null, null);

    };

    private void onSuccessSwitchChange(JSONObject jsonObject) {
        if (isVisible()) {
            Toast.makeText(getContext(), "Warnung wurde erfolgreich aktualisiert", Toast.LENGTH_SHORT).show();

            //update actionbar counters
            ((MainActivity) getActivity()).updateActionBarCounters();

        }
    }

    private HeaderIcons.OnIconClickListener onIconClickListener = (viewBinding, view) -> {

        if (view == viewBinding.imageOne) {
            displayWarningInfoOptionMenu(view);
        } else if (view == viewBinding.imageTwo) {
            navigateToFragment(R.id.nav_controller_settings, getActivity(), false, null);
        } else if (view == viewBinding.cardViewResetWarning) {
            showResetWarningDialog();
        }
    };

    private void showResetWarningDialog() {
        new GenericDialog.Builder(getContext())
                .setTitle(getString(R.string.all_resetWarnings))
                .setTitleAppearance(R.color.text_all_gunmetal, R.dimen.textSize_body_medium)
                .setMessage(getResources().getString(R.string.all_resetWarningsMessage))
                .setMessageAppearance(R.color.text_all_riverBed, R.dimen.textSize_body_small)
                .addNewButton(R.style.PrimaryButtonStyle,
                        getResources().getString(R.string.all_confirm), R.dimen.textSize_body_medium, v -> {

                            String resetUrl = APP_URL.WARNING_API + "reset";
                            RequestQueueSingleton.getInstance(getContext()).stringRequest(resetUrl, Request.Method.GET, this::onSuccessReset, null, null);

                        })
                .addNewButton(R.style.SecondaryButtonStyle,
                        getResources().getString(R.string.all_back), R.dimen.textSize_body_medium, v ->

                                Log.e(TAG, "dialog dismissed."))

                .setButtonOrientation(LinearLayout.HORIZONTAL)
                .setCancelable(true)
                .generate();
    }

    private void onSuccessReset(String s) {
        // Reload warnings
        if (warningPlantsListSection != null) {
            groupAdapter.remove(warningPlantsListSection);
        }
        getWarningsListFromApi();

    }

    private GenericButton.HeaderButtonClickListener headerButtonClickListener = (stringId, view) -> {

        switch (stringId) {
            case R.string.all_frostWarning:
                isFrostWarning = true;
                break;
            case R.string.all_stormWarning:
                isFrostWarning = false;
                break;
        }

        if (warningPlantsListSection != null) {
            groupAdapter.remove(warningPlantsListSection);
        }
        getWarningsListFromApi();
    };

    public void displayWarningInfoOptionMenu(View v) {


        LayoutInflater layoutInflater = (LayoutInflater) getContext().getSystemService(Context.LAYOUT_INFLATER_SERVICE);
        final View popupView = layoutInflater.inflate(R.layout.popup_menu_warning_frag_info, null);

        PopupWindow popupWindow = new PopupWindow(
                popupView,
                ViewGroup.LayoutParams.WRAP_CONTENT,
                ViewGroup.LayoutParams.WRAP_CONTENT);

        popupWindow.setBackgroundDrawable(new ColorDrawable(android.graphics.Color.TRANSPARENT));
        popupWindow.setElevation(20);
        popupWindow.setOutsideTouchable(true);

        SwitchMaterial turnOffWarning = popupView.findViewById(R.id.switch_turn_off_warning);
        SwitchMaterial activateWarning = popupView.findViewById(R.id.switch_activate_warning);

        turnOffWarning.setOnClickListener(v1 -> Toast.makeText(getContext(), "clicked turn off", Toast.LENGTH_SHORT).show());
        activateWarning.setOnClickListener(v1 -> Toast.makeText(getContext(), "activate clicked", Toast.LENGTH_SHORT).show());

        popupWindow.showAsDropDown(v, -20, 0);
    }

    private ExpandableToggleHeaderItem.OnExpandableHeaderListener onExpandableHeaderListener = (stringId) -> {
        // Pretend to make a network request
        switch (stringId) {
            case R.string.all_ecoElements:

                break;
        }
    };

    /**
     * Check if user entered info (either by authenticating or by entering the data manually)
     * exists. If it doesn't, redirect to LoginFragment.
     **/

    @Override
    public void onResume() {
        super.onResume();

        if (!PreferencesUtility.getLoggedIn(getActivity())) {
            navigateToFragment(R.id.nav_controller_login, (Activity) getContext(), true, null);
        }
    }
}