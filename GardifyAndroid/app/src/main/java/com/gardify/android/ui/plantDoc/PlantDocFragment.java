package com.gardify.android.ui.plantDoc;

import android.app.Activity;
import android.content.res.Resources;
import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ProgressBar;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.RecyclerView;

import com.gardify.android.R;
import com.gardify.android.data.misc.RequestData;
import com.gardify.android.data.plantsDocModel.PlantDocViewModel;
import com.gardify.android.generic.RecycleViewGenericAdapter;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.PreferencesUtility;
import com.gardify.android.utils.RequestQueueSingleton;
import com.gardify.android.utils.RequestType;

import java.util.Arrays;
import java.util.List;
import java.util.stream.Collectors;

import static com.gardify.android.utils.APP_URL.isAndroid;
import static com.gardify.android.utils.UiUtils.configGenericRecyclerView;
import static com.gardify.android.utils.UiUtils.navigateToFragment;
import static com.gardify.android.utils.UiUtils.setupToolbar;


public class PlantDocFragment extends Fragment implements RecycleViewGenericAdapter.OnItemClickListener<PlantDocViewModel>, RecycleViewGenericAdapter.OnTextChangeListener {

    private RecyclerView recyclerViewPlants;
    private ProgressBar progressBar;
    private TextView textViewPlantSearch;
    private RecycleViewGenericAdapter<PlantDocViewModel, PlantDocViewModel, Nullable> adapter;
    private List<PlantDocViewModel> plantDocList;
    private List<PlantDocViewModel> plantDocListOriginal;

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {

        setupToolbar(getActivity(), "PFLANZEN-DOC", R.drawable.gardify_app_icon_pflanzendoc, R.color.toolbar_plantDoc_setupToolbar,true);

        View view = inflater.inflate(R.layout.fragment_general_recycleview, container, false);

        Init(view);

        String apiUrl = APP_URL.PLANT_DOC_ROUTE + "getAllEntry" + isAndroid();
        progressBar.setVisibility(View.VISIBLE);
        RequestQueueSingleton.getInstance(getContext()).typedRequest(apiUrl, this::onSuccess, this::onError, PlantDocViewModel[].class, new RequestData(RequestType.PflanzenDocModel));


        return view;
    }

    private void Init(View view) {
        recyclerViewPlants = view.findViewById(R.id.recycler_view_fragment_general);
        progressBar = view.findViewById(R.id.progressbar_fragment_general);
        textViewPlantSearch = view.findViewById(R.id.textView_plant_doc_header_search);
    }

    public void onError(Exception ex, RequestData data) {
        if (isVisible()) {
            Resources res = getContext().getResources();
            String network = res.getString(R.string.all_network);
            String anErrorOccurred = res.getString(R.string.all_anErrorOccurred);
            Log.e(network, anErrorOccurred + ex.toString());
            progressBar.setVisibility(View.GONE);
        }
    }

    private void onSuccess(PlantDocViewModel[] model, RequestData data) {
        plantDocList = Arrays.asList(model); // recycler item list
        plantDocListOriginal = Arrays.asList(model); // recycler item list

        PlantDocViewModel plantDocHeader = new PlantDocViewModel(); // header view model

        adapter = new RecycleViewGenericAdapter<>(plantDocList, R.layout.recycler_view_plant_doc_row_item, plantDocHeader, R.layout.recycler_view_plant_doc_header,
                null, 0, this, null, null, this);

        configGenericRecyclerView(getActivity(), recyclerViewPlants, adapter, RecyclerView.VERTICAL);
        progressBar.setVisibility(View.GONE);

    }



    @Override
    public void onItemClick(PlantDocViewModel position) {

    }

    @Override
    public void textSearch(String searchText) {
        plantDocList = plantDocListOriginal.stream().filter(p -> p.getThema().toLowerCase().contains(searchText))
                .collect(Collectors.toList());
        adapter.updateList(plantDocList);
        adapter.notifyDataSetChanged();
    }

    @Override
    public void onResume() {
        super.onResume();
        if (!PreferencesUtility.getLoggedIn(getActivity())) {
            navigateToFragment(R.id.nav_controller_login, (Activity) getContext(), true, null);
        }
    }
}