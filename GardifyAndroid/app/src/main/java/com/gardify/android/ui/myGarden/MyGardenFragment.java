package com.gardify.android.ui.myGarden;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.res.Resources;
import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.LinearLayout;
import android.widget.PopupMenu;
import android.widget.ProgressBar;
import android.widget.Spinner;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;
import androidx.lifecycle.ViewModelProviders;
import androidx.recyclerview.widget.GridLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.android.volley.Request;
import com.android.volley.VolleyError;
import com.gardify.android.R;
import com.gardify.android.data.account.ApplicationUser;
import com.gardify.android.data.misc.RequestData;
import com.gardify.android.data.myGarden.Eco.EcoElement;
import com.gardify.android.data.myGarden.MyGarden;
import com.gardify.android.data.myGarden.PlantCount;
import com.gardify.android.data.myGarden.UserDevice.UserDevice;
import com.gardify.android.data.myGarden.UserGarden.UserGarden;
import com.gardify.android.databinding.RecyclerViewMyGardenDeviceItemBinding;
import com.gardify.android.generic.CustomBottomSheet;
import com.gardify.android.generic.GenericDialog;
import com.gardify.android.ui.generic.CarouselGroup;
import com.gardify.android.ui.generic.ExpandableHeaderItem;
import com.gardify.android.ui.generic.ExpandableInnerHeaderItem;
import com.gardify.android.ui.generic.ExpandableNoBgHeaderItem;
import com.gardify.android.ui.generic.InfiniteScrollListener;
import com.gardify.android.ui.generic.decoration.CarouselItemDecoration;
import com.gardify.android.ui.generic.interfaces.OnExpandableHeaderListener;
import com.gardify.android.ui.generic.recyclerItem.CardItem;
import com.gardify.android.ui.generic.recyclerItem.CardViewTopBottomSection;
import com.gardify.android.ui.generic.recyclerItem.HeaderTwoIcons;
import com.gardify.android.ui.myGarden.interfaces.OnPlantClickListener;
import com.gardify.android.ui.myGarden.recyclerItems.CardAddItem;
import com.gardify.android.ui.myGarden.recyclerItems.DeviceCardItem;
import com.gardify.android.ui.myGarden.recyclerItems.EcoElementCarouselItem;
import com.gardify.android.ui.myGarden.recyclerItems.GardenCardItem;
import com.gardify.android.ui.generic.recyclerItem.GenericGridItem;
import com.gardify.android.ui.myGarden.recyclerItems.ListenCardItem;
import com.gardify.android.ui.myGarden.recyclerItems.PlantEcoFilterCardItem;
import com.gardify.android.ui.myGarden.recyclerItems.PlantEcoFilterIconsCardItem;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.ApiUtils;
import com.gardify.android.utils.PreferencesUtility;
import com.gardify.android.utils.RequestQueueSingleton;
import com.gardify.android.utils.RequestType;
import com.gardify.android.viewModelData.BadgesIconVM;
import com.xwray.groupie.ExpandableGroup;
import com.xwray.groupie.Group;
import com.xwray.groupie.GroupAdapter;
import com.xwray.groupie.Section;

import org.jetbrains.annotations.NotNull;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collection;
import java.util.Collections;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;
import java.util.stream.Stream;

import static com.gardify.android.ui.home.HomeFragment.RECYCLE_VIEW_DEFAULT_SPAN_COUNT;
import static com.gardify.android.ui.saveToGarden.SaveToGardenFragment.PLANT_ID_ARG;
import static com.gardify.android.utils.ApiUtils.startCountFromZero;
import static com.gardify.android.utils.UiUtils.displayAlertDialog;
import static com.gardify.android.utils.UiUtils.navigateToFragment;
import static com.gardify.android.utils.UiUtils.setupToolbar;
import static com.gardify.android.utils.UiUtils.showErrorDialogNetworkParsed;
import static com.gardify.android.viewModelData.BadgesIconVM.EcoBadges;
import static com.gardify.android.viewModelData.BadgesIconVM.FrostBadges;

public class MyGardenFragment extends Fragment {

    private static final String TAG = "MyGardenFragment";

    //groupie adapter tags
    public static final String INSET_TYPE_KEY = "inset_type";
    public static final String INSET = "inset";
    public static final int RECYCLE_VIEW_SPAN_COUNT = 3;

    //Views
    private RecyclerView recyclerViewPlants;
    private ProgressBar progressBar;

    //Groupie adapter
    private GroupAdapter groupAdapter;
    private GridLayoutManager layoutManager;
    private List<Group> plantPayloadList = new ArrayList<>();
    private Section updateWithPayloadSection;
    private InfiniteScrollListener infiniteScrollListener;

    //Lists
    private List<MyGarden> loadedGardenList = new ArrayList<>();
    private List<UserDevice> userDeviceList;
    private List<UserGarden> userGardenList;
    private int listCounter = 0, deviceCounter = 0, ecoCounter = 0, plantCounter = 0;

    // UI temp settings
    private boolean isGrid = true;
    private int selectedUserGardenListId = 0;
    private String selectedUserGardenName;
    private int expandedMenuState = 0;

    //Api related
    private String baseApiUrl = APP_URL.USER_PLANT_API;
    private static final int PLANT_TAKE_COUNT = 10;
    private String paramEcoTags = "";
    private List<String> loadedPagesList = new ArrayList<>();

