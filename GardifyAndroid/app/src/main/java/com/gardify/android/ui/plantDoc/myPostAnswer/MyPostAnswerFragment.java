package com.gardify.android.ui.plantDoc.myPostAnswer;

import android.content.res.Resources;
import android.os.Bundle;

import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.RecyclerView;

import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ProgressBar;

import com.gardify.android.data.misc.RequestData;
import com.gardify.android.data.plantsDocModel.PlanDocViewModelAnswer;
import com.gardify.android.data.plantsDocModel.PlantDocAnswerList;
import com.gardify.android.R;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.PreferencesUtility;
import com.gardify.android.generic.RecycleViewGenericAdapter;
import com.gardify.android.utils.RequestQueueSingleton;
import com.gardify.android.utils.RequestType;

import java.util.Arrays;
import java.util.HashMap;
import java.util.List;

import static com.gardify.android.utils.UiUtils.configGenericRecyclerView;


public class MyPostAnswerFragment extends Fragment implements RecycleViewGenericAdapter.OnItemClickListener<PlantDocAnswerList> {

    private RecyclerView recyclerView;
    private ProgressBar progressBar;
    private RecycleViewGenericAdapter<PlantDocAnswerList, PlanDocViewModelAnswer, PlanDocViewModelAnswer> adapter;
    private String questionId = null;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        View view = inflater.inflate(R.layout.fragment_general_recycleview, container, false);
        Bundle args = getArguments();
        if (args != null) {
            questionId = args.getString("questionId");
        }
        Initialize(view);

        String apiUrl = APP_URL.PLANT_DOC_ROUTE + questionId + "/getEntry";
        RequestQueueSingleton.getInstance(getContext()).typedRequest(apiUrl, this::onSuccess, this::onError, PlanDocViewModelAnswer.class, new RequestData(RequestType.PflanzenDocModel));


        return view;
    }

    private void Initialize(View root) {
        recyclerView = root.findViewById(R.id.recycler_view_fragment_general);
        progressBar = root.findViewById(R.id.progressbar_fragment_general);
    }

    public void onError(Exception ex, RequestData data) {
        if (isVisible()) {
            Resources res = getResources();
            String network = res.getString(R.string.all_network);
            String anErrorOccurred = res.getString(R.string.all_anErrorOccurred);
            Log.e(network, anErrorOccurred + ex.toString());
            progressBar.setVisibility(View.GONE);
        }
    }

    private void onSuccess(PlanDocViewModelAnswer model, RequestData data) {
        List<PlanDocViewModelAnswer> pflanzenDocList = Arrays.asList(model);
        List<PlantDocAnswerList> answerList = pflanzenDocList.get(0).getPlantDocAnswerList();

        // update shared pref for last seen
        updateLastSeen(answerList);

        adapter = new RecycleViewGenericAdapter<>(
                answerList, R.layout.recycler_view_plant_doc_answer_row_item, pflanzenDocList.get(0), R.layout.recycler_view_plant_doc_answer_header, pflanzenDocList.get(0), R.layout.recycler_view_plant_doc_answer_footer, this, null, null);
        configGenericRecyclerView(getActivity(), recyclerView, adapter, RecyclerView.VERTICAL);

        progressBar.setVisibility(View.GONE);


    }

    private void updateLastSeen(List<PlantDocAnswerList> answerList) {
        HashMap<Integer, Integer> lastSeenDateHashMap = PreferencesUtility.getSeenAnswerDate(getContext());
        if (answerList.size() != 0) {
            lastSeenDateHashMap.replace(Integer.valueOf(questionId), answerList.size() + 1);
            PreferencesUtility.setSeenAnswerDate(getContext(), lastSeenDateHashMap);
        }
    }

    @Override
    public void onItemClick(PlantDocAnswerList position) {

    }

}