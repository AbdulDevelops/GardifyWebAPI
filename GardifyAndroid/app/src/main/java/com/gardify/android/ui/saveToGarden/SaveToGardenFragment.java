package com.gardify.android.ui.saveToGarden;

import android.os.Bundle;

import androidx.fragment.app.Fragment;
import androidx.navigation.Navigation;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import android.text.Html;
import android.text.TextUtils;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import com.android.volley.Request;
import com.android.volley.VolleyError;
import com.gardify.android.data.account.UserMainGarden;
import com.gardify.android.data.misc.RequestData;
import com.gardify.android.data.myGarden.PlantCount;
import com.gardify.android.data.saveToGarden.PlantList;
import com.gardify.android.R;
import com.gardify.android.data.todos.TodoCount;
import com.gardify.android.ui.MainActivity;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.PreferencesUtility;
import com.gardify.android.utils.RequestQueueSingleton;
import com.gardify.android.utils.RequestType;
import com.google.android.material.switchmaterial.SwitchMaterial;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.Arrays;
import java.util.List;

import static com.gardify.android.utils.UiUtils.navigateToFragment;

public class SaveToGardenFragment extends Fragment implements SaveToGardenRecyclerAdapter.OnItemClickListener, View.OnClickListener {


    public static final String PLANT_ID_ARG = "PLANT_ID";
    public static final String PLANT_NAME_ARG = "PLANT_NAME";

    private static final String ARG_FRAGMENT_NAME = "FRAGMENT_NAME";

    private String plantId, plantName;
    private SaveToGardenRecyclerAdapter recyclerAdapter;

    // UI Views
    private TextView headerNamePlantToSave, userMainGardenTextView, numberTextView, createNewListTextView;
    private RecyclerView saveToGardenRecycleView;
    private Button addToGardenBtn, plusCountBtn, minusCountBtn;
    private SwitchMaterial topPlantSwitch;

    private int plantCount = 1;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        if (getArguments() != null) {
            plantId = getArguments().getString(PLANT_ID_ARG);
            plantName = getArguments().getString(PLANT_NAME_ARG);
        }
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        View view = inflater.inflate(R.layout.fragment_save_to_garden, container, false);

        //initializing views
        init(view);

        String apiUrl = APP_URL.USER_PLANT_BY_ID + plantId;
        RequestQueueSingleton.getInstance(getContext()).typedRequest(apiUrl, this::onSuccess, null, PlantList[].class, new RequestData(RequestType.PflanzenSucheModel));