    public View onCreateView(@NonNull LayoutInflater inflater,
                             ViewGroup container, Bundle savedInstanceState) {

        //setup Toolbar
        authenticationCheck();
        setupToolbar(getActivity(), "MEIN GARTEN", R.drawable.gardify_app_icon_mein_garten_normal, R.color.toolbar_all_greyishTurquoise, true);

        View root = inflater.inflate(R.layout.fragment_my_garden, container, false);


        initializeViews(root);
        SetupGroupAdapter();

        return root;
    }

    public void initializeViews(View root) {
        recyclerViewPlants = root.findViewById(R.id.recycleView_myGarden);
        progressBar = root.findViewById(R.id.progressBar_myGarden);

        selectedUserGardenName = getContext().getResources().getString(R.string.myGarden_myPlants);
    }

    private void SetupGroupAdapter() {
        groupAdapter = new GroupAdapter();
        groupAdapter.setSpanCount(RECYCLE_VIEW_DEFAULT_SPAN_COUNT);
        layoutManager = new GridLayoutManager(getContext(), groupAdapter.getSpanCount());
        layoutManager.setSpanSizeLookup(groupAdapter.getSpanSizeLookup());
        recyclerViewPlants.setLayoutManager(layoutManager);
        populateAdapter();
        recyclerViewPlants.setAdapter(groupAdapter);
    }

    private ExpandableGroup expandableGroupEco, expandableGroupDevices, expandableGroupListen, expandableInnerGroupPlantsFilter, expandableEcoFilterGroup;
    private ExpandableNoBgHeaderItem expandableHeaderList;
    private ExpandableHeaderItem expandableHeaderDevices, expandableHeaderEco, expandableHeaderEcoFilterPlants;
    private Group carousel;
    private ExpandableInnerHeaderItem innerExpandableHeaderPlantsFilter;
    private PlantEcoFilterIconsCardItem plantFilterCardItem;
    private HeaderTwoIcons headerGridListGroup;

    private void populateAdapter() {

        // section with background
        Section cardViewTopSection = new Section(new CardViewTopBottomSection(true));
        Section cardViewBottomSection = new Section(new CardViewTopBottomSection(false));
        Section sectionWithBackground = new Section();
        sectionWithBackground.setHeader(cardViewTopSection);
        sectionWithBackground.setFooter(cardViewBottomSection);

        // Expandable group Listen
        expandableHeaderList = new ExpandableNoBgHeaderItem(getContext(), R.color.text_all_gunmetal, R.color.expandableHeader_all_white, R.string.myGarden_lists, 0, onExpandableHeaderListener);
        expandableGroupListen = new ExpandableGroup(expandableHeaderList);
        sectionWithBackground.add(expandableGroupListen);
        groupAdapter.add(sectionWithBackground);

        // Expandable group Geräte/Devices
        expandableHeaderDevices = new ExpandableHeaderItem(getContext(), R.color.text_all_gunmetal, R.color.expandableHeader_all_white, R.string.all_devices, 0, onExpandableHeaderListener);
        expandableGroupDevices = new ExpandableGroup(expandableHeaderDevices);
        groupAdapter.add(expandableGroupDevices);

        // Expandable group Eco-Element
        expandableHeaderEco = new ExpandableHeaderItem(getContext(), R.color.text_all_gunmetal, R.color.expandableHeader_all_white, R.string.all_ecoElements, 0, onExpandableHeaderListener);
        expandableGroupEco = new ExpandableGroup(expandableHeaderEco);
        expandableGroupEco.add(new CardItem(getContext(), R.color.expandableGroup_all_whiteSmoke, R.string.myGarden_ecoElementDescription));

        groupAdapter.add(expandableGroupEco);

        expandableHeaderEcoFilterPlants = new ExpandableHeaderItem(getContext(), R.color.text_all_white, R.color.expandableHeader_myGarden_ecoFilterPlants, R.string.all_plants, 0, onExpandableHeaderListener);
        PlantCount plantCount = PreferencesUtility.getPlantCount(getContext());
        int plantCountSorts;
        int plantCountTotal;
        if(plantCount != null) {
             plantCountSorts = plantCount.getSorts();
             plantCountTotal = plantCount.getTotal();
        } else {
            plantCountSorts = 0;
            plantCountTotal = 0;
        }
        expandableHeaderEcoFilterPlants.setTitle(plantCountSorts + "/" + plantCountTotal);
        expandableEcoFilterGroup = new ExpandableGroup(expandableHeaderEcoFilterPlants);
        plantFilterCardItem = new PlantEcoFilterIconsCardItem(getContext(), R.color.cardItem_myGarden_plantsFilter, loadedGardenList, "");
        expandableEcoFilterGroup.add(plantFilterCardItem);

        // plants filter inner expandable
        innerExpandableHeaderPlantsFilter = new ExpandableInnerHeaderItem(getContext(), R.color.expandableHeader_myGarden_plantsFilter, getContext().getResources().getString(R.string.myGarden_filterPlants), 0);
        expandableInnerGroupPlantsFilter = new ExpandableGroup(innerExpandableHeaderPlantsFilter);
        expandableInnerGroupPlantsFilter.add(new PlantEcoFilterCardItem(getContext(), true, onFilterEcoClickListener));

        // Expandable group Plants // bio icons
        List<BadgesIconVM> badgesIconVMList = CombineEcoBadgesFrostBadges();
        for (BadgesIconVM badgesIconVM : badgesIconVMList) {
            expandableInnerGroupPlantsFilter.add(new PlantEcoFilterCardItem(getContext(), badgesIconVM, true, onFilterEcoClickListener));
        }
        expandableEcoFilterGroup.add(expandableInnerGroupPlantsFilter);
        expandableEcoFilterGroup.add(new CardAddItem(getContext(), R.color.expandableGroup_myGarden_plantsFilter, R.string.myGarden_addPlant, onCardAddClickListener));
        groupAdapter.add(expandableEcoFilterGroup);

        // Grid-List view header Buttons
        Section GridListOptionSection = new Section();
        headerGridListGroup = new HeaderTwoIcons(selectedUserGardenName, getContext(), isGrid, R.drawable.selector_grid_view_icon, R.drawable.selector_list_view_icon, onIconClickListener);
        GridListOptionSection.add(headerGridListGroup);
        groupAdapter.add(GridListOptionSection);

        // Main recycleView Update with payload
        updateWithPayloadSection = new Section();
        groupAdapter.add(updateWithPayloadSection);
        loadedGardenList = new ArrayList<>();
        loadPlantsFromApiInfiniteScroll(baseApiUrl);

        getUserGardenListFromApi();
        getDevicesFromApi();
        getEcoElementsFromApi();
    }

