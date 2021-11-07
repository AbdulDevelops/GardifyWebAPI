package com.gardify.android.ui.myGarden;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;
import android.widget.LinearLayout;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.StringRes;
import androidx.fragment.app.Fragment;
import androidx.navigation.Navigation;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.android.volley.Request;
import com.android.volley.VolleyError;
import com.gardify.android.data.misc.RequestData;
import com.gardify.android.data.myGarden.MyGarden;
import com.gardify.android.data.saveToGarden.PlantList;
import com.gardify.android.R;
import com.gardify.android.ui.saveToGarden.SaveToGardenRecyclerAdapter;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.ApiUtils;
import com.gardify.android.utils.RequestQueueSingleton;
import com.gardify.android.utils.RequestType;
import org.jetbrains.annotations.NotNull;
import org.json.JSONObject;

import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.stream.Collectors;

import static com.gardify.android.utils.UiUtils.displayAlertDialog;
import static com.gardify.android.utils.UiUtils.showErrorDialogNetworkParsed;

public class EditPlantFragment extends Fragment implements SaveToGardenRecyclerAdapter.OnItemClickListener, View.OnClickListener {

    // the fragment initialization parameters, e.g. ARG_ITEM_NUMBER
    private static final String VIEW_TYPE = "VIEW_TYPE";
    private static final String MY_GARDEN = "MY_GARDEN";


    private @StringRes
    int editPlantViewType;
    private String myGardenJson;
    private SaveToGardenRecyclerAdapter recyclerAdapter;
    private MyGarden myGarden;

    private RecyclerView recycleView;
    private Button addToGardenBtn, plusCountBtn, minusCountBtn;
    private TextView numberTextView, headerTextView, changeListLabel, notesLabel;
    private EditText notesEditText;
    private LinearLayout plantCountLinearLayout;
    private PlantList selectedPlant;
    private int plantCount=1;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        if (getArguments() != null) {
            editPlantViewType = getArguments().getInt(VIEW_TYPE);
            myGardenJson = getArguments().getString(MY_GARDEN);
            myGarden = ApiUtils.getGsonParser().fromJson(myGardenJson, MyGarden.class);
        }
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        View view = inflater.inflate(R.layout.fragment_my_garden_edit_plant, container, false);
        Init(view);

        String apiUrl = APP_URL.USER_LIST_API + "/plantlists/" + myGarden.getUserPlant().getId();
        RequestQueueSingleton.getInstance(getContext()).typedRequest(apiUrl, this::onSuccess, null, PlantList[].class, new RequestData(RequestType.PflanzenSucheModel));

