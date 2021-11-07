package com.gardify.android.ui.plantDoc.answer;

import android.content.res.Resources;
import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ProgressBar;

import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.RecyclerView;

import com.gardify.android.data.misc.RequestData;
import com.gardify.android.data.plantsDocModel.PlanDocViewModelAnswer;
import com.gardify.android.data.plantsDocModel.PlantDocAnswerList;
import com.gardify.android.R;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.generic.RecycleViewGenericAdapter;
import com.gardify.android.utils.RequestQueueSingleton;
import com.gardify.android.utils.RequestType;

import java.util.Arrays;
import java.util.List;

import static com.gardify.android.utils.UiUtils.configGenericRecyclerView;


public class AnswerFragment extends Fragment implements RecycleViewGenericAdapter.OnItemClickListener<PlantDocAnswerList>{

    RecyclerView recyclerViewPlants;
    ProgressBar progressBar;
    RecycleViewGenericAdapter<PlantDocAnswerList, PlanDocViewModelAnswer, PlanDocViewModelAnswer> adapter;

    String answerJsonString = null;
    String questionId = null;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_plant_doc_answer, container, false);



        Bundle args = getArguments();
        if (args != null) {
            questionId = args.getString("questionId");
        }
        Init(view);
        return view;
    }

    private void Init(View view) {
        recyclerViewPlants = view.findViewById(R.id.recyclerView_plant_doc_answer);
        progressBar = view.findViewById(R.id.progressBar_plant_doc_answer);

        String apiUrl = APP_URL.PLANT_DOC_ROUTE + questionId + "/getEntry";

        RequestQueueSingleton.getInstance(getContext()).typedRequest(apiUrl, this::onSuccess, this::onError, PlanDocViewModelAnswer.class, new RequestData(RequestType.PflanzenDocViewModelAnswer));

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

    private void onSuccess(PlanDocViewModelAnswer model, RequestData data) {
        List<PlanDocViewModelAnswer> pflanzenDocList = Arrays.asList(model);
        List<PlantDocAnswerList> answerList= pflanzenDocList.get(0).getPlantDocAnswerList();

            adapter = new RecycleViewGenericAdapter<>(
                    answerList, R.layout.recycler_view_plant_doc_answer_row_item, pflanzenDocList.get(0),R.layout.recycler_view_plant_doc_answer_header,pflanzenDocList.get(0), R.layout.recycler_view_plant_doc_answer_footer,this,null,null);
            configGenericRecyclerView(getActivity(), recyclerViewPlants, adapter, RecyclerView.VERTICAL);


        progressBar.setVisibility(View.GONE);
    }

    @Override
    public void onItemClick(PlantDocAnswerList position) {

    }
}