    @NotNull
    private List<BadgesIconVM> CombineEcoBadgesFrostBadges() {
        return Stream.of(EcoBadges, FrostBadges)
                .flatMap(Collection::stream)
                .collect(Collectors.toList());
    }

    private boolean hasMorePlants(int currentPage) {
        return loadedPagesList.size() == (currentPage * PLANT_TAKE_COUNT);
    }

    private void loadPlantsFromApiInfiniteScroll(String baseUrl) {
        progressBar.setVisibility(View.VISIBLE);
        resetInfiniteScroll();
        loadedGardenList.clear();

        infiniteScrollListener = new InfiniteScrollListener(layoutManager) {
            @Override
            public void onLoadMore(int currentPage) {
                //progressBar.setVisibility(View.VISIBLE);
                Log.d(TAG, "pageCount : " + currentPage);
                currentPage = startCountFromZero(currentPage);
                String paramUrl = baseUrl + "?take=" + PLANT_TAKE_COUNT + "&skip=" + currentPage * PLANT_TAKE_COUNT;
                //when if(hasMorePlants(...)) is in the code, only 10 plants get loaded
                //if(hasMorePlants(currentPage)) {
                    Log.d(TAG, "Hat nocht mehr: " + currentPage);
                    if (!loadedPagesList.contains(paramUrl)) {
                        loadedPagesList.add(paramUrl);
                        progressBar.setVisibility(View.VISIBLE);
                        RequestQueueSingleton.getInstance(getContext()).typedRequest(paramUrl, this::onSuccessUserPlant, null, MyGarden[].class, new RequestData(RequestType.MyGarden));
                    }
                //}
            }

            private void onSuccessUserPlant(MyGarden[] model, RequestData data) {
                if (!isVisible()) {
                    return;
                }
                if (model.length > 0) {
                    progressBar.setVisibility(View.GONE);
                    List<MyGarden> myGardenNextList = Arrays.asList(model);
                    updatePayloadSection(myGardenNextList, isGrid);
                    progressBar.setVisibility(View.GONE);
                    // updateComponentsAssociatedWithMyGarden();
                    loadedGardenList.addAll(myGardenNextList);
                    updateEcoFilterIconsCount();
                } else {
                    removeInfiniteScrollListener();
                }
            }

        };

        recyclerViewPlants.addOnScrollListener(infiniteScrollListener);
    }

    private void removeInfiniteScrollListener() {
        if (infiniteScrollListener != null) {
            recyclerViewPlants.removeOnScrollListener(infiniteScrollListener);
            progressBar.setVisibility(View.GONE);
        }
    }

    private void resetInfiniteScroll() {
        updateWithPayloadSection.removeAll(plantPayloadList);
        plantPayloadList.clear();
        groupAdapter.notifyDataSetChanged();
        loadedPagesList.clear();
        recyclerViewPlants.smoothScrollToPosition(0);
        recyclerViewPlants.clearOnScrollListeners();
    }

    GenericGridItem gridItem = null;
    private void updatePayloadSection(List<MyGarden> myGardenNextList, boolean isGrid) {

        myGardenNextList = filterListEcoTags(myGardenNextList);
        if (isGrid) {
            for (int i = 0; i < myGardenNextList.size(); i++) {

                String imageUrl = APP_URL.BASE_ROUTE_INTERN + myGardenNextList.get(i).getUserPlant().getImages().get(0).getSrcAttr();
                MyGarden myGarden = myGardenNextList.get(i);

                gridItem = new GenericGridItem.Builder(getContext())
                        .setId(i)
                        .setImageUrl(imageUrl)
                        .setSpanCount(RECYCLE_VIEW_SPAN_COUNT)
                        .setImageClickListener((binding, view, position) -> {
                            onPlantClickListener.onClick(myGarden, null, binding, view, position);
                        })
                        .build();

                plantPayloadList.add(gridItem);
            }
        } else {
            for (int i = 0; i < myGardenNextList.size(); i++) {
                plantPayloadList.add(new GardenCardItem(i, getContext(), myGardenNextList.get(i), onPlantClickListener));
            }
        }
        updateWithPayloadSection.update(plantPayloadList);
        groupAdapter.notifyDataSetChanged();
    }

    private List<MyGarden> filterListEcoTags(List<MyGarden> myGardenList) {
        if (!paramEcoTags.isEmpty()) {
            myGardenList = myGardenList.stream()
                    .filter(x -> x.getUserPlant().getBadges().stream()
                            .anyMatch(one -> one.getId().equals(paramEcoTags)))
                    .collect(Collectors.toList());
        }
        return myGardenList;
    }

