package com.gardify.android.ui.myGarden;

import android.content.res.Resources;
import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ProgressBar;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;
import androidx.lifecycle.ViewModelProviders;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.android.volley.Request;
import com.android.volley.VolleyError;
import com.gardify.android.data.misc.RequestData;
import com.gardify.android.data.myGarden.AdminDevice.AdminDevice;
import com.gardify.android.R;
import com.gardify.android.ui.myGarden.adapter.AddDeviceAdapter;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.RequestQueueSingleton;
import com.gardify.android.utils.RequestType;

import org.json.JSONArray;
import org.json.JSONException;

import java.util.Arrays;
import java.util.List;
import java.util.stream.Collectors;

public class AddDeviceFragment extends Fragment implements AddDeviceAdapter.OnItemClickListener {

    private static final String EXPANDED_MENU_STATE = "EXPANDED_MENU";
    private static final String TAG = "AddDeviceFragment:";
    private RecyclerView recyclerView;
    private ProgressBar progressBar;

    public View onCreateView(@NonNull LayoutInflater inflater,
                             ViewGroup container, Bundle savedInstanceState) {

        View root = inflater.inflate(R.layout.fragment_my_garden_add_device, container, false);

        recyclerView = root.findViewById(R.id.recyclerView_my_garden_add_device);
        progressBar = root.findViewById(R.id.progressBar_my_garden_add_device);

        String apiUrl = APP_URL.DEVICE_API + "AdminDevices";
        RequestQueueSingleton.getInstance(getContext()).typedRequest(apiUrl, this::onSuccessUserPlant, this::onError, AdminDevice[].class, new RequestData(RequestType.AdminDevice));

        Button addDevice = root.findViewById(R.id.button_my_garden_admin_add_device);
        addDevice.setOnClickListener(view -> {

            adminDeviceList = adminDeviceList.stream().filter(p -> p.isCheckedFlag())
                    .collect(Collectors.toList());

            int[] selectedIds = new int[adminDeviceList.size()];

            for (int i = 0; i < adminDeviceList.size(); i++) {
                selectedIds[i] = adminDeviceList.get(i).getId();
            }


            // add selected Ids to database
            JSONArray jsonArray = null;

            try {
                jsonArray = new JSONArray(selectedIds);

            } catch (JSONException e) {
                e.printStackTrace();
            }

            String postDeviceUrl = APP_URL.DEVICE_API + "postAdminDevice";
            RequestQueueSingleton.getInstance(getContext()).arrayRequest(postDeviceUrl, Request.Method.POST, this::onSuccessAddDevice, this::onErrorAddDevice, jsonArray);

        });

        return root;
    }

    private void onErrorAddDevice(VolleyError volleyError) {
        //Toast.makeText(getContext(), "" + volleyError, Toast.LENGTH_SHORT).show();
        Log.e(TAG, "AddDeviceFragment: " + volleyError.getMessage());

        if(volleyError.getMessage().contains("success")){
            goBackToMyGarden();
        }
    }

    private void onSuccessAddDevice(JSONArray jsonArray) {

        Log.e(TAG, "" + jsonArray);

        // return to my garden
        goBackToMyGarden();

    }
    private void goBackToMyGarden() {
        //pop back stack
        persistDataViewModel.setMyGardenState(R.string.myGarden_addNewDeviceAccessories);
        getActivity().onBackPressed();

    }

    List<AdminDevice> adminDeviceList;
    AddDeviceAdapter addDeviceAdapter;

    private void onSuccessUserPlant(AdminDevice[] model, RequestData data) {

        adminDeviceList = Arrays.asList(model);

        //adapter
        addDeviceAdapter = new AddDeviceAdapter(getActivity(), adminDeviceList, this);

        configRecyclerView(recyclerView, addDeviceAdapter);
        progressBar.setVisibility(View.GONE);
    }

    public void onError(Exception ex, RequestData data) {
        if (getActivity() != null) {
            Resources res = getContext().getResources();
            String network = res.getString(R.string.all_network);
            String anErrorOccurred = res.getString(R.string.all_anErrorOccurred);
            Log.e(network, anErrorOccurred + ex.toString());
            progressBar.setVisibility(View.GONE);
        }
    }

    private void configRecyclerView(RecyclerView recyclerView, AddDeviceAdapter adapter) {
        LinearLayoutManager layoutManager = new LinearLayoutManager(getActivity(), RecyclerView.VERTICAL, false);
        recyclerView.setLayoutManager(layoutManager);
        recyclerView.setAdapter(adapter);
    }

    @Override
    public void OnItemClickListener(View view, int position) {
        if (view.getId() == R.id.checkbox_filter_item_row_plant_search_filter) {

            if (adminDeviceList.get(position).isCheckedFlag()) {
                adminDeviceList.get(position).setCheckedFlag(false);
            } else {
                adminDeviceList.get(position).setCheckedFlag(true);
            }

        }
    }
    MyGardenPersistDataViewModel persistDataViewModel;
    @Override
    public void onActivityCreated(@Nullable Bundle savedInstanceState) {
        super.onActivityCreated(savedInstanceState);
        persistDataViewModel = ViewModelProviders.of(getActivity()).get(MyGardenPersistDataViewModel.class);
    }
}