        return view;
    }

    private void Init(View view) {
        /* finding views block */
        headerTextView= view.findViewById(R.id.text_view_my_garden_edit_plant_header);
        changeListLabel = view.findViewById(R.id.my_garden_edit_plant_change_list_label);
        recycleView = view.findViewById(R.id.recyclerView_my_garden_edit_plant);
        addToGardenBtn = view.findViewById(R.id.button_save_to_garden_add);
        notesLabel = view.findViewById(R.id.my_garden_edit_plant_notes_label);
        notesEditText = view.findViewById(R.id.edit_text_my_garden_plant_notes);
        plantCountLinearLayout = view.findViewById(R.id.linear_layout_my_garden_plant_count);
        plusCountBtn = view.findViewById(R.id.button_my_garden_plant_list_plus);
        minusCountBtn = view.findViewById(R.id.button_my_garden_plant_list_minus);
        numberTextView = view.findViewById(R.id.text_view_specify_number);
        addToGardenBtn.setOnClickListener(this);
        plusCountBtn.setOnClickListener(this);
        minusCountBtn.setOnClickListener(this);

        //update UI header
        headerTextView.setText(myGarden.getUserPlant().getName() + " in eine andere Liste verschieben");

        switch (editPlantViewType) {
            case R.string.myGarden_movePlant:
                headerTextView.setText(myGarden.getUserPlant().getName() + " in eine andere Liste verschieben");
                notesEditText.setVisibility(View.GONE);
                notesLabel.setVisibility(View.GONE);
                plantCountLinearLayout.setVisibility(View.GONE);
                //update UI header
                break;

            case R.string.myGarden_notesForThePlant:
                headerTextView.setText("Notizen bearbeiten");
                changeListLabel.setVisibility(View.GONE);
                plantCountLinearLayout.setVisibility(View.GONE);
                recycleView.setVisibility(View.GONE);
                notesEditText.setText(myGarden.getUserPlant().getNotes());
                break;

            case R.string.myGarden_specifyNumber:
                headerTextView.setText(getResources().getString(R.string.myGarden_specifyNumber));
                notesEditText.setVisibility(View.GONE);
                notesLabel.setVisibility(View.GONE);
                plantCount = myGarden.getUserPlant().getCount();
                numberTextView.setText("Anzahl: " + plantCount);
                break;
        }

    }

    List<PlantList> plantLists;

    private void onSuccess(PlantList[] model, RequestData data) {

        plantLists = Arrays.asList(model);
        //adapter
        recyclerAdapter = new SaveToGardenRecyclerAdapter(getActivity(), plantLists, false, this);

        configRecyclerView(recycleView, recyclerAdapter);


    }

    private void configRecyclerView(RecyclerView recyclerView, SaveToGardenRecyclerAdapter adapter) {
        LinearLayoutManager layoutManager = new LinearLayoutManager(getActivity(), RecyclerView.VERTICAL, false);
        recyclerView.setLayoutManager(layoutManager);
        recyclerView.setAdapter(adapter);
    }

    @Override
    public void onClick(View view) {

        switch (view.getId()) {
            case R.id.button_my_garden_plant_list_plus:
                plantCount++;
                numberTextView.setText("Anzahl: " + plantCount);

                break;

            case R.id.button_my_garden_plant_list_minus:
                plantCount--;
                if (plantCount < 1)
                    plantCount = 1;
                numberTextView.setText("Anzahl: " + plantCount);

                break;
        }

        // switch view type
        switch (editPlantViewType) {
            case R.string.myGarden_movePlant:
                movePlantOnclick(view);
                break;
            case R.string.myGarden_notesForThePlant:
                updateNoteOnclick(view);
                break;

            case R.string.myGarden_specifyNumber:
                updateCountOnclick(view);
                break;
        }
    }

    private void movePlantOnclick(View view) {
        switch (view.getId()) {

            case R.id.button_save_to_garden_add:

                HashMap<String, String> hashMap = new HashMap<>();
                hashMap.put("UserplantId", String.valueOf(myGarden.getUserPlant().getId()));
                hashMap.put("NewListId", String.valueOf(selectedPlant.getId()));

                JSONObject jsonObject = new JSONObject(hashMap);
                String apiUrl = APP_URL.USER_PLANT_API + "movePlant";
                RequestQueueSingleton.getInstance(getContext()).stringRequest(apiUrl, Request.Method.POST, this::OnEditSuccess, null, jsonObject);

                break;
        }
    }

    private void updateNoteOnclick(View view) {
        switch (view.getId()) {

            case R.id.button_save_to_garden_add:

                HashMap<String, String> params = new HashMap<>();

                params.put("Id", "" + myGarden.getUserPlant().getId());
                params.put("Description", myGarden.getUserPlant().getDescription());
                params.put("Count", "" + myGarden.getUserPlant().getCount());
                params.put("Name", myGarden.getUserPlant().getName());
                Integer[] userLists = convertListToArray(myGarden.getListIds());
                params.put("UserListId", Arrays.toString(userLists));
                params.put("Notes", notesEditText.getText().toString());
                params.put("IsInPot", "" + myGarden.getUserPlant().getIsInPot());

                JSONObject jsonObject = new JSONObject(params);
                String apiUrl = APP_URL.USER_PLANT_API + "false";
                RequestQueueSingleton.getInstance(getContext()).stringRequest(apiUrl, Request.Method.PUT, this::OnEditSuccess, this::OnError, jsonObject);

                break;
        }
    }

    private void updateCountOnclick(View view) {
        switch (view.getId()) {

            case R.id.button_save_to_garden_add:

                if (selectedPlant != null) {
                    HashMap<String, String> params = new HashMap<>();

                    params.put("Id", "" + myGarden.getUserPlant().getId());
                    params.put("Description", myGarden.getUserPlant().getDescription());
                    params.put("Count", "" + plantCount);
                    params.put("Name", myGarden.getUserPlant().getName());
                    params.put("UserListId", "" + selectedPlant.getId());
                    params.put("Notes", myGarden.getUserPlant().getNotes());
                    params.put("IsInPot", "" + myGarden.getUserPlant().getIsInPot());

                    JSONObject jsonObject = new JSONObject(params);
                    String apiUrl = APP_URL.USER_PLANT_API + "true";
                    RequestQueueSingleton.getInstance(getContext()).stringRequest(apiUrl, Request.Method.PUT, this::OnEditSuccess, this::OnError, jsonObject);
                } else {
                    Toast.makeText(getContext(), "Bitte Liste auswählen", Toast.LENGTH_SHORT).show();
                }
                break;
        }
    }

    private void OnError(VolleyError error) {
        showErrorDialogNetworkParsed(getContext(), error);
    }


    public Integer[] convertListToArray(List<Integer> list) {
        return list.stream().toArray(Integer[]::new);
    }

    private void OnEditSuccess(String s) {
        switch (editPlantViewType) {
            case R.string.myGarden_movePlant:
                displayAlertDialog(getContext(), "Die Pflanze wurde erfolgreich verschoben.");
                break;
        }
        //pop backstack
        Navigation.findNavController(getActivity(), R.id.nav_host_fragment).popBackStack();
    }

    @Override
    public void OnItemClickListener(View view, int position) {

        if (view.getId() == R.id.checkBox_save_to_garden) {
            selectedPlant = plantLists.get(position);
        }
    }

    @NotNull
    private String userListsCommaSeparated(List<Integer> list) {
        return list.stream().map(String::valueOf)
                .collect(Collectors.joining(","));
    }
}