    private final ListenCardItem.OnUserListClickListener onUserListClickListener = (userGarden, viewBinding, view, position) -> {
        // Pretend to make a network request
        if (viewBinding.textViewMyGardenListenName.equals(view)) {
            // user lists
            if (userGarden != null) {
                if (userGarden.getId() != selectedUserGardenListId) {
                    selectedUserGardenListId = userGarden.getId();
                    selectedUserGardenName = userGarden.getName();
                    //update headerGridListGroup name
                    headerGridListGroup.setText(selectedUserGardenName);
                    baseApiUrl = APP_URL.USER_PLANT_API + "UserPlantByUserListId/" + selectedUserGardenListId;
                } else {
                    resetSelectedUserGardenList();
                }
            } else {
                resetSelectedUserGardenList();
            }

            loadPlantsFromApiInfiniteScroll(baseApiUrl);
            expandableGroupListen.onToggleExpanded();


        } else if (viewBinding.imageViewMyGardenMoreOptions.equals(view)) {
            // data to pass to next fragment
            Bundle args = new Bundle();
            String personJsonString = ApiUtils.getGsonParser().toJson(userGarden);
            args.putString("USER_GARDEN_DATA", personJsonString);
            navigateToFragment(R.id.nav_controller_my_garden_edit_user_garden_list, (Activity) getContext(), false, args);

        } else if (viewBinding.imageViewMyGardenListenDelete.equals(view)) {

            showDeleteUserListAlertDialog(userGarden);
        }
    };

    private void resetSelectedUserGardenList() {
        selectedUserGardenName = getContext().getResources().getString(R.string.myGarden_myPlants);
        headerGridListGroup.setText(selectedUserGardenName);
        baseApiUrl = APP_URL.USER_PLANT_API;
        selectedUserGardenListId = 0; // meinPflanze selection is 0
    }

    private final OnExpandableHeaderListener onExpandableHeaderListener = (stringId) -> {
        // Pretend to make a network request
        switch (stringId) {
            case R.string.all_ecoElements:
                getEcoElementsFromApi();
                break;

            case R.string.all_devices:
                if (userDeviceList == null)
                    getDevicesFromApi();
                break;

            case R.string.myGarden_lists:
                getUserGardenListFromApi();
                break;

            case R.string.all_plants:
                updateEcoFilterIconsCount();
                break;
        }
    };

    private void updateEcoFilterIconsCount() {
        if (expandableEcoFilterGroup.isExpanded()) {
            plantFilterCardItem.setEcoCounters(loadedGardenList);
            groupAdapter.notifyDataSetChanged();
        }
    }

    private void getEcoElementsFromApi() {
        progressBar.setVisibility(View.VISIBLE);
        String apiUrlEcoElements = APP_URL.USER_GARDEN_API + "ecoelements";
        RequestQueueSingleton.getInstance(getContext()).typedRequest(apiUrlEcoElements, this::onSuccessEcoElements, this::onError, EcoElement[].class, new RequestData(RequestType.EcoElement));

    }

    private void getUserGardenListFromApi() {
        progressBar.setVisibility(View.VISIBLE);
        String apiUrl = APP_URL.USER_LIST_API;
        RequestQueueSingleton.getInstance(getContext()).typedRequest(apiUrl, this::onSuccessUserGarden, this::onError, UserGarden[].class, new RequestData(RequestType.UserGarden));
    }

    private final DeviceCardItem.OnDeviceClickListener onDeviceClickListener = (userDevice, viewBinding, view, position) -> {
        // Pretend to make a network request

        if (viewBinding.switchMyGardenDeviceFrost.equals(view)) {
            // toggle frost
            userDevice.setNotifyForFrost(!userDevice.getNotifyForFrost());
            UpdatingSwitchValueDevice(userDevice, viewBinding);
            GroupAdapterItemChange(userDevice, position);

        } else if (viewBinding.switchMyGardenDeviceStorm.equals(view)) {
            // toggle wind
            userDevice.setNotifyForWind(!userDevice.getNotifyForWind());
            UpdatingSwitchValueDevice(userDevice, viewBinding);
            GroupAdapterItemChange(userDevice, position);

        } else if (viewBinding.textViewMyGardenDeviceItemPlusButton.equals(view)) {

            UpdateApiDeviceCount(userDevice, userDevice.getCount() + 1);
            userDevice.setCount(userDevice.getCount() + 1);
            GroupAdapterItemChange(userDevice, position);

        } else if (viewBinding.textViewMyGardenDeviceItemMinusButton.equals(view)) {
            if (userDevice.getCount() > 1) {
                UpdateApiDeviceCount(userDevice, userDevice.getCount() - 1);
                userDevice.setCount(userDevice.getCount() - 1);
            }
            GroupAdapterItemChange(userDevice, position);

        } else if (viewBinding.textviewMyGardenDelete.equals(view)) {
            String deleteDeviceUrl = APP_URL.DEVICE_API + userDevice.getId();
            RequestQueueSingleton.getInstance(getContext()).stringRequest(deleteDeviceUrl, Request.Method.DELETE, MyGardenFragment.this::onSuccessDeleteDevice, null, null);
        }
    };

    private void GroupAdapterItemChange(UserDevice userDevice, int position) {
        // update adapter
        Group group = groupAdapter.getItem(position);
        groupAdapter.onItemChanged(group, position, userDevice);
        groupAdapter.notifyDataSetChanged();
    }

