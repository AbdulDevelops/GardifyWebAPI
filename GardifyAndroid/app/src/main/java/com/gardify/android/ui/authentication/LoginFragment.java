package com.gardify.android.ui.authentication;

import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.GridLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.android.volley.Request;
import com.android.volley.VolleyError;
import com.gardify.android.R;
import com.gardify.android.ui.authentication.interfaces.OnUpdate;
import com.gardify.android.ui.authentication.recyclerItems.AuthGuidedTourRow;
import com.gardify.android.ui.authentication.recyclerItems.LoginRow;
import com.gardify.android.ui.authentication.recyclerItems.RegisterRow;
import com.gardify.android.ui.generic.ExpandableHeaderItem;
import com.gardify.android.ui.generic.HeaderItemDecoration;
import com.gardify.android.ui.generic.interfaces.OnExpandableHeaderListener;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.PreferencesUtility;
import com.gardify.android.utils.RequestQueueSingleton;
import com.xwray.groupie.ExpandableGroup;
import com.xwray.groupie.GroupAdapter;

import org.json.JSONObject;

import static com.gardify.android.ui.home.HomeFragment.RECYCLE_VIEW_DEFAULT_SPAN_COUNT;
import static com.gardify.android.utils.APP_URL.isAndroid;
import static com.gardify.android.utils.UiUtils.navigateToFragment;


public class LoginFragment extends Fragment {

    private static final String ACTION_ARGUMENT = "action_argument";
    boolean splashRegisterClick;

    public View onCreateView(@NonNull LayoutInflater inflater,
                             ViewGroup container, Bundle savedInstanceState) {

        View root = inflater.inflate(R.layout.fragment_general_recycleview, container, false);

        init(root);

        return root;
    }
    
    public OnUpdate onUpdate = stringID -> {
        switch (stringID) {
            case R.string.all_login:
                Log.d("login", "login header expanded");

                String mainGardenUrl = APP_URL.USER_GARDEN_API + "main" + isAndroid();
                RequestQueueSingleton.getInstance(getContext()).objectRequest(mainGardenUrl, Request.Method.GET, this::MainGardenSuccess, this::MainGardenError, null);

                break;
            case R.string.all_guidedTour:

                //Passing arguments
                Bundle args = new Bundle();
                args.putInt(ACTION_ARGUMENT, R.string.all_guidedTour);
                // change fragment
                navigateToFragment(R.id.nav_controller_home, getActivity(), true, args);

                break;
        }

    };
    ExpandableGroup expandableGroupLogin, expandableGroupRegister, expandableGroupGuidedTour;
    private GroupAdapter groupAdapter;
    private final OnExpandableHeaderListener onExpandableHeaderListener = (stringId) -> {
        switch (stringId) {
            case R.string.all_login:
                if (expandableGroupRegister.isExpanded())
                    expandableGroupRegister.onToggleExpanded();
                if (expandableGroupGuidedTour.isExpanded())
                    expandableGroupGuidedTour.onToggleExpanded();
                break;
            case R.string.registerLogin_newRegistration:
                if (expandableGroupLogin.isExpanded())
                    expandableGroupLogin.onToggleExpanded();
                if (expandableGroupGuidedTour.isExpanded())
                    expandableGroupGuidedTour.onToggleExpanded();
                break;
            case R.string.all_guidedTour:
                if (expandableGroupLogin.isExpanded())
                    expandableGroupLogin.onToggleExpanded();
                if (expandableGroupRegister.isExpanded())
                    expandableGroupRegister.onToggleExpanded();
                break;

        }
    };

    public void init(View root) {
        /* finding views block */
        RecyclerView recyclerView = root.findViewById(R.id.recycler_view_fragment_general);

        Bundle splashBundle = this.getArguments();
        if (splashBundle != null)
            splashRegisterClick = splashBundle.getBoolean("help");

        // initialize groupAdapter
        groupAdapter = new GroupAdapter();
        groupAdapter.setSpanCount(RECYCLE_VIEW_DEFAULT_SPAN_COUNT);
        GridLayoutManager layoutManager = new GridLayoutManager(getContext(), groupAdapter.getSpanCount());
        layoutManager.setSpanSizeLookup(groupAdapter.getSpanSizeLookup());
        recyclerView.setLayoutManager(layoutManager);
        recyclerView.addItemDecoration(new HeaderItemDecoration(0, 0));
        populateAdapter();
        recyclerView.setAdapter(groupAdapter);
    }

    private void populateAdapter() {

        // Expandable login
        ExpandableHeaderItem expandableHeaderLogin = new ExpandableHeaderItem(getContext(), R.color.text_all_white, R.color.expandableHeader_login_login, R.string.all_login, 0, onExpandableHeaderListener);
        expandableGroupLogin = new ExpandableGroup(expandableHeaderLogin);
        expandableGroupLogin.add(new LoginRow(getContext(), R.color.text_all_gunmetal, R.string.all_login, onUpdate));
        // expand Login row when Login Button in RegisterSplashActivity is clicked
        if (!splashRegisterClick)
            expandableGroupLogin.onToggleExpanded();
        groupAdapter.add(expandableGroupLogin);

        // Expandable Register
        ExpandableHeaderItem expandableHeaderRegister = new ExpandableHeaderItem(getContext(), R.color.text_all_white, R.color.expandableHeader_login_register, R.string.registerLogin_newRegistration, 0, onExpandableHeaderListener);
        expandableGroupRegister = new ExpandableGroup(expandableHeaderRegister);
        expandableGroupRegister.add(new RegisterRow(getContext(), R.color.text_all_gunmetal, R.string.all_login, onUpdate));
        // expand Register row when Register Button in RegisterSplashActivity is clicked
        if (splashRegisterClick)
            expandableGroupRegister.onToggleExpanded();
        groupAdapter.add(expandableGroupRegister);

        // Expandable Register
        ExpandableHeaderItem expandableGuidedTour = new ExpandableHeaderItem(getContext(), R.color.text_all_white, R.color.expandableHeader_login_guidedTour, R.string.all_guidedTour, R.string.registerLogin_withoutRegistration, onExpandableHeaderListener);
        expandableGroupGuidedTour = new ExpandableGroup(expandableGuidedTour);
        expandableGroupGuidedTour.add(new AuthGuidedTourRow(R.string.all_guidedTour, onUpdate));
        groupAdapter.add(expandableGroupGuidedTour);

    }

    private void MainGardenSuccess(JSONObject jsonObject) {
        PreferencesUtility.setUserMainGarden(getContext(), jsonObject.toString());

        //change fragment
        navigateToFragment(R.id.nav_controller_home, getActivity(), true, null);

    }

    private void MainGardenError(VolleyError volleyError) {
        Log.d(this.getClass().getSimpleName(), volleyError.toString());
    }

}