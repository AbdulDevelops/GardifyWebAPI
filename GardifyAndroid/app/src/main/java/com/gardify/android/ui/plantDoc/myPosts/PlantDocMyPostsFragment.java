package com.gardify.android.ui.plantDoc.myPosts;

import android.app.Activity;
import android.content.res.Resources;
import android.os.Bundle;

import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.GridLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ProgressBar;

import com.gardify.android.data.misc.RequestData;
import com.gardify.android.data.plantsDocModel.PlantDocViewModel;
import com.gardify.android.R;
import com.gardify.android.ui.generic.interfaces.OnExpandableHeaderListener;
import com.gardify.android.ui.generic.ExpandableHeaderItem;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.PreferencesUtility;
import com.gardify.android.utils.RequestQueueSingleton;
import com.gardify.android.utils.RequestType;
import com.xwray.groupie.ExpandableGroup;
import com.xwray.groupie.GroupAdapter;
import com.xwray.groupie.Section;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.List;

import static com.gardify.android.ui.home.HomeFragment.RECYCLE_VIEW_DEFAULT_SPAN_COUNT;
import static com.gardify.android.utils.UiUtils.navigateToFragment;


public class PlantDocMyPostsFragment extends Fragment {

    private RecyclerView recyclerView;
    private ProgressBar progressBar;

    private GroupAdapter groupAdapter;
    private ExpandableGroup expandableGroupListen;
    private GridLayoutManager layoutManager;
    ExpandableHeaderItem expandableHeaderList;
    private List<PlantDocViewModel> plantDocModelsList;
    boolean isGrid = false;
    boolean isInfoVisible = false;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        View root = inflater.inflate(R.layout.fragment_general_recycleview, container, false);

        Initialize(root);

        String apiUrl = APP_URL.PLANT_DOC_ROUTE + "getCurrentUserPosts";
        RequestQueueSingleton.getInstance(getContext()).typedRequest(apiUrl, this::onSuccess, this::onError, PlantDocViewModel[].class, new RequestData(RequestType.PflanzenDocModel));
        progressBar.setVisibility(View.VISIBLE);

        return root;
    }

    private void Initialize(View root) {
        recyclerView = root.findViewById(R.id.recycler_view_fragment_general);
        progressBar = root.findViewById(R.id.progressbar_fragment_general);
    }

    private void onSuccess(PlantDocViewModel[] model, RequestData data) {
        progressBar.setVisibility(View.GONE);

        plantDocModelsList = new ArrayList<>();
        plantDocModelsList = Arrays.asList(model);
        groupAdapter = new GroupAdapter();
        groupAdapter.setSpanCount(RECYCLE_VIEW_DEFAULT_SPAN_COUNT);
        layoutManager = new GridLayoutManager(getContext(), groupAdapter.getSpanCount());
        layoutManager.setSpanSizeLookup(groupAdapter.getSpanSizeLookup());
        recyclerView.setLayoutManager(layoutManager);
        populateAdapter(isGrid);
        recyclerView.setAdapter(groupAdapter);
    }

    public void onError(Exception ex, RequestData data) {
        if (getActivity() != null) {
            Resources res = getActivity().getResources();
            String network = res.getString(R.string.all_network);
            String anErrorOccurred = res.getString(R.string.all_anErrorOccurred);
            Log.e(network, anErrorOccurred + ex.toString());

        }
    }

    private void populateAdapter(boolean grid) {
        groupAdapter.clear();
        // Expandable group Listen
        Section headerGridList = new Section(new HeaderGridListPlantDocMyPosts(R.string.plantDoc_myPosts, onGridViewClickListener));
        groupAdapter.add(headerGridList);

        updateSeenFromSharedPreferencesList(grid);


    }
    private Section updateWithPayloadSection;

    private void updateSeenFromSharedPreferencesList(boolean grid) {
        updateWithPayloadSection = new Section();
        HashMap<Integer, Integer> lastSeenDateHashMap = PreferencesUtility.getSeenAnswerDate(getContext());
        if (lastSeenDateHashMap!=null && lastSeenDateHashMap.size()>0) {
            for (PlantDocViewModel plantDocQuestion : plantDocModelsList) {
                int savedTotalCount = lastSeenDateHashMap.get(plantDocQuestion.getQuestionId()) ==null ? 0 : lastSeenDateHashMap.get(plantDocQuestion.getQuestionId());
                if (plantDocQuestion.getTotalAnswers() > savedTotalCount && plantDocQuestion.getTotalAnswers() > 0) {
                    isInfoVisible = true;
                } else {
                    isInfoVisible = false;
                }
                populateExpandableList(plantDocQuestion,grid, isInfoVisible);
            }

        } else {
            lastSeenDateHashMap = new HashMap<>();
            for (PlantDocViewModel plantDocQuestion : plantDocModelsList) {
                lastSeenDateHashMap.put(plantDocQuestion.getQuestionId(), plantDocQuestion.getTotalAnswers());

                populateExpandableList(plantDocQuestion, grid, true);
            }
            PreferencesUtility.setSeenAnswerDate(getContext(), lastSeenDateHashMap);
        }
        groupAdapter.add(updateWithPayloadSection);
    }

    private void populateExpandableList(PlantDocViewModel plantDocQuestion, boolean grid, boolean isInfoVisible) {
        if(grid){
            updateWithPayloadSection.add(new PlantDocMyPostsCardItem(getContext(), plantDocQuestion, "", isInfoVisible));
        }else {
            expandableHeaderList = new ExpandableHeaderItem(getContext(), R.color.text_all_gunmetal, R.color.expandableHeader_all_white, plantDocQuestion.getThema(), isInfoVisible, onExpandableHeaderListener);
            expandableGroupListen = new ExpandableGroup(expandableHeaderList);
            expandableGroupListen.add(new PlantDocMyPostsCardItem(getContext(), plantDocQuestion, "", isInfoVisible));
            groupAdapter.add(expandableGroupListen);
        }
    }

    private OnExpandableHeaderListener onExpandableHeaderListener = (stringId) -> {
        // Pretend to make a network request
        switch (stringId) {
            case R.string.all_ecoElements:

                break;
        }
    };

    private HeaderGridListPlantDocMyPosts.onGridViewClickListener onGridViewClickListener = (grid) -> {
        // Pretend to make a network request
        isGrid = grid;

        populateAdapter(grid);
    };

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
            navigateToFragment(R.id.nav_controller_login,(Activity) getContext(), true, null);
        }
    }
}