    private final PlantEcoFilterCardItem.OnFilterEcoClickListener onFilterEcoClickListener = (_badgesVM, reset) -> {

        innerExpandableHeaderPlantsFilter.setTitle(getResources().getString(R.string.myGarden_filterPlants));
        String badgeID="";

        if (!reset) {
            innerExpandableHeaderPlantsFilter.setTitle(_badgesVM.getName());
            badgeID = _badgesVM.getId();
            paramEcoTags = badgeID;

        } else {
            paramEcoTags = "";
        }
        expandableInnerGroupPlantsFilter.onToggleExpanded();

        loadPlantsFromApiInfiniteScroll(baseApiUrl);

        //update row item
        plantFilterCardItem.selectEcoTag(badgeID);
        groupAdapter.notifyDataSetChanged();

    };

    private final CardAddItem.OnCardClickListener onCardAddClickListener = (stringId) -> {
        // Pretend to make a network request
        switch (stringId) {
            case R.string.myGarden_addPlant:
                AddPlantDialog();
                break;

            case R.string.myGarden_addNewList:
                navigateToFragment(R.id.nav_controller_my_garden_edit_user_garden_list, (Activity) getContext(), false, null);

                break;

            case R.string.myGarden_addNewDeviceAccessories:
                navigateToFragment(R.id.nav_controller_my_garden_add_device, (Activity) getContext(), false, null);

                break;
        }
    };

    private final EcoElementCarouselItem.OnEcoClickListener onEcoClickListener = (ecoElement, viewBinding, view) -> {
        // Pretend to make a network request

        if (viewBinding.switchMyGardenEcoElement.equals(view)) {
            // change check
            UpdatingSwitchValueEcoElement(ecoElement);

        } else if (viewBinding.buttonMyGardenEcoMoreDetail.equals(view)) {
            Bundle bundle = new Bundle();
            bundle.putString("ECO_DETAIL", ecoElement.getName());
            navigateToFragment(R.id.nav_controller_my_garden_eco_detail, (Activity) getContext(), false, bundle);

        }
    };

    private final OnPlantClickListener onPlantClickListener = (myGarden, cardBinding, gridBinding, view, position) -> {
        // Pretend to make a network request
        if (cardBinding != null) {
            if (cardBinding.textviewMyGardenPlantOptions.equals(view)) {
                // change check
                showPlantMenuOptions(view, myGarden);
            } else if (cardBinding.imageViewMyGardenPlantItemPlantImage.equals(view)
                    || cardBinding.buttonItemGardenCardDetail.equals(view)) {
                // change check
                gotoDetailFragment(myGarden);
            }
        } else {
            if (gridBinding.image.equals(view)) {
                // scroll to selected object position - list view
                isGrid = false;
                smoothScrollToCardPosition(myGarden);
            }
        }
    };

    private void smoothScrollToCardPosition(MyGarden myGarden) {
        int index = loadedGardenList.indexOf(myGarden);
        loadedGardenList.remove(index);
        loadedGardenList.add(0, myGarden);

        // delete plants
        updateWithPayloadSection.removeAll(plantPayloadList);
        plantPayloadList.clear();

        isGrid = false;
        updateGridIcon(isGrid);

        // reload plants
        updatePayloadSection(loadedGardenList, isGrid);
        recyclerViewPlants.smoothScrollToPosition(0);
    }

    private void gotoDetailFragment(MyGarden myGarden) {
        Bundle bundle = new Bundle();
        bundle.putString(PLANT_ID_ARG, String.valueOf(myGarden.getUserPlant().getPlantId()));
        navigateToFragment(R.id.nav_controller_plant_detail, (Activity) getContext(), false, bundle);
    }

    private final HeaderTwoIcons.onIconClickListener onIconClickListener = (grid, binding) -> {
        //set button states
        updateGridIcon(grid);
        binding.imageOne.setSelected(grid);
        binding.imageTwo.setSelected(!grid);
        // update main list
        loadPlantsFromApiInfiniteScroll(baseApiUrl);

    };

    private void updateGridIcon(boolean _isGrid) {
        isGrid = _isGrid;
        headerGridListGroup.setGridFlag(isGrid);
        groupAdapter.notifyDataSetChanged();
    }

    private void clearExpandAbleGroup(ExpandableGroup expandableGroup) {
        int groupCount = expandableGroup.getGroupCount();
        for (int i = 1; i < groupCount; i++) {
            Group group = expandableGroup.getGroup(1);
            expandableGroup.remove(group);
        }
    }

    private void onSuccessDevice(UserDevice[] model, RequestData data) {
        deviceCounter = 0;
        userDeviceList = new ArrayList<>();
        userDeviceList.clear();
        for (UserDevice userDevice : model) {
            userDeviceList.add(userDevice);
        }

        clearExpandAbleGroup(expandableGroupDevices);

        for (int i = 0; i < userDeviceList.size(); i++) {
            expandableGroupDevices.add(new DeviceCardItem(getContext(), userDeviceList.get(i), onDeviceClickListener));
            deviceCounter++;
        }
        expandableHeaderDevices.setTitle(deviceCounter);
        expandableGroupDevices.add(new CardAddItem(getContext(), R.color.expandableGroup_all_whiteSmoke, R.string.myGarden_addNewDeviceAccessories, onCardAddClickListener));
        groupAdapter.notifyDataSetChanged();
        progressBar.setVisibility(View.GONE);
    }

