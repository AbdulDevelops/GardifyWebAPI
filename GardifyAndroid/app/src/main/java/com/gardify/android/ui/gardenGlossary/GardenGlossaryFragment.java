package com.gardify.android.ui.gardenGlossary;

import android.content.res.Resources;
import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ProgressBar;

import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.GridLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.gardify.android.R;
import com.gardify.android.data.gardenGlossary.Example;
import com.gardify.android.data.misc.RequestData;
import com.gardify.android.ui.gardenGlossary.recyclerItem.GardenGlossaryCardItem;
import com.gardify.android.ui.gardenGlossary.recyclerItem.GardenGlossaryFooterCardItem;
import com.gardify.android.ui.gardenGlossary.recyclerItem.HeaderGridListGardenGlossary;
import com.gardify.android.ui.generic.ExpandableHeaderItem;
import com.gardify.android.ui.generic.HeaderItemDecoration;
import com.gardify.android.ui.generic.interfaces.OnExpandableHeaderListener;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.RequestQueueSingleton;
import com.gardify.android.utils.RequestType;
import com.xwray.groupie.ExpandableGroup;
import com.xwray.groupie.GroupAdapter;
import com.xwray.groupie.Section;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

import static com.gardify.android.ui.home.HomeFragment.RECYCLE_VIEW_DEFAULT_SPAN_COUNT;
import static com.gardify.android.utils.UiUtils.setupToolbar;

public class GardenGlossaryFragment extends Fragment {

    private RecyclerView recyclerView;

    private GroupAdapter groupAdapter;
    private ExpandableGroup expandableGroupListen;
    private GridLayoutManager layoutManager;
    private ExpandableHeaderItem expandableHeaderList;
    private ProgressBar progressBar;

    private List<Example> exampleList;
    private int betweenPadding;
    boolean isGrid = false;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        setupToolbar(getActivity(), "Garten Glossar", R.drawable.gardify_app_icon_gartenwissen, R.color.toolbar_gardenGlossary_setupToolbar,true);

        // Inflate the layout for this fragment
        View root = inflater.inflate(R.layout.fragment_general_recycleview, container, false);
        init(root);

        String apiUrl = APP_URL.BASE_ROUTE_API + "lexiconapi/";
        RequestQueueSingleton.getInstance(getContext()).typedRequest(apiUrl, this::onSuccess, this::onError, Example[].class, new RequestData(RequestType.GardenKnowledge));
        progressBar.setVisibility(View.VISIBLE);


        return root;

    }

    public void init(View root) {
        recyclerView = root.findViewById(R.id.recycler_view_fragment_general);
        progressBar = root.findViewById(R.id.progressbar_fragment_general);
    }

    private void onSuccess(Example[] model, RequestData data) {
        exampleList = new ArrayList<>();
        exampleList = Arrays.asList(model);
        groupAdapter = new GroupAdapter();
        groupAdapter.setSpanCount(RECYCLE_VIEW_DEFAULT_SPAN_COUNT);
        layoutManager = new GridLayoutManager(getContext(), groupAdapter.getSpanCount());
        layoutManager.setSpanSizeLookup(groupAdapter.getSpanSizeLookup());
        recyclerView.setLayoutManager(layoutManager);
        betweenPadding = getContext().getResources().getDimensionPixelSize(R.dimen.marginPaddingSize_10sdp);
        recyclerView.addItemDecoration(new HeaderItemDecoration(0, 0));
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
        Section headerGridList = new Section(new HeaderGridListGardenGlossary(R.string.gardenGlossary_empty, onGridViewClickListener));
        groupAdapter.add(headerGridList);
        if (grid) {
            Section updateWithPayloadSection = new Section();
            for (Example example : exampleList) {
                updateWithPayloadSection.add(new GardenGlossaryCardItem(getContext(), example, ""));
            }

            groupAdapter.add(updateWithPayloadSection);
        } else {
            for (Example example : exampleList) {
                expandableHeaderList = new ExpandableHeaderItem(getContext(), 0, R.color.expandableHeader_all_white, example.getName(), false, onExpandableHeaderListener);
                expandableGroupListen = new ExpandableGroup(expandableHeaderList);
                expandableGroupListen.add(new GardenGlossaryCardItem(getContext(), example, ""));
                groupAdapter.add(expandableGroupListen);
            }
        }
        Section footer = new Section(new GardenGlossaryFooterCardItem(getContext()));
        groupAdapter.add(footer);
        progressBar.setVisibility(View.GONE);
    }

    private OnExpandableHeaderListener onExpandableHeaderListener = (stringId) -> {
        // Pretend to make a network request
        switch (stringId) {
            case R.string.all_ecoElements:

                break;
        }
    };
    private HeaderGridListGardenGlossary.onGridViewClickListener onGridViewClickListener = (grid) -> {
        // Pretend to make a network request
        isGrid = grid;

        populateAdapter(grid);
    };

}