        return view;
    }

    public void init(View view) {
        /* finding views block */
        headerNamePlantToSave = view.findViewById(R.id.text_view_save_to_my_garden_header);
        saveToGardenRecycleView = view.findViewById(R.id.recyclerView_my_garden_edit_plant);
        userMainGardenTextView = view.findViewById(R.id.text_view_save_to_garden_user_garden);
        plusCountBtn = view.findViewById(R.id.button_save_to_garden_plant_list_plus);
        minusCountBtn = view.findViewById(R.id.button_save_to_garden_plant_list_minus);
        numberTextView = view.findViewById(R.id.text_view_save_to_garden_specify_number);
        topPlantSwitch = view.findViewById(R.id.switch_save_to_garden_top_plant);
        createNewListTextView = view.findViewById(R.id.text_view_save_to_garden_create_new_list);
        addToGardenBtn = view.findViewById(R.id.button_save_to_garden_add);

        // set listeners
        addToGardenBtn.setOnClickListener(this);
        plusCountBtn.setOnClickListener(this);
        minusCountBtn.setOnClickListener(this);
        createNewListTextView.setOnClickListener(this);

        // update UI
        String saveToGarden =  getContext().getResources().getString(R.string.all_saveToGardenHeaderText);
        headerNamePlantToSave.setText(plantName + " " + saveToGarden);
        UserMainGarden userMainGarden = PreferencesUtility.getUserMainGarden(getContext());
        userMainGardenTextView.setText(userMainGarden.getName());

        createNewListTextView.setText(Html.fromHtml("Bitte eine Liste auswählen oder <u>eine neue Liste erstellen</u>"));

    }

    List<PlantList> plantLists;

    private void onSuccess(PlantList[] model, RequestData data) {

        plantLists = Arrays.asList(model);
        //adapter
        recyclerAdapter = new SaveToGardenRecyclerAdapter(getActivity(), plantLists, true,this);

        configRecyclerView(saveToGardenRecycleView, recyclerAdapter);

        // update UI
        numberTextView.setText("Anzahl: " + plantCount);

    }

    private void configRecyclerView(RecyclerView recyclerView, SaveToGardenRecyclerAdapter adapter) {
        LinearLayoutManager layoutManager = new LinearLayoutManager(getActivity(), RecyclerView.VERTICAL, false);
        recyclerView.setLayoutManager(layoutManager);
        recyclerView.setAdapter(adapter);
    }

    @Override
    public void onClick(View view) {

        switch (view.getId()) {

            case R.id.button_save_to_garden_plant_list_plus:
                plantCount++;
                numberTextView.setText("Anzahl: " + plantCount);

                break;

            case R.id.button_save_to_garden_plant_list_minus:
                plantCount--;
                if (plantCount < 1)
                    plantCount = 1;
                numberTextView.setText("Anzahl: " + plantCount);

                break;

            case R.id.text_view_save_to_garden_create_new_list:

                Bundle args = new Bundle();
                args.putInt(ARG_FRAGMENT_NAME, R.string.all_saveToGarden);
                navigateToFragment(R.id.nav_controller_my_garden_edit_user_garden_list, getActivity(), false, args);

                break;

            case R.id.button_save_to_garden_add:

                SavePlantPostRequest();

                break;

        }
    }

    private void SavePlantPostRequest() {
        // Change fragment
        String postUrl = APP_URL.USER_PLANT_PROP_ADD;
        //String postUrl = "http://httpbin.org/post";

        JSONObject jsonBody = new JSONObject();

        // creating json array
        JSONArray ArrayOfUserList=new JSONArray();

        for (PlantList plant: plantLists) {
            if(plant.getListSelected()){
                JSONObject obj=new JSONObject();
                try {
                    obj.put("UserPlantId", 1);
                    obj.put("UserListId",plant.getId());
                } catch (JSONException e) {
                    e.printStackTrace();
                }
                ArrayOfUserList.put(obj);
            }
        }

        try {
            jsonBody.put("PlantId", Integer.valueOf(plantId));
            jsonBody.put("InitialAgeInDays", 1);
            jsonBody.put("Count", plantCount);
            jsonBody.put("IsInPot", topPlantSwitch.isChecked());
            jsonBody.put("Todos", null);
            jsonBody.put("ArrayOfUserlist", ArrayOfUserList);

        } catch (JSONException e) {
            e.printStackTrace();
        }

        RequestQueueSingleton.getInstance(getContext()).objectRequest(postUrl, Request.Method.POST, this::SaveSuccess, this::SaveError, jsonBody);
    }

    private void SaveError(VolleyError volleyError) {
        Log.d("Response", volleyError.toString());
        Toast.makeText(getContext(),volleyError.toString(), Toast.LENGTH_SHORT).show();
    }

    private void SaveSuccess(JSONObject response) {
        Log.d("Response", response.toString());
        Toast.makeText(getContext(), R.string.all_dataSavedSuccessfully, Toast.LENGTH_SHORT).show();
        Navigation.findNavController(getActivity(), R.id.nav_host_fragment).popBackStack();
        ((MainActivity) getActivity()).updateActionBarCounters();

    }
    @Override
    public void OnItemClickListener(View view, int position) {

        if(view.getId() == R.id.checkBox_save_to_garden){
            boolean checkedStatus = plantLists.get(position).getListSelected();

            plantLists.get(position).setListSelected(!checkedStatus);
            recyclerAdapter.Update(plantLists);
        }
    }

    @Override
    public void onPause() {
        super.onPause();
    }
}