    private void onSuccessUserGarden(UserGarden[] model, RequestData data) {
        listCounter = 0;
        userGardenList = new ArrayList<>();
        userGardenList = Arrays.asList(model);
        //adapter

        clearExpandAbleGroup(expandableGroupListen);

        for (int i = 0; i < userGardenList.size(); i++) {
            expandableGroupListen.add(new ListenCardItem(getResources().getColor(R.color.expandableGroup_all_white, null), getContext(),
                    selectedUserGardenListId, userGardenList.get(i), onUserListClickListener));
            listCounter++;
        }
        expandableGroupListen.add(new CardAddItem(getContext(), R.color.expandableGroup_all_white, R.string.myGarden_addNewList, onCardAddClickListener));
        expandableHeaderList.setCount(listCounter);
        groupAdapter.notifyDataSetChanged();
        progressBar.setVisibility(View.GONE);

    }

    List<EcoElement> ecoElementArrayList = new ArrayList<>();

    private void onSuccessEcoElements(EcoElement[] model, RequestData data) {
        ecoCounter = 0;
        ecoElementArrayList.clear();
        Collections.addAll(ecoElementArrayList, model);

        // remove previous carousel
        if (carousel != null)
            expandableGroupEco.remove(carousel);

        for (EcoElement ecoElement : ecoElementArrayList) {
            if (ecoElement.getChecked())
                ecoCounter++;
        }

        carousel = makeEcoElementsCarouselGroup();
        expandableGroupEco.add(carousel);
        expandableHeaderEco.setTitle(ecoCounter);

        progressBar.setVisibility(View.GONE);
    }

    private void UpdatingSwitchValueDevice(UserDevice userDevice, RecyclerViewMyGardenDeviceItemBinding binding) {

        // device params
        ApplicationUser user = PreferencesUtility.getUser(getContext());

        Map<String, String> params = new HashMap<>();
        params.put("Id", userDevice.getId().toString());
        params.put("Name", userDevice.getName());
        params.put("isActive", userDevice.getIsActive().toString());
        params.put("notifyForWind", "" + binding.switchMyGardenDeviceStorm.isChecked());
        params.put("notifyForFrost", "" + binding.switchMyGardenDeviceFrost.isChecked());
        params.put("Note", userDevice.getNote());
        params.put("Gardenid", userDevice.getGardenid().toString());
        params.put("AdminDevId", userDevice.getAdminDevId().toString());
        params.put("CreatedBy", userDevice.getCreatedBy());
        params.put("EditedBy", user.getEmail());
        params.put("UserDevListId", userDevice.getUserDevListId().toString());
        params.put("Date", userDevice.getDate());
        params.put("Count", userDevice.getCount().toString());
        params.put("Todos", userDevice.getTodos() == null ? "" : userDevice.getTodos().toString());
        params.put("showMenu", "true");

        String updateDeviceUrl = APP_URL.DEVICE_API + "update/" + userDevice.getId();

        JSONObject ObjParams = new JSONObject(params);
        RequestQueueSingleton.getInstance(getContext()).objectRequest(updateDeviceUrl, Request.Method.PUT, this::onSuccessUpdate, this::onErrorPost, ObjParams);
    }

    private void UpdatingSwitchValueEcoElement(EcoElement ecoElement) {
        Log.d(TAG, "UpdatingSwitchValueEcoElement: Gesetzelt");
        ecoElement.setChecked(!ecoElement.getChecked());
        String updateEcoElementsUrl = APP_URL.USER_GARDEN_API + "updateecoelements";
        Map<String, String> params = new HashMap<>();
        params.put("Id", ecoElement.getId().toString());
        params.put("Checked", ecoElement.getChecked().toString());

        JSONObject ObjParams = new JSONObject(params);
        RequestQueueSingleton.getInstance(getContext()).objectRequest(updateEcoElementsUrl, Request.Method.PUT, this::onSuccessUpdate, this::onErrorPost, ObjParams);
    }

    public void showPlantMenuOptions(View v, MyGarden mMyGarden) {
        PopupMenu popup = new PopupMenu(getContext(), v);

        Bundle bundle = new Bundle();
        String myGardenJsonString = ApiUtils.getGsonParser().toJson(mMyGarden);
        bundle.putString("MY_GARDEN", myGardenJsonString);

        popup.setOnMenuItemClickListener(item -> {
            switch (item.getItemId()) {
                case R.id.move_plant:

                    bundle.putInt("VIEW_TYPE", R.string.myGarden_movePlant);
                    navigateToFragment(R.id.nav_controller_my_garden_edit_plant, (Activity) getContext(), false, bundle);

                    return true;
                case R.id.plant_note:
                    bundle.putInt("VIEW_TYPE", R.string.myGarden_notesForThePlant);
                    navigateToFragment(R.id.nav_controller_my_garden_edit_plant, (Activity) getContext(), false, bundle);

                    return true;
                case R.id.specify_number:
                    bundle.putInt("VIEW_TYPE", R.string.myGarden_specifyNumber);
                    navigateToFragment(R.id.nav_controller_my_garden_edit_plant, (Activity) getContext(), false, bundle);

                    return true;
                case R.id.delete_plant:

                    CustomBottomSheet customBottomSheet = new CustomBottomSheet(R.string.all_delete, mMyGarden, "Willst du " + mMyGarden.getUserPlant().getName() + " wirklich aus deinem Garten löschen?",
                            "Ja", onBottomSheetClickListener);
                    customBottomSheet.show(getActivity().getSupportFragmentManager(), "BottomSheet");

                    return true;
                default:
                    return false;
            }
        });

        popup.inflate(R.menu.menu_mygarden_plantoption);
        popup.show();
    }

    private final CustomBottomSheet.OnBottomSheetClickListener<MyGarden> onBottomSheetClickListener = (stringId, mMyGarden) -> {
        if (stringId == R.string.all_delete) {// remove from api
            String plantDeleteUrl = APP_URL.USER_PLANT_API + "deleteUserPlantFromAllUserList/" + mMyGarden.getUserPlant().getPlantId() + "/" + mMyGarden.getUserPlant().getGardenid();
            RequestQueueSingleton.getInstance(getContext()).stringRequest(plantDeleteUrl, Request.Method.DELETE, this::plantDeleteSuccess, null, null);
            progressBar.setVisibility(View.VISIBLE);
        }
    };

    private void plantDeleteSuccess(String jsonObject) {
        displayAlertDialog(getContext(), getResources().getString(R.string.myGarden_deletingPlantSuccessful));
        // getMyGardenPlantsFromApi(apiUrl);
    }

    private void UpdateApiDeviceCount(UserDevice userDevice, int i) {
        ApplicationUser user = PreferencesUtility.getUser(getContext());

        Map<String, String> params = new HashMap<>();
        params.put("Id", userDevice.getId().toString());
        params.put("Name", userDevice.getName());
        params.put("isActive", userDevice.getIsActive().toString());
        params.put("notifyForWind", userDevice.getNotifyForWind().toString());
        params.put("notifyForFrost", userDevice.getNotifyForFrost().toString());
        params.put("Note", userDevice.getNote());
        params.put("Gardenid", userDevice.getGardenid().toString());
        params.put("AdminDevId", userDevice.getAdminDevId().toString());
        params.put("CreatedBy", userDevice.getCreatedBy());
        params.put("EditedBy", user.getEmail());
        params.put("UserDevListId", userDevice.getUserDevListId().toString());
        params.put("Date", userDevice.getDate());
        params.put("Count", String.valueOf(i));
        params.put("Todos", userDevice.getTodos() == null ? "" : userDevice.getTodos().toString());
        params.put("showMenu", "true");

        JSONObject ObjParams = new JSONObject(params);

        RequestQueueSingleton.getInstance(getContext()).objectRequest(APP_URL.UPDATE_DEVICE_COUNT_API, Request.Method.PUT, this::onSuccessUpdateDevice, null, ObjParams);
    }


    private Group makeEcoElementsCarouselGroup() {
        int betweenPadding = getContext().getResources().getDimensionPixelSize(R.dimen.marginPaddingSize_8sdp);
        CarouselItemDecoration carouselDecoration = new CarouselItemDecoration(0, betweenPadding);
        GroupAdapter carouselAdapter = new GroupAdapter();
        for (int i = 0; i < ecoElementArrayList.size(); i++) {
            carouselAdapter.add(new EcoElementCarouselItem(getContext(), ecoElementArrayList.get(i), onEcoClickListener));
        }
        return new CarouselGroup(carouselDecoration, carouselAdapter);
    }

    private void getDevicesFromApi() {
        progressBar.setVisibility(View.VISIBLE);
        RequestQueueSingleton.getInstance(getContext()).typedRequest(APP_URL.DEVICE_API, this::onSuccessDevice, this::onError, UserDevice[].class, new RequestData(RequestType.UserDevice));

    }

    private void onSuccessDeleteDevice(String stringResponse) {
        displayAlertDialog(getContext(), getResources().getString(R.string.myGarden_deletingDeviceSuccessful));
        getDevicesFromApi();

    }

    private void onSuccessUpdateDevice(JSONObject jsonObject) {
        Log.e(TAG, jsonObject.toString());
    }

    private void onErrorPost(VolleyError error) {
        showErrorDialogNetworkParsed(getContext(), error);
        progressBar.setVisibility(View.GONE);
    }

    private void onSuccessUpdate(JSONObject jsonObject) {
        Log.d(TAG, "" + jsonObject.toString());
        progressBar.setVisibility(View.GONE);

    }

    private void AddPlantDialog() {

        new GenericDialog.Builder(getContext())
                .addNewButton(R.style.TransparentButtonStyle, getContext().getResources().getString(R.string.myGarden_addPlantViaPlantList),
                        R.dimen.textSize_body_small, view -> {
                            navigateToFragment(R.id.nav_controller_plant_search, getActivity(), false, null);
                        })
                .addNewButton(R.style.TransparentButtonStyle, getContext().getResources().getString(R.string.myGarden_addPlantViaPlantScan),
                        R.dimen.textSize_body_small, view -> {
                            navigateToFragment(R.id.nav_controller_plant_scan, getActivity(), false, null);
                        })
                .setButtonOrientation(LinearLayout.VERTICAL)
                .setCancelable(true)
                .generate();
    }

    public void RestorePreviousState() {
        // populate expandable lists
        if (groupAdapter != null) {
            SetupGroupAdapter();

            switch (expandedMenuState) {
                case R.string.myGarden_addNewDeviceAccessories:
                    getDevicesFromApi();
                    expandableGroupDevices.onToggleExpanded();
                    break;
                case R.string.myGarden_addNewList:
                    getUserGardenListFromApi();
                    expandableGroupListen.onToggleExpanded();
                    break;
                case R.string.all_ecoElements:
                    getEcoElementsFromApi();
                    expandableGroupEco.onToggleExpanded();
                    break;
                case R.string.all_calendar:
                    progressBar.setVisibility(View.VISIBLE);
                    loadPlantsFromApiInfiniteScroll(baseApiUrl);
                    break;
            }
        }
        //reset state
        persistDataViewModel.setMyGardenState(0);
    }

    private void showDeleteUserListAlertDialog(UserGarden userGarden) {

        new GenericDialog.Builder(getContext())
                .setTitle(userGarden.getName() + " " + getResources().getString(R.string.all_delete))
                .setTitleAppearance(R.color.text_all_gunmetal, R.dimen.textSize_body_medium)
                .setMessage(getResources().getString(R.string.myGarden_deletingWarning))
                .addNewButton(R.style.PrimaryButtonStyle,
                        getResources().getString(R.string.myGarden_moveToAnotherList), R.dimen.textSize_body_small, view -> {
                            showMoveUserGardenListAlertDialog(userGarden);
                        })
                .addNewButton(R.style.PrimaryWarningButtonStyle,
                        getResources().getString(R.string.myGarden_deleteImmediately), R.dimen.textSize_body_small, view -> {
                            String deleteDeviceUrl = APP_URL.USER_LIST_API + userGarden.getGardenId() + "/" + userGarden.getId();
                            RequestQueueSingleton.getInstance(getContext()).stringRequest(deleteDeviceUrl, Request.Method.DELETE, this::onSuccessDeleteUserGardenList, null, null);
                        })
                .setButtonOrientation(LinearLayout.VERTICAL)
                .setCancelable(true)
                .generate();
    }

    private void showMoveUserGardenListAlertDialog(UserGarden _userGarden) {
        AlertDialog.Builder dialogBuilderSpinner = new AlertDialog.Builder(getContext());
        LayoutInflater inflater = this.getLayoutInflater();

        //customizing the buttons and title
        View spinnerDialogView = inflater.inflate(R.layout.popup_dialog_default_spinner, null);
        Button confirmButton = spinnerDialogView.findViewById(R.id.button_popup_spinner_dialog_confirm);
        TextView headerTextView = spinnerDialogView.findViewById(R.id.button_popup_spinner_dialog_header);
        TextView descTextView = spinnerDialogView.findViewById(R.id.button_popup_spinner_dialog_description);
        Spinner spinnerUserGarden = spinnerDialogView.findViewById(R.id.spinner_dialog);

        headerTextView.setText(getResources().getString(R.string.myGarden_movePlant));
        descTextView.setText(getResources().getString(R.string.myGarden_moveToFollowingList));
        confirmButton.setText(getResources().getString(R.string.myGarden_moveAndDelete));

        // populate spinner
        List<UserGarden> spinnerUserGardenList = new ArrayList<>();
        spinnerUserGardenList.addAll(userGardenList);
        spinnerUserGardenList.remove(_userGarden);

        //position 0 spinner item
        UserGarden userGardenDefault = new UserGarden(0, "_");
        spinnerUserGardenList.add(0, userGardenDefault);

        ArrayAdapter<UserGarden> countryArrayAdapter = new ArrayAdapter<>(getContext(), R.layout.custom_spinner_item, spinnerUserGardenList);
        spinnerUserGarden.setAdapter(countryArrayAdapter);

        //dialog builder
        dialogBuilderSpinner.setView(spinnerDialogView);
        AlertDialog spinnerDialog = dialogBuilderSpinner.create();
        spinnerDialog.show();

        // button click listeners
        confirmButton.setOnClickListener(v -> {
            UserGarden selectedGarden = (UserGarden) spinnerUserGarden.getSelectedItem();
            int selectedGardenId = selectedGarden.getId();
            if (selectedGardenId != 0) {
                String deleteMovePlantsApi = APP_URL.USER_PLANT_API + "moveAllPlants";

                HashMap<String, String> hashMap = new HashMap<>();
                hashMap.put("CurrentListId", String.valueOf(_userGarden.getId()));
                hashMap.put("NewListId", String.valueOf(selectedGardenId));

                JSONObject jsonObject = new JSONObject(hashMap);

                RequestQueueSingleton.getInstance(getContext()).stringRequest(deleteMovePlantsApi, Request.Method.POST,
                        this::onSuccessDeleteUserGardenList, null, jsonObject);
                progressBar.setVisibility(View.VISIBLE);
                spinnerDialog.dismiss();
            }
        });
    }
    private void onSuccessDeleteUserGardenList(String response) {
        progressBar.setVisibility(View.GONE);
        displayAlertDialog(getContext(), getResources().getString(R.string.myGarden_deletingListSuccessful));
        selectedUserGardenListId = 0; // reset selected list
        baseApiUrl = APP_URL.USER_PLANT_API + "UserPlantByUserListId/" + selectedUserGardenListId;
        //todo fix
        loadPlantsFromApiInfiniteScroll(baseApiUrl);
        getUserGardenListFromApi();
    }

    public void onError(Exception ex, RequestData data) {
        if (getActivity() != null) {
            Resources res = getActivity().getResources();
            String network = res.getString(R.string.all_network);
            String anErrorOccurred = res.getString(R.string.all_anErrorOccurred);
            Log.e(network, anErrorOccurred + ex.toString());
            progressBar.setVisibility(View.GONE);
        }
    }

    /**
     * MyGardenPersistDataViewModel is used for data Persistence between MyGarden SubFragments
     **/

    MyGardenPersistDataViewModel persistDataViewModel;

    @Override
    public void onActivityCreated(@Nullable Bundle savedInstanceState) {
        super.onActivityCreated(savedInstanceState);
        persistDataViewModel = ViewModelProviders.of(getActivity()).get(MyGardenPersistDataViewModel.class);
        persistDataViewModel.getMyGardenState().observe(getViewLifecycleOwner(), _menuState -> expandedMenuState = _menuState);
        persistDataViewModel.getMyGardenList().observe(getViewLifecycleOwner(), _myGardenList -> {
            loadedGardenList = _myGardenList;
            RestorePreviousState();
        });
    }

    /**
     * Check if user entered info (either by authenticating or by entering the data manually)
     * exists. If it doesn't, redirect to LoginFragment.
     **/

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