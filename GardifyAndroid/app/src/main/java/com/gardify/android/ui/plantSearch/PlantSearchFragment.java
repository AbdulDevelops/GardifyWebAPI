package com.gardify.android.ui.plantSearch;

import android.graphics.Color;
import android.net.Uri;
import android.os.Bundle;
import android.os.Handler;
import android.util.Log;
import android.util.TypedValue;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.cardview.widget.CardView;
import androidx.fragment.app.Fragment;
import androidx.lifecycle.ViewModelProviders;
import androidx.recyclerview.widget.GridLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.android.volley.Request;
import com.gardify.android.R;
import com.gardify.android.data.misc.RequestData;
import com.gardify.android.data.myGarden.MyGarden;
import com.gardify.android.data.plantSearchFilterData.Cats;
import com.gardify.android.data.plantSearchFilterData.PlantGroup;
import com.gardify.android.data.plantSearchFilterData.PlantTags;
import com.gardify.android.data.plantSearchModel.Plant;
import com.gardify.android.data.plantSearchModel.PlantsSearchModel;
import com.gardify.android.ui.generic.ExpandableHeaderItem;
import com.gardify.android.ui.generic.ExpandableInnerHeaderItem;
import com.gardify.android.ui.generic.ExpandableInputHeaderItem;
import com.gardify.android.ui.generic.ExpandableSearchInputHeaderItem;
import com.gardify.android.ui.generic.InfiniteScrollListener;
import com.gardify.android.ui.generic.interfaces.OnExpandableHeaderListener;
import com.gardify.android.ui.generic.interfaces.OnExpandableInputHeaderListener;
import com.gardify.android.ui.generic.recyclerItem.CardItemNoResult;
import com.gardify.android.ui.generic.recyclerItem.CardViewTopBottomSection;
import com.gardify.android.ui.myGarden.recyclerItems.PlantEcoFilterCardItem;
import com.gardify.android.ui.plantSearch.recyclerItems.DetailSearchCardItem;
import com.gardify.android.ui.plantSearch.recyclerItems.DetailSearchColumnItem;
import com.gardify.android.ui.plantSearch.recyclerItems.FrostHadyCardItem;
import com.gardify.android.ui.plantSearch.recyclerItems.GardenGroupCardItem;
import com.gardify.android.ui.plantSearch.recyclerItems.PlantCardItem;
import com.gardify.android.ui.plantSearch.recyclerItems.PlantFamilyCardItem;
import com.gardify.android.ui.plantSearch.recyclerItems.PlantGroupCardItem;
import com.gardify.android.ui.plantSearch.recyclerItems.RangeSliderCardItem;
import com.gardify.android.ui.plantSearch.recyclerItems.TitleItemCard;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.PreferencesUtility;
import com.gardify.android.utils.RequestQueueSingleton;
import com.gardify.android.utils.RequestType;
import com.gardify.android.viewModelData.BadgesIconVM;
import com.google.android.material.chip.Chip;
import com.google.android.material.chip.ChipGroup;
import com.xwray.groupie.ExpandableGroup;
import com.xwray.groupie.Group;
import com.xwray.groupie.GroupAdapter;
import com.xwray.groupie.Section;

import org.jetbrains.annotations.NotNull;
import org.json.JSONArray;
import org.json.JSONException;

import java.io.UnsupportedEncodingException;
import java.net.URLDecoder;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collections;
import java.util.Comparator;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Optional;
import java.util.stream.Collectors;

import static com.gardify.android.ui.home.HomeFragment.RECYCLE_VIEW_DEFAULT_SPAN_COUNT;
import static com.gardify.android.ui.plantSearch.PlantSearchFragment.FilterType.autumnColorsTag;
import static com.gardify.android.ui.plantSearch.PlantSearchFragment.FilterType.colorsTag;
import static com.gardify.android.ui.plantSearch.PlantSearchFragment.FilterType.cookieTag;
import static com.gardify.android.ui.plantSearch.PlantSearchFragment.FilterType.excludeTag;
import static com.gardify.android.ui.plantSearch.PlantSearchFragment.FilterType.leafColorsTag;
import static com.gardify.android.ui.plantSearch.PlantSearchFragment.FilterType.slider;
import static com.gardify.android.ui.plantSearch.recyclerItems.RangeSliderCardItem.PLANT_MAX_HEIGHT;
import static com.gardify.android.ui.plantSearch.recyclerItems.RangeSliderCardItem.PLANT_MIN_HEIGHT;
import static com.gardify.android.ui.saveToGarden.SaveToGardenFragment.PLANT_ID_ARG;
import static com.gardify.android.ui.saveToGarden.SaveToGardenFragment.PLANT_NAME_ARG;
import static com.gardify.android.utils.ApiUtils.startCountFromZero;
import static com.gardify.android.utils.StringUtils.getShortMonthNameByNumber;
import static com.gardify.android.utils.UiUtils.displayAlertDialog;
import static com.gardify.android.utils.UiUtils.navigateToFragment;
import static com.gardify.android.utils.UiUtils.setupToolbar;
import static com.gardify.android.viewModelData.BadgesIconVM.EcoBadges;
import static java.util.stream.Collectors.toList;

public class PlantSearchFragment extends Fragment {

    private static final String TAG = "PlantSearchFragment";

    public static final String JAN_MONTH_VALUE = "1";
    public static final String DEC_MONTH_VALUE = "12";
    public static final int FIX_DETAIL_SEARCH_COUNT = 286;
    public static final int FIX_FROST_HARDY_COUNT = 9;
    public static final int CONDITIONALLY_POISONOUS_ID = 335;
    public static final int HIGHLY_POISONOUS_ID = 560;
    //Arguments
    public static final String SEARCH_TEXT_ARG = "SEARCHED_TEXT";
    private static final int RECYCLE_VIEW_SPAN_COUNT = 12;

    private static int selectedFilterCounter = 0;
    private ChipGroup chipGroup;
    private Chip searchNameChip;
    private List<Chip> chipList = new ArrayList<>();
    private List<Chip> searchNameChipList = new ArrayList<Chip>();
    //private int pageNumber = 0;
    private int totalPageCount = 0;
    //filter params
    private String paramSearchedText;
    private final List<Integer> paramPlantGroups = new ArrayList<>();
    private final List<Integer> paramGardenGroups = new ArrayList<>();
    private final List<String> paramPlantFamilies = new ArrayList<>();
    private final List<String> paramEcoTags = new ArrayList<>();
    private Map<String, String> paramFreezes;
    // detail search params
    private final List<PlantTags> paramCookieTags = new ArrayList<>();
    private final List<PlantTags> paramColorsTags = new ArrayList<>();
    private final List<PlantTags> paramExcludeTags = new ArrayList<>();
    private final List<PlantTags> paramLeafColorsTags = new ArrayList<>();
    private final List<PlantTags> paramAutumnColorsTags = new ArrayList<>();
    private final List<Integer> paramMonthTag = new ArrayList<>();
    private final List<Integer> paramHeightTag = new ArrayList<>();
    //groupie adapter
    private GroupAdapter groupAdapter;
    private RecyclerView plantsRecycleView;
    private ProgressBar progressBar;
    private TextView textViewHit;
    private TextView textViewSearchCriteria;
    private GridLayoutManager layoutManager;
    //filter params
    private List<BadgesIconVM> badgesIconVMList = null;
    private List<PlantGroup.Group> plantGroupsList;
    private List<PlantGroup.GardenGroup> gardenGroupsList;
    private List<Plant> loadedPlantsList;
    private List<String> plantFamilyList;
    private List<Cats> categoriesList;
    private List<PlantTags> tagsList;
    //search criteria
    private boolean isSearchCriteriaVisible = true;
    private CardView searchHitCardView, searchCriteriaCardView;
    private Section updateWithPayloadSection, noSearchResultSection;
    private HashMap<String, FilterType> filterList = new HashMap<String, FilterType>();

    //infinite scroll
    private List<String> requestedPagesList = new ArrayList<>();
    private static final int PLANT_TAKE_COUNT = 10;
    List<Group> plantPayloadList = new ArrayList<>();

    List<String> userPlantList = new ArrayList<>();

    //viewmodel for chipgroup
    private PlantSearchPersistDataViewModel plantSearchPersistDataViewModel;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        if (getArguments() != null) {
            paramSearchedText = getArguments().getString(SEARCH_TEXT_ARG);
        }

        loadUserPlants();
    }

    public View onCreateView(@NonNull LayoutInflater inflater,
                             ViewGroup container, Bundle savedInstanceState) {

        View root = inflater.inflate(R.layout.fragment_plant_search, container, false);
        setupToolbar(getActivity(), "PFLANZEN SUCHE", R.drawable.gardify_app_icon_pflanzensuche, R.color.toolbar_plantSearch_setupToolbar, true);

        //initializing views
        init(root);

        //setApiRequest();
        setupGroupAdapter();
        return root;
    }

    private void setupGroupAdapter() {
        groupAdapter = new GroupAdapter();
        groupAdapter.clear();
        groupAdapter.setSpanCount(RECYCLE_VIEW_DEFAULT_SPAN_COUNT);
        layoutManager = new GridLayoutManager(getContext(), groupAdapter.getSpanCount());
        layoutManager.setSpanSizeLookup(groupAdapter.getSpanSizeLookup());
        plantsRecycleView.setLayoutManager(layoutManager);
        populateAdapter();
        plantsRecycleView.setAdapter(groupAdapter);
    }

    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);

    }

    public void init(View root) {
        /* finding views block */
        plantsRecycleView = root.findViewById(R.id.recyclerView_plant_search);
        progressBar = root.findViewById(R.id.progressBar_plant_search);
        ImageView visibilityImgSearchCriteria = root.findViewById(R.id.image_view_plant_search_critera_visibility);
        ImageView goToTopImage = root.findViewById(R.id.image_view_plant_search_go_to_top);
        ImageView resetSearch = root.findViewById(R.id.imageView_plant_search_reset_search);
        searchHitCardView = root.findViewById(R.id.cardview_plant_search_hit);
        searchCriteriaCardView = root.findViewById(R.id.cardview_plant_search_criteria);
        textViewHit = root.findViewById(R.id.textView_plant_search_hit);
        textViewSearchCriteria = root.findViewById(R.id.textView_plant_search_search_criteria);
        chipGroup = root.findViewById(R.id.chipGroup_plant_search);

        visibilityImgSearchCriteria.setOnClickListener(searchCriteriaVisibilityListener());
        goToTopImage.setOnClickListener(goToTopListener());
        resetSearch.setOnClickListener(resetSearchListener());
    }

    private ExpandableGroup expandableGroupPlantGroups;
    private ExpandableGroup expandableGroupGardenGroup;
    private ExpandableGroup expandableGroupPlantFamily;
    private ExpandableGroup expandableGroupDetailSearch;
    private ExpandableInputHeaderItem expandableInputHeaderPlantGroups, expandableInputHeaderPlantFamily, expandableInputHeaderGardenGroups;
    private ExpandableSearchInputHeaderItem expandableSearchInputHeaderPlantSearch;

    private ExpandableHeaderItem expandableHeaderPlantEcoCriteria, expandableHeaderPlantFrostHaerte,
            expandableHeaderPlantDetailSearch;

    private void populateAdapter() {

        // section with background
        Section cardViewTopSection = new Section(new CardViewTopBottomSection(true));
        Section cardViewBottomSection = new Section(new CardViewTopBottomSection(false));
        Section sectionWithBackground = new Section();
        sectionWithBackground.setHeader(cardViewTopSection);
        sectionWithBackground.setFooter(cardViewBottomSection);

        // Expandable input group plant names search
        expandableSearchInputHeaderPlantSearch = new ExpandableSearchInputHeaderItem(getContext(), R.color.text_all_gunmetal, R.color.expandableHeader_all_white, paramSearchedText, R.string.all_plantName, R.string.plantSearch_search, onExpandableInputHeaderListener);
        ExpandableGroup expandableGroupPlantNames = new ExpandableGroup(expandableSearchInputHeaderPlantSearch);
        sectionWithBackground.add(expandableGroupPlantNames);

        // Expandable group plant group
        expandableInputHeaderPlantGroups = new ExpandableInputHeaderItem(getContext(), R.color.text_all_gunmetal, R.color.expandableHeader_all_white, R.string.all_plantsGroup, R.string.plantSearch_evergreen, onExpandableInputHeaderListener);
        expandableGroupPlantGroups = new ExpandableGroup(expandableInputHeaderPlantGroups);
        sectionWithBackground.add(expandableGroupPlantGroups);

        // Expandable group plant family
        expandableInputHeaderPlantFamily = new ExpandableInputHeaderItem(getContext(), R.color.text_all_gunmetal, R.color.expandableHeader_all_white, R.string.plantSearch_plantFamily, R.string.plantSearch_ericaceae, onExpandableInputHeaderListener);
        expandableGroupPlantFamily = new ExpandableGroup(expandableInputHeaderPlantFamily);
        sectionWithBackground.add(expandableGroupPlantFamily);

        // Expandable plant horticulture group
        expandableInputHeaderGardenGroups = new ExpandableInputHeaderItem(getContext(), R.color.text_all_gunmetal, R.color.expandableHeader_all_white, R.string.plantSearch_HorticulturalGroup, R.string.plantSearch_roses, onExpandableInputHeaderListener);
        expandableGroupGardenGroup = new ExpandableGroup(expandableInputHeaderGardenGroups);
        sectionWithBackground.add(expandableGroupGardenGroup);
        groupAdapter.add(sectionWithBackground);

        // Expandable group plant eco criteria
        badgesIconVMList = EcoBadges;
        expandableHeaderPlantEcoCriteria = new ExpandableHeaderItem(getContext(), R.color.text_all_white, R.color.expandableHeader_plantSearch_ecoFilter, R.string.all_ecologyCriteria, 0, onExpandableHeaderListener);
        ExpandableGroup expandableGroupEcoCriteria = new ExpandableGroup(expandableHeaderPlantEcoCriteria);
        for (BadgesIconVM badgesIconVM : badgesIconVMList) {
            expandableGroupEcoCriteria.add(new PlantEcoFilterCardItem(getContext(), badgesIconVM, true, onFilterEcoClickListener));
        }
        groupAdapter.add(expandableGroupEcoCriteria);

        // Expandable group plant FrostHaerte
        expandableHeaderPlantFrostHaerte = new ExpandableHeaderItem(getContext(), R.color.text_all_white, R.color.expandableHeader_plantSearch_frostHardy, R.string.plantSearch_frostHardness, 0, onExpandableHeaderListener);
        ExpandableGroup expandableGroupFrostHaerte = new ExpandableGroup(expandableHeaderPlantFrostHaerte);
        expandableGroupFrostHaerte.add(new FrostHadyCardItem(getContext(), onFrostHardClickListener));

        groupAdapter.add(expandableGroupFrostHaerte);

        // Expandable group plant detail search
        expandableHeaderPlantDetailSearch = new ExpandableHeaderItem(getContext(), R.color.text_all_white, R.color.expandableHeader_plantSearch_detailSearch, R.string.plantSearch_detailedSearch, 0, onExpandableHeaderListener);
        expandableGroupDetailSearch = new ExpandableGroup(expandableHeaderPlantDetailSearch);
        groupAdapter.add(expandableGroupDetailSearch);

        //no search result group
        noSearchResultSection = new Section();

        // Main recycleView Update with payload
        updateWithPayloadSection = new Section();
        groupAdapter.add(updateWithPayloadSection);
        loadedPlantsList = new ArrayList<>();
        loadPlantsFromApiInfiniteScroll();

        // expandable header item counter
        getExpandableHeaderCount();
    }

    private void loadPlantsFromApiInfiniteScroll() {
        resetInfiniteScroll();
        loadedPlantsList.clear();

        plantsRecycleView.addOnScrollListener(new InfiniteScrollListener(layoutManager) {
            @Override
            public void onLoadMore(int currentPage) {
                Log.d(TAG, "pageCount : " + currentPage);
                currentPage = startCountFromZero(currentPage);
                String plantSearchUrl = getBuilderModifiedUrl(APP_URL.PLANT_SEARCH) + "&take=" + PLANT_TAKE_COUNT + "&skip=" + currentPage * PLANT_TAKE_COUNT;
                if (hasMorePlants(currentPage)) {
                    if (!isUrlLoaded(plantSearchUrl)) {
                        requestedPagesList.add(plantSearchUrl);
                        progressBar.setVisibility(View.VISIBLE);
                        RequestQueueSingleton.getInstance(getContext()).typedRequest(plantSearchUrl, this::onSuccessPlants, this::onErrorPlants, PlantsSearchModel.class, new RequestData(RequestType.PflanzenSucheModel));
                    }
                }
            }

            private void onErrorPlants(Exception e, RequestData requestData) {
                displayAlertDialog(getContext(), "Keine passende Pflanze gefunden.");
            }

            private void onSuccessPlants(PlantsSearchModel model, RequestData data) {
                if (!isVisible()) {
                    return;
                }
                List<Plant> plantNextList = model.getPlants();
                Handler handler = new Handler();
                handler.postDelayed(new Runnable() {
                    @Override
                    public void run() {
                        updatePayloadSection(plantNextList);
                        progressBar.setVisibility(View.GONE);
                        // updateComponentsAssociatedWithMyGarden();
                        loadedPlantsList.addAll(plantNextList);
                    }
                }, 1000);


                // UpdateSearchCriteriaLabel();
            }

        });

        getTotalCountFromApi();

    }

    private void loadUserPlants() {
        String paramUrl = APP_URL.USER_PLANT_API + "?skip=0&take=1000";
        Log.d(TAG, "loadUserPlants: ");
        RequestQueueSingleton.getInstance(getContext()).typedRequest(paramUrl, this::onSuccessUserPlant, this::onErrorUserPlants, MyGarden[].class, new RequestData(RequestType.MyGarden));
    }

    private void onSuccessUserPlant(MyGarden[] model, RequestData requestData) {
        if (!isVisible())
            return;
        if (model.length > 0) {
            List<MyGarden> myGardenNextList = Arrays.asList(model);
            for (int i = 0; i < model.length; i++) {
                Log.d(TAG, "onSuccessUserPlant: " + myGardenNextList.get(i).getUserPlant().getName());
                userPlantList.add(myGardenNextList.get(i).getUserPlant().getName());
            }
        }
    }

    private void onErrorUserPlants(Exception e, RequestData requestData) {
        Toast.makeText(getContext(), "Fehler beim Laden! " + e.getMessage(), Toast.LENGTH_SHORT).show();
    }

    private boolean isUrlLoaded(String plantSearchUrl) {
        return requestedPagesList.contains(plantSearchUrl);
    }

    private boolean hasMorePlants(int currentPage) {
        return loadedPlantsList.size() == (currentPage * PLANT_TAKE_COUNT);
    }

    private void getExpandableHeaderCount() {
        if (plantGroupsList == null) {
            getPlantGroupFromApi();
            getPlantFamilyFromApi();
        }
    }

    private void resetInfiniteScroll() {
        updateWithPayloadSection.removeAll(plantPayloadList);
        requestedPagesList.clear();
        //plantsRecycleView.smoothScrollToPosition(0);
        plantsRecycleView.clearOnScrollListeners();
    }

    private void updatePayloadSection(List<Plant> plantList) {
        plantPayloadList.clear();
        for (int i = 0; i < plantList.size(); i++) {
            Log.d(TAG, "updatePayloadSection: " + plantList.get(i).getNameGerman() + " userplantList: " + userPlantList);
            if(userPlantList.contains(plantList.get(i).getNameGerman()))
                plantPayloadList.add(new PlantCardItem(i, getContext(), plantList.get(i), true, onPlantClickListener));
            else
                plantPayloadList.add(new PlantCardItem(i, getContext(), plantList.get(i), false, onPlantClickListener));
        }
        updateWithPayloadSection.addAll(plantPayloadList);

        /**cursor does not jump around anymore when this gets deleted*/
        //groupAdapter.notifyDataSetChanged();

        //go to recyclerview position if plant details or save to garden clicked
        gotoRecyclerViewPosition();
    }

    private void updatePlantsList() {
        // Main recycleView Update with payload
        if (groupAdapter != null) {
            //  clearPayloadSection();
            updateWithPayloadSection = new Section();
            if (loadedPlantsList.size() > 0) {
                for (int i = 0; i < loadedPlantsList.size(); i++) {
                    updateWithPayloadSection.add(new PlantCardItem(i, getContext(), loadedPlantsList.get(i), false, onPlantClickListener));
                }
            } else {
                textViewHit.setText("0" + " Treffer");
                updateWithPayloadSection.add(new CardItemNoResult(getContext(), R.string.plantSearch_noResultMessage, R.string.menuActivityMainDrawer_suggestPlant, onCardClickListener));
            }

            groupAdapter.add(updateWithPayloadSection);
        }
    }

    private void getTotalCountFromApi() {
        // get total count
        String searchCountUrl = getBuilderModifiedUrl(APP_URL.PLANT_SEARCH_TOTAL_COUNT);
        RequestQueueSingleton.getInstance(getContext()).stringRequest(searchCountUrl, Request.Method.GET, this::onSuccessCount, null, null);
    }

    public void ClearData() {
        //Delete saved instances on fragment change
        //   updateWithPayloadSection = null;
        groupAdapter = null;
    }

    private final PlantCardItem.OnPlantClickListener onPlantClickListener = (plant, viewBinding, view, position) -> {
        // Pretend to make a network request

        if (viewBinding.buttonRecyclerViewPLantSearchPlantItemDetail.equals(view) ||
                viewBinding.imageViewPlantSearchPlantImage.equals(view)) {
            //save chip group
            plantSearchPersistDataViewModel.setChipListLiveData(chipList);
            plantSearchPersistDataViewModel.setIsFilterAppliedLiveData(true);
            //save scroll position
            plantSearchPersistDataViewModel.setRecyclerViewPositionLiveData(position);
            plantSearchPersistDataViewModel.setNeedToScrollLiveData(true);
            // Change fragment
            Bundle bundle = new Bundle();
            bundle.putString(PLANT_ID_ARG, String.valueOf(plant.getId()));
            navigateToFragment(R.id.nav_controller_plant_detail, getActivity(), false, bundle);
            ClearData();
        }

        if (viewBinding.buttonPlantSearchSaveToGarden.equals(view)) {

            //save chip group
            plantSearchPersistDataViewModel.setChipListLiveData(chipList);
            plantSearchPersistDataViewModel.setIsFilterAppliedLiveData(true);
            //save scroll position
            plantSearchPersistDataViewModel.setRecyclerViewPositionLiveData(position);
            plantSearchPersistDataViewModel.setNeedToScrollLiveData(true);
            // Change fragment
            Bundle bundle = new Bundle();
            bundle.putString(PLANT_ID_ARG, String.valueOf(plant.getId()));
            bundle.putString(PLANT_NAME_ARG, String.valueOf(plant.getNameGerman()));
            if (!PreferencesUtility.getLoggedIn(getContext()))
                navigateToFragment(R.id.nav_controller_login, getActivity(), false, bundle);
            else
                navigateToFragment(R.id.nav_controller_save_to_garden, getActivity(), false, bundle);
            ClearData();
        }

    };
    private final FrostHadyCardItem.OnFrostHardClickListener onFrostHardClickListener = (progressPosition, temperatureArray) -> {
        paramFreezes = new HashMap<>();
        if (progressPosition > 0) {
            List<String> frostHardInterval = getFrostHardIntervals();
            String frostHardy = getContext().getResources().getString(R.string.plantSearch_frostHardness);
            paramFreezes.put(frostHardInterval.get(progressPosition), frostHardy + " " + temperatureArray[progressPosition]);
        } else {
            paramFreezes.clear();
        }
        updateChipGroupTags();
        loadPlantsFromApiInfiniteScroll();

    };
    private final PlantGroupCardItem.OnPlantGroupClickListener onPlantGroupClickListener = (groupId) -> {
        // Pretend to make a network request

        paramPlantGroups.clear();
        paramPlantGroups.add(groupId);
        updateChipGroupTags();
        loadPlantsFromApiInfiniteScroll();
        expandableGroupPlantGroups.onToggleExpanded();

    };
    private final GardenGroupCardItem.OnGardenGroupClickListener onGardenGroupClickListener = (groupId) -> {
        // Pretend to make a network request

        paramGardenGroups.clear();
        paramGardenGroups.add(groupId);
        updateChipGroupTags();
        loadPlantsFromApiInfiniteScroll();
        expandableGroupGardenGroup.onToggleExpanded();

    };
    private final PlantFamilyCardItem.OnPlantFamilyClickListener onPlantFamilyClickListener = (plantFamilyName) -> {
        // Pretend to make a network request

        paramPlantFamilies.clear();
        paramPlantFamilies.add(plantFamilyName);
        updateChipGroupTags();
        loadPlantsFromApiInfiniteScroll();
        expandableGroupPlantFamily.onToggleExpanded();

    };
    private final CardItemNoResult.OnCardClickListener onCardClickListener = textResId -> {
        switch (textResId) {
            case R.string.plantSearch_proposePlantForDatabase:
                navigateToFragment(R.id.nav_controller_suggest_plants, getActivity(), true, null);
                ClearData();
                break;
        }
    };

    private final OnExpandableInputHeaderListener onExpandableInputHeaderListener = (stringId, typedString) -> {
        // Pretend to make a network request
        switch (stringId) {
            case R.string.all_plantName:

                Log.d("PlantSearchFrag:", typedString);
                if (typedString.length() > 0) {
                    paramSearchedText = typedString;
                } else {
                    paramSearchedText = "";
                }
                updateChipGroupTags();
                loadPlantsFromApiInfiniteScroll();
                break;
            case R.string.all_plantsGroup:
            case R.string.plantSearch_HorticulturalGroup:

                getPlantGroupFromApi();
                progressBar.setVisibility(View.VISIBLE);

                break;

            case R.string.plantSearch_plantFamily:
                getPlantFamilyFromApi();
                progressBar.setVisibility(View.VISIBLE);


                break;
        }
    };

    private void getPlantGroupFromApi() {
        String groupApi = APP_URL.PLANT_GROUP_API;
        RequestQueueSingleton.getInstance(getActivity()).typedRequest(groupApi, this::onSuccessGroups, null, PlantGroup.class, new RequestData(RequestType.PlantGroup));
    }

    private void getPlantFamilyFromApi() {
        String plantFamilyUrl = APP_URL.PLANT_FAMILY;
        RequestQueueSingleton.getInstance(getContext()).arrayRequest(plantFamilyUrl, Request.Method.GET, this::onSuccessPlantFamily, null, null);
    }

    private final PlantEcoFilterCardItem.OnFilterEcoClickListener onFilterEcoClickListener = (badgesVM, reset) -> {
        paramEcoTags.removeIf(s -> s.equals(badgesVM.getId()));
        paramEcoTags.add(badgesVM.getId());
        updateChipGroupTags();
        loadPlantsFromApiInfiniteScroll();
        //update list checked status
        updateBadgesCheckedStatus(badgesVM);
    };

    private void updateBadgesCheckedStatus(BadgesIconVM badgesVM) {
        badgesIconVMList.stream()
                .filter(obj -> obj.getId() == badgesVM.getId())
                .findFirst()
                .ifPresent(o -> o.setChecked(badgesVM.isChecked()));
    }

    private final RangeSliderCardItem.OnSliderChangeListener onSliderChangeListener = (sliderType, sliderValuesMonth, sliderValuesHeight) -> {

        switch (sliderType) {

            case R.string.plantSearch_floweringPeriod:
                if (sliderValuesMonth != null) {
                    paramMonthTag.clear();
                    paramMonthTag.add(Math.round(sliderValuesMonth.getValues().get(0)));
                    paramMonthTag.add(Math.round(sliderValuesMonth.getValues().get(1)));
                }
                break;
            case R.string.plantSearch_growthHeight:
                if (sliderValuesHeight != null) {
                    paramHeightTag.clear();
                    paramHeightTag.add(Math.round(sliderValuesHeight.getValues().get(0)));
                    paramHeightTag.add(Math.round(sliderValuesHeight.getValues().get(1)));
                }
                break;
        }

        updateChipGroupTags();
        loadPlantsFromApiInfiniteScroll();
    };
    private final DetailSearchCardItem.OnPlantTagListener onPlantTagListener = (plantTag) -> {
        // Pretend to make a network request

        for (Cats cats : categoriesList) {
            if (cats.getId() == plantTag.getCategoryId()) {
                Optional<DetailMenu> matchingObject = DetailMenu.menuList.stream().filter(p -> p.category.equalsIgnoreCase(cats.getTitle())).findFirst();
                if (matchingObject.isPresent()) {
                    DetailMenu selectionOption = matchingObject.get();

                    switch (selectionOption.filterType) {
                        case cookieTag:
                            Log.d("detail search", "cookie tag");
                            if (selectionOption.isMulti) {
                                DetailSearchMultiSelection(plantTag, paramCookieTags);
                            } else {
                                DetailSearchSingleSelection(plantTag, paramCookieTags);
                            }

                            break;
                        case colorsTag:
                            Log.d("detail search", "colorsTag");
                            DetailSearchMultiSelection(plantTag, paramColorsTags);

                            break;
                        case excludeTag:
                            Log.d("detail search", "excludeTag");
                            DetailSearchMultiSelection(plantTag, paramExcludeTags);

                            break;
                        case leafColorsTag:
                            Log.d("detail search", "leafColorsTag");
                            DetailSearchMultiSelection(plantTag, paramLeafColorsTags);

                            break;
                        case autumnColorsTag:
                            Log.d("detail search", "autumnColorsTag");
                            DetailSearchMultiSelection(plantTag, paramAutumnColorsTags);

                            break;
                        case slider:
                            Log.d("detail search", "slider");
                            break;
                        case plantGroup:
                            Log.d("detail search", "plantGroup");
                            break;
                        case plantFamily:
                            Log.d("detail search", "plantFamily");
                            break;
                        case ecoTag:
                            Log.d("detail search", "ecoTag");
                            break;
                    }


                }
            }
        }
        updateChipGroupTags();

    };
    private final OnExpandableHeaderListener onExpandableHeaderListener = (stringId) -> {
        // Pretend to make a network request
        switch (stringId) {

            case R.string.all_ecologyCriteria:
            case R.string.plantSearch_frostHardness:

                break;

            case R.string.plantSearch_detailedSearch:

                // api request for detailSearch categories and tags
                progressBar.setVisibility(View.VISIBLE);
                if (expandableGroupDetailSearch.isExpanded())
                    getPlantCategoriesFromApi();
                else
                    loadPlantsFromApiInfiniteScroll();


                break;
        }
    };

    private void getPlantCategoriesFromApi() {
        String catsUrl = APP_URL.PLANT_CATS_API;
        RequestQueueSingleton.getInstance(getActivity()).typedRequest(catsUrl, this::onSuccessCategories, null, Cats[].class, new RequestData(RequestType.Cats));
    }

    @NotNull
    private View.OnClickListener resetSearchListener() {
        return v -> {
            //clear search parameters
            navigateToFragment(R.id.nav_controller_plant_search, getActivity(), true, null);
        };
    }

    @NotNull
    private View.OnClickListener goToTopListener() {
        return v -> {
            layoutManager.smoothScrollToPosition(plantsRecycleView, null, 0);
        };
    }

    @NotNull
    private View.OnClickListener searchCriteriaVisibilityListener() {
        return v -> {
            if (!isSearchCriteriaVisible) {
                searchCriteriaCardView.setVisibility(View.VISIBLE);
                searchHitCardView.setVisibility(View.VISIBLE);
                isSearchCriteriaVisible = true;
            } else {
                searchCriteriaCardView.setVisibility(View.GONE);
                searchHitCardView.setVisibility(View.GONE);
                isSearchCriteriaVisible = false;
            }
        };
    }

    private void onSuccessCategories(Cats[] model, RequestData data) {

        categoriesList = Arrays.asList(model);
        getPlantTagsFromApi();

    }

    private void getPlantTagsFromApi() {
        // Load child category tags
        String tagsUrl = APP_URL.PLANT_TAGS_API;
        RequestQueueSingleton.getInstance(getActivity()).typedRequest(tagsUrl, this::onSuccessTags, null, PlantTags[].class, new RequestData(RequestType.PlantTags));
    }

    private void onSuccessTags(PlantTags[] model, RequestData data) {

        if (tagsList == null)
            tagsList = Arrays.asList(model);
        setExpandableHeaderCounts();
        PopulateDetailCategoriesAndTagsList();

        progressBar.setVisibility(View.GONE);

    }

    private void PopulateDetailCategoriesAndTagsList() {
        clearExpandAbleGroup(expandableGroupDetailSearch);

        //delete extra cats that are not in menuList
        categoriesList = categoriesList.stream()
                .filter(cat -> DetailMenu.menuList.stream()
                        .anyMatch(detailMenu -> detailMenu.category.equalsIgnoreCase(cat.getTitle())))
                .collect(toList());

        // order list
        Collections.sort(categoriesList, Comparator.comparing(item ->
                DetailMenu.menuListOrder.indexOf(item.getTitle())));

        expandableGroupDetailSearch.add(new TitleItemCard(getContext(), R.color.expandableGroup_all_whiteSmoke, R.string.plantSearch_topFilter, null));
        for (PlantTags plantTag : tagsList) {
            if (plantTag.getTitle().equalsIgnoreCase("immergrün") || plantTag.getTitle().equalsIgnoreCase("Sichtschutz")) {
                expandableGroupDetailSearch.add(new DetailSearchColumnItem(getContext(), plantTag, false, "", onPlantTagListener));
            }
        }

        for (Cats cat : categoriesList) {
            Optional<DetailMenu> matchingObject = DetailMenu.menuList.stream().filter(p -> p.category.equalsIgnoreCase(cat.getTitle())).findFirst();

            DetailMenu ObjectDetailMenu = null;
            if (matchingObject.isPresent()) {
                ObjectDetailMenu = matchingObject.get();
            }


            if (cat.getTitle().equalsIgnoreCase("Licht")) {
                expandableGroupDetailSearch.add(new TitleItemCard(getContext(), R.color.expandableGroup_all_whiteSmoke, R.string.plantSearch_location, null));

                for (PlantTags plantTag : tagsList) {
                    if (cat.getId() == plantTag.getCategoryId()) {
                        expandableGroupDetailSearch.add(new DetailSearchColumnItem(getContext(), plantTag, false, "", onPlantTagListener));
                    }
                }

                // set sliders for Blühdauer and Wuchshöhe after "Licht" at position 1, 2, 3, 4 respectively
                expandableGroupDetailSearch.add(new TitleItemCard(getContext(), R.color.expandableGroup_all_whiteSmoke, R.string.plantSearch_floweringPeriod, null));
                expandableGroupDetailSearch.add(new RangeSliderCardItem(getContext(), R.string.plantSearch_floweringPeriod, onSliderChangeListener));

                expandableGroupDetailSearch.add(new TitleItemCard(getContext(), R.color.expandableGroup_all_whiteSmoke, R.string.plantSearch_growthHeight, null));
                expandableGroupDetailSearch.add(new RangeSliderCardItem(getContext(), R.string.plantSearch_growthHeight, onSliderChangeListener));


            } else if (cat.getTitle().equalsIgnoreCase("Verwendung")) {
                expandableGroupDetailSearch.add(new TitleItemCard(getContext(), R.color.expandableGroup_all_whiteSmoke, R.string.plantSearch_use, null));

                for (PlantTags plantTag : tagsList) {
                    if (cat.getId() == plantTag.getCategoryId()) {
                        expandableGroupDetailSearch.add(new DetailSearchColumnItem(getContext(), plantTag, false, "", onPlantTagListener));
                    }
                }
            } else if (cat.getTitle().equalsIgnoreCase("Blütenfarben")) {
                expandableGroupDetailSearch.add(new TitleItemCard(getContext(), R.color.expandableGroup_all_whiteSmoke, R.string.plantSearch_blossomColored, null));

                for (PlantTags plantTag : tagsList) {
                    if (cat.getId() == plantTag.getCategoryId()) {
                        expandableGroupDetailSearch.add(new DetailSearchColumnItem(getContext(), plantTag, false, "", onPlantTagListener));
                    }
                }
            } else {
                ExpandableInnerHeaderItem innerExpandableHeaderPlantsFilter = new ExpandableInnerHeaderItem(getContext(), R.color.expandableHeader_all_white, cat.getTitle(), 0);
                ExpandableGroup innerGroupDetailSearch = new ExpandableGroup(innerExpandableHeaderPlantsFilter);
                for (PlantTags plantTag : tagsList) {
                    if (cat.getId() == plantTag.getCategoryId()) {
                        innerGroupDetailSearch.add(new DetailSearchCardItem(getContext(), plantTag, ObjectDetailMenu.isMulti, "", onPlantTagListener));
                    }
                }
                expandableGroupDetailSearch.add(innerGroupDetailSearch);

            }
        }
        groupAdapter.notifyDataSetChanged();
    }

    private void DetailSearchSingleSelection(PlantTags _plantTag, List<PlantTags> tagUpdate) {
        // remove plantTag if category ids match and selection is not multi
        tagUpdate.removeIf(s -> s.getCategoryId() == _plantTag.getCategoryId());
        //reset tagsList checkbox selection
        for (PlantTags plantTag : tagsList) {
            if (plantTag.getCategoryId() == _plantTag.getCategoryId()) {
                plantTag.setChecked(false);
            }
        }
        tagUpdate.add(_plantTag);
        updateTagsListCheckedStatus(_plantTag, true);
        updateSearchCriteriaLabel();
    }

    private void DetailSearchMultiSelection(PlantTags plantTag, List<PlantTags> tagUpdate) {
        // add plantTag to list <T> if checked
        if (plantTag.isChecked()) {
            tagUpdate.add(plantTag);
            updateTagsListCheckedStatus(plantTag, true);
            Log.d(TAG, "DetailSearchMultiSelection: " + plantTag.toString());

        } else {
            tagUpdate.remove(plantTag);
        }
        updateSearchCriteriaLabel();
    }

    private void onSuccessCount(String plantCount) {

        int plantResultCount = Integer.parseInt(plantCount);
        if (plantResultCount != 0) {
            totalPageCount = Integer.parseInt(plantCount) / PLANT_TAKE_COUNT;
            textViewHit.setText(plantCount + " Treffer");

            //remove no search result section
            try {
                if (noSearchResultSection != null)
                    groupAdapter.remove(noSearchResultSection);
            } catch (Exception e) {
                e.printStackTrace();
            }

        } else {
            // updatePlantsList();
            onNoResult();
        }

        if (getArguments() != null) {
            showPlantForPassedQuery(plantResultCount);
        }
    }

    private void onNoResult() {
        progressBar.setVisibility(View.GONE);
        textViewHit.setText("0" + " Treffer");

        //group for no search result
        if (noSearchResultSection.getGroupCount() == 0) {
            noSearchResultSection.add(new CardItemNoResult(getContext(), R.string.plantSearch_noResultMessage, R.string.plantSearch_proposePlantForDatabase, R.string.plantSearch_noResultSuggestionHint, onCardClickListener));
        }
        if (groupAdapter.getAdapterPosition(noSearchResultSection) < 0)
            groupAdapter.add(noSearchResultSection);
    }

    private void showPlantForPassedQuery(int plantCount) {
        if (plantCount == 1) {
            updateChipGroupTags();
            layoutManager.scrollToPosition(layoutManager.getItemCount() - 1);
        } else {
            paramSearchedText = "";
            loadPlantsFromApiInfiniteScroll();
            expandableSearchInputHeaderPlantSearch.resetEditText(paramSearchedText);

        }
        setArguments(null);
    }

    private void onSuccessGroups(PlantGroup model, RequestData data) {

        plantGroupsList = model.getGroups();
        gardenGroupsList = model.getGardenGroups();
        setExpandableHeaderCounts();
        clearExpandAbleGroup(expandableGroupPlantGroups);

        for (int i = 0; i < plantGroupsList.size(); i++) {

            expandableGroupPlantGroups.add(new PlantGroupCardItem(getContext(), R.color.expandableGroup_all_whiteSmoke, plantGroupsList.get(i), onPlantGroupClickListener));
        }
        for (int i = 0; i < gardenGroupsList.size(); i++) {

            expandableGroupGardenGroup.add(new GardenGroupCardItem(getContext(), R.color.expandableGroup_all_whiteSmoke, gardenGroupsList.get(i), onGardenGroupClickListener));
        }

        groupAdapter.notifyDataSetChanged();
        progressBar.setVisibility(View.GONE);

    }

    private void onSuccessPlantFamily(JSONArray jsonArray) {

        plantFamilyList = new ArrayList<>();

        for (int i = 0; i < jsonArray.length(); i++) {
            try {
                plantFamilyList.add(jsonArray.get(i).toString());
            } catch (JSONException e) {
                e.printStackTrace();
            }
        }
        setExpandableHeaderCounts();
        clearExpandAbleGroup(expandableGroupPlantFamily);

        for (int i = 0; i < plantFamilyList.size(); i++) {

            expandableGroupPlantFamily.add(new PlantFamilyCardItem(getContext(), R.color.expandableGroup_all_whiteSmoke, plantFamilyList.get(i), onPlantFamilyClickListener));
        }

        groupAdapter.notifyDataSetChanged();
        progressBar.setVisibility(View.GONE);

    }

    private void setExpandableHeaderCounts() {
        expandableInputHeaderPlantGroups.setTitle(plantGroupsList != null ? plantGroupsList.size() : 0);
        expandableInputHeaderGardenGroups.setTitle(gardenGroupsList != null ? gardenGroupsList.size() : 0);
        expandableInputHeaderPlantFamily.setTitle(plantFamilyList != null ? plantFamilyList.size() : 0);
        expandableHeaderPlantEcoCriteria.setTitle(EcoBadges.size());
        expandableHeaderPlantFrostHaerte.setTitle(FIX_FROST_HARDY_COUNT);
        expandableHeaderPlantDetailSearch.setTitle(FIX_DETAIL_SEARCH_COUNT);
    }

    public void updateChipGroupTags() {
        SetupChipGroups();
        updateSearchCriteriaLabel();
    }

    private void SetupChipGroups() {
        selectedFilterCounter = 0;

        if (chipGroup != null) {
            chipGroup.removeAllViews();
        }
        // setup chips for search filter
        setupParamSearchNameChip(paramSearchedText, chipGroup);
        setupParamPlantFamilyChip(paramPlantFamilies, chipGroup);
        setupParamPlantGroupChip(paramPlantGroups, chipGroup);
        setupParamGardenGroupChip(paramGardenGroups, chipGroup);
        setupParamEcoChip(paramEcoTags, chipGroup);

        //Detail search params
        SetupParamsDetailChip(paramCookieTags, chipGroup);
        SetupParamsDetailChip(paramColorsTags, chipGroup);
        SetupParamsDetailChip(paramExcludeTags, chipGroup);
        SetupParamsDetailChip(paramLeafColorsTags, chipGroup);
        SetupParamsDetailChip(paramAutumnColorsTags, chipGroup);

        //slider values
        setupParamFrostHaerteChip(paramFreezes, chipGroup);
        setupParamMonthTagChip(paramMonthTag, chipGroup);
        setupParamHeightTagChip(paramHeightTag, chipGroup);
    }

    //method for updating chipgroup for applied filters after coming back from plant details
    private void updateChipGroupForAppliedFilter() {
        selectedFilterCounter = 0;
        Log.d(TAG, "updateChipGroupForAppliedFilter: " + chipList.size());
        chipGroup.removeAllViews();
        String checkForDuplicatesString = "";
        for (Chip _chip : chipList) {
            if (!checkForDuplicatesString.contains(_chip.getText().toString())) {
                Chip chip = initializeChip();
                chip.setText(_chip.getText());
                chipGroup.addView(chip);
                checkForDuplicatesString += " " + _chip.getText().toString();
                selectedFilterCounter++;
                chip.setOnCloseIconClickListener(v -> {
                    //TODO: check which kind of chip has been closed to determine which filter to remove
                    // aka make removing filters actually do something
                    chipGroup.removeView(chip);
                    selectedFilterCounter--;
                    updateSearchCriteriaLabel();
                    loadPlantsFromApiInfiniteScroll();
                });
            }
        }
        updateSearchCriteriaLabel();
        Log.d(TAG, "updateChipGroupForAppliedFilter: " + checkForDuplicatesString);
        chipList.clear();
        plantSearchPersistDataViewModel.setIsFilterAppliedLiveData(false);
    }

    private void setupParamMonthTagChip(List<Integer> _paramMonthTag, ChipGroup chipGroup) {
        if (_paramMonthTag.size() > 0) {

            String tagName = getShortMonthNameByNumber((_paramMonthTag.get(0))) + " - "
                    + getShortMonthNameByNumber((_paramMonthTag.get(1)));

            // Initialize a new chip instance
            Chip chip = initializeChip();
            chip.setText(tagName);
            selectedFilterCounter++;
            //Added click listener on close icon to remove tag from ChipGroup
            chip.setOnCloseIconClickListener(v -> {
                chipGroup.removeView(chip);
                paramMonthTag.clear();
                selectedFilterCounter--;
                updateSearchCriteriaLabel();
                loadPlantsFromApiInfiniteScroll();
            });
            chipGroup.addView(chip);
            chipList.add(chip);
        }
    }

    private void setupParamHeightTagChip(List<Integer> _paramHeightTag, ChipGroup chipGroup) {
        if (_paramHeightTag.size() > 0) {

            String tagName = _paramHeightTag.get(0) + " - "
                    + _paramHeightTag.get(1);

            // Initialize a new chip instance
            Chip chip = initializeChip();
            chip.setText(tagName);
            selectedFilterCounter++;
            //Added click listener on close icon to remove tag from ChipGroup
            chip.setOnCloseIconClickListener(v -> {
                chipGroup.removeView(chip);
                paramHeightTag.clear();
                selectedFilterCounter--;
                updateSearchCriteriaLabel();
                loadPlantsFromApiInfiniteScroll();
            });
            chipGroup.addView(chip);
            chipList.add(chip);
        }
    }

    private void setupParamFrostHaerteChip(Map<String, String> _paramFreezes, ChipGroup chipGroup) {
        if (_paramFreezes != null && _paramFreezes.size() > 0) {

            Map.Entry<String, String> frostHardParamEntry = _paramFreezes.entrySet().iterator().next();

            // Initialize a new chip instance
            Chip chip = initializeChip();
            chip.setText(frostHardParamEntry.getValue());
            selectedFilterCounter++;
            //Added click listener on close icon to remove tag from ChipGroup
            chip.setOnCloseIconClickListener(v -> {
                chipGroup.removeView(chip);
                paramFreezes.clear();
                selectedFilterCounter--;
                updateSearchCriteriaLabel();
                loadPlantsFromApiInfiniteScroll();
            });
            chipGroup.addView(chip);
            chipList.add(chip);
        }
    }

    private void setupParamEcoChip(List<String> _paramEcoTagList, ChipGroup chipGroup) {
        if (_paramEcoTagList != null) {

            for (String paramsEcoTag : _paramEcoTagList) {
                Optional<BadgesIconVM> matchingObject = badgesIconVMList.stream().
                        filter(p -> p.getId().equals(paramsEcoTag)).
                        findFirst();

                BadgesIconVM ecoTag = matchingObject.get();
                String tagName = ecoTag.getName();
                selectedFilterCounter++;
                // Initialize a new chip instance
                Chip chip = initializeChip();
                chip.setText(tagName);
                //Added click listener on close icon to remove tag from ChipGroup
                chip.setOnCloseIconClickListener(v -> {
                    chipGroup.removeView(chip);
                    _paramEcoTagList.remove(ecoTag.getId());
                    ecoTag.setChecked(false);
                    updateBadgesCheckedStatus(ecoTag);
                    selectedFilterCounter--;
                    updateSearchCriteriaLabel();
                    loadPlantsFromApiInfiniteScroll();
                });
                chipGroup.addView(chip);
                chipList.add(chip);
                Log.d(TAG, "setupParamEcoChip: " + chipList.size());
            }
        }
    }

    private void setupParamPlantGroupChip(List<Integer> _paramPlantGroup, ChipGroup chipGroup) {
        if (_paramPlantGroup.size() > 0) {
            Optional<PlantGroup.Group> matchingObject = plantGroupsList.stream().
                    filter(p -> p.getId() == _paramPlantGroup.get(0)).
                    findFirst();

            PlantGroup.Group badgesVM = matchingObject.get();
            String tagName = badgesVM.getName();
            // Initialize a new chip instance
            Chip chip = initializeChip();
            chip.setText(tagName);
            selectedFilterCounter++;

            //Added click listener on close icon to remove tag from ChipGroup
            chip.setOnCloseIconClickListener(v -> {
                chipGroup.removeView(chip);
                paramPlantGroups.clear();
                selectedFilterCounter--;
                updateSearchCriteriaLabel();
                loadPlantsFromApiInfiniteScroll();
            });
            chipGroup.addView(chip);
            chipList.add(chip);
        }
    }

    private void setupParamGardenGroupChip(List<Integer> _paramGardenGroup, ChipGroup chipGroup) {
        if (_paramGardenGroup.size() > 0) {
            Optional<PlantGroup.GardenGroup> matchingObject = gardenGroupsList.stream().
                    filter(p -> p.getId() == _paramGardenGroup.get(0)).
                    findFirst();

            PlantGroup.GardenGroup badgesVM = matchingObject.get();
            String tagName = badgesVM.getName();
            // Initialize a new chip instance
            Chip chip = initializeChip();
            chip.setText(tagName);
            selectedFilterCounter++;

            //Added click listener on close icon to remove tag from ChipGroup
            chip.setOnCloseIconClickListener(v -> {
                chipGroup.removeView(chip);
                paramGardenGroups.clear();
                selectedFilterCounter--;
                updateSearchCriteriaLabel();
                loadPlantsFromApiInfiniteScroll();
            });
            chipGroup.addView(chip);
            chipList.add(chip);
        }
    }

    private void setupParamPlantFamilyChip(List<String> _paramPlantFamily, ChipGroup chipGroup) {

        if (_paramPlantFamily.size() > 0) {
            Optional<String> matchingObject = plantFamilyList.stream().
                    filter(p -> p.equals(_paramPlantFamily.get(0))).
                    findFirst();

            // Initialize a new chip instance
            Chip chip = initializeChip();
            chip.setText(matchingObject.get());
            selectedFilterCounter++;

            //Added click listener on close icon to remove tag from ChipGroup
            chip.setOnCloseIconClickListener(v -> {
                chipGroup.removeView(chip);
                paramPlantFamilies.clear();
                selectedFilterCounter--;
                updateSearchCriteriaLabel();
                loadPlantsFromApiInfiniteScroll();
            });
            chipGroup.addView(chip);
            chipList.add(chip);
        }
    }

    private void setupParamSearchNameChip(String _searchedTextList, ChipGroup chipGroup) {
        if (_searchedTextList != null && _searchedTextList.length() > 1) {
            selectedFilterCounter++;
            if (chipList.contains(searchNameChip)) {
                chipList.remove(searchNameChip);
                chipGroup.removeView(searchNameChip);
            }
            // Initialize a new chip instance
            searchNameChip = initializeChip();
            searchNameChip.setText(_searchedTextList);
            //Added click listener on close icon to remove tag from ChipGroup
            searchNameChip.setOnCloseIconClickListener(v -> {
                chipGroup.removeView(searchNameChip);
                paramSearchedText = "";
                selectedFilterCounter--;
                expandableSearchInputHeaderPlantSearch.resetEditText(paramSearchedText);
                updateSearchCriteriaLabel();
                loadPlantsFromApiInfiniteScroll();
            });
            chipGroup.addView(searchNameChip);
            chipList.add(searchNameChip);
        }
    }

    private void SetupParamsDetailChip(List<PlantTags> _paramDetailTags, ChipGroup chipGroup) {
        final List<String> exclusionTags = Arrays.asList(getResources().getStringArray(R.array.plantSearch_exclusionTagsArray));
        if (_paramDetailTags != null) {
            for (PlantTags paramsDetailTagId : _paramDetailTags) {
                Optional<PlantTags> matchingObject = _paramDetailTags.stream().
                        filter(p -> p.getId() == paramsDetailTagId.getId()).
                        findFirst();

                PlantTags _plantTag = matchingObject.get();
                String tagName = _plantTag.getTitle();
                Log.d(TAG, "SetupParamsDetailChip: " + paramsDetailTagId.getId() + ": " + tagName);
                selectedFilterCounter++;
                // Initialize a new chip instance
                Chip chip;
                if (exclusionTags.contains(tagName) || paramsDetailTagId.getId() == CONDITIONALLY_POISONOUS_ID || paramsDetailTagId.getId() == HIGHLY_POISONOUS_ID)
                    chip = initializeChipRed();
                else
                    chip = initializeChip();
                chip.setText(tagName);
                //Added click listener on close icon to remove tag from ChipGroup
                chip.setOnCloseIconClickListener(v -> {
                    chipGroup.removeView(chip);
                    // update tagsList check status
                    updateTagsListCheckedStatus(_plantTag, false);

                    _paramDetailTags.remove(_plantTag);
                    selectedFilterCounter--;
                    updateSearchCriteriaLabel();
                    loadPlantsFromApiInfiniteScroll();
                });
                chipGroup.addView(chip);
                chipList.add(chip);
                Log.d(TAG, "SetupParamsDetailChip: " + chipList.size());
            }
        }
    }

    private void updateTagsListCheckedStatus(PlantTags _plantTag, boolean flag) {
        int index = tagsList.indexOf(_plantTag);
        _plantTag.setChecked(flag);
        tagsList.set(index, _plantTag);
        // reload detail list
        //PopulateDetailCategoriesAndTagsList();
    }

    private void updateSearchCriteriaLabel() {
        textViewSearchCriteria.setText("Ihre Suchkriterien (" + selectedFilterCounter + ")");
    }

    @NotNull
    private Chip initializeChip() {
        Chip chip = new Chip(getActivity());
        Log.d(TAG, "initializeChip");
        int paddingDp = (int) TypedValue.applyDimension(
                TypedValue.COMPLEX_UNIT_DIP, 10,
                getContext().getResources().getDisplayMetrics()
        );
        chip.setPadding(paddingDp, paddingDp, paddingDp, paddingDp);
        chip.setCloseIconResource(android.R.drawable.ic_menu_close_clear_cancel);
        chip.setCloseIconEnabled(true);
        return chip;
    }

    @NotNull
    private Chip initializeChipRed() {
        Chip chip = new Chip(getActivity());
        Log.d(TAG, "initializeChipRed");
        int paddingDp = (int) TypedValue.applyDimension(
                TypedValue.COMPLEX_UNIT_DIP, 10,
                getContext().getResources().getDisplayMetrics()
        );
        chip.setPadding(paddingDp, paddingDp, paddingDp, paddingDp);
        chip.setCloseIconResource(android.R.drawable.ic_menu_close_clear_cancel);
        chip.setCloseIconEnabled(true);
        chip.setChipBackgroundColorResource(R.color.chip_plantSearch_setupParamsDetail);
        chip.setTextColor(Color.WHITE);
        return chip;
    }

    private void clearExpandAbleGroup(ExpandableGroup expandableGroup) {
        int groupCount = expandableGroup.getGroupCount();
        for (int i = 1; i < groupCount; i++) {
            Group group = expandableGroup.getGroup(1);
            expandableGroup.remove(group);
        }
    }

    private String getBuilderModifiedUrl(String baseUrl) {
        String selectedParamGroup = paramPlantGroups.stream().map(i -> i.toString()).collect(Collectors.joining(","));
        String selectedParamGardenGroup = paramGardenGroups.stream().map(i -> i.toString()).collect(Collectors.joining(","));
        String selectedParamFamily = paramPlantFamilies.stream().collect(Collectors.joining(","));
        String selectedCookieTags = paramCookieTags.stream().map(i -> "" + i.getId()).collect(Collectors.joining(","));
        String selectedColorsTags = paramColorsTags.stream().map(i -> "" + i.getId()).collect(Collectors.joining(","));
        String selectedExcludeTags = paramExcludeTags.stream().map(i -> "" + i.getId()).collect(Collectors.joining(","));
        String selectedLeafColorTags = paramLeafColorsTags.stream().map(i -> "" + i.getId()).collect(Collectors.joining(","));
        String selectedAutumnColorsTags = paramAutumnColorsTags.stream().map(i -> "" + i.getId()).collect(Collectors.joining(","));
        Log.w(TAG, "getBuilderModifiedUrl: " + selectedAutumnColorsTags);
        String selectedEcoTags = paramEcoTags.stream().collect(Collectors.joining(","));
        Map.Entry<String, String> frostHardParamEntry = null;
        String frostHardValue = null;
        if (paramFreezes != null && paramFreezes.size() > 0) {
            frostHardParamEntry = paramFreezes.entrySet().iterator().next();
            frostHardValue = frostHardParamEntry.getKey();
        }
        Uri.Builder builder = Uri.parse(baseUrl).buildUpon()
                .appendQueryParameter("searchText", paramSearchedText == null ? "" : android.net.Uri.encode(paramSearchedText))
                .appendQueryParameter("cookieTags", selectedCookieTags)
                .appendQueryParameter("ecosTags", paramEcoTags == null ? "" : selectedEcoTags)
                .appendQueryParameter("selHmin", paramHeightTag.size() > 0 ? paramHeightTag.get(0).toString() : String.valueOf(PLANT_MIN_HEIGHT))
                .appendQueryParameter("groupId", selectedParamGroup)
                .appendQueryParameter("gardenGroup", selectedParamGardenGroup)
                .appendQueryParameter("family", selectedParamFamily)
                .appendQueryParameter("selHmax", paramHeightTag.size() > 0 ? paramHeightTag.get(1).toString() : String.valueOf(PLANT_MAX_HEIGHT))
                .appendQueryParameter("freezes", frostHardValue)
                .appendQueryParameter("excludes", selectedExcludeTags)
                .appendQueryParameter("colors", selectedColorsTags)
                .appendQueryParameter("leafColors", selectedLeafColorTags)
                .appendQueryParameter("autumnColors", selectedAutumnColorsTags)
                .appendQueryParameter("selMinMonth", paramMonthTag.size() > 0 ? IncrementMonthIndex(paramMonthTag.get(0)).toString() : JAN_MONTH_VALUE)
                .appendQueryParameter("selMaxMonth", paramMonthTag.size() > 0 ? IncrementMonthIndex(paramMonthTag.get(1)).toString() : DEC_MONTH_VALUE);


        try {
            return URLDecoder.decode(builder.build().toString(), "UTF-8");
        } catch (UnsupportedEncodingException e) {
            e.printStackTrace();
        }
        return "";
    }

    private Integer IncrementMonthIndex(int monthNumber) {
        return monthNumber + 1;
    }


    private List<String> getFrostHardIntervals() {
        String[] sourceArray = {
                "",
                "433,286,287,288,289,290,291,292,293,294",
                "433,286,287,288,289,290,291,292,293",
                "433,286,287,288,289,290,291,292",
                "433,286,287,288,289,290,291",
                "433,286,287,288,289,290",
                "433,286,287,288,289,290",
                "433,286,287,288,289",
                "433,286,287,288",
                "433,286,287",
                "433,286",
                "433,285"
        };
        return Arrays.asList(sourceArray);
    }

    enum FilterType {
        cookieTag,
        ecoTag,
        colorsTag,
        excludeTag,
        leafColorsTag,
        autumnColorsTag,
        plantGroup,
        plantFamily,
        slider,
    }

    public static class DetailMenu {
        public static List<DetailMenu> menuList = Arrays.asList(
                new DetailMenu("Licht", "Standort", cookieTag, true),
                new DetailMenu("Besonderheiten", "Top-Filter", cookieTag, false),
                new DetailMenu("Blühdauer", "Blühdauer", slider, false),
                new DetailMenu("Wuchshöhe", "Wuchshöhe", slider, false),
                new DetailMenu("Blütenfarben", "Blütenfarben", colorsTag, true),
                new DetailMenu("Verwendung", "Verwendung", cookieTag, true),
                new DetailMenu("Ausschlusskriterien", "Ausschlusskriterien", excludeTag, true),
                new DetailMenu("Besonderheiten", "Besonderheiten", cookieTag, true),
                new DetailMenu("Herbstfärbung", "Herbstfärbung", autumnColorsTag, true),

                new DetailMenu("Wuchs", "Wuchs", cookieTag, true),
                new DetailMenu("Nutzpflanzen", "Nutzpflanzen", cookieTag, true),
                new DetailMenu("Dekoaspekte", "Dekoaspekte", cookieTag, true),
                new DetailMenu("Blüten", "Blüten", cookieTag, true),
                new DetailMenu("Blütenform", "Blütenform", cookieTag, true),
                new DetailMenu("Blütengröße", "Blütengröße", cookieTag, false),
                new DetailMenu("Fruchtfarbe", "Fruchtfarbe", cookieTag, true),
                new DetailMenu("Früchte", "Früchte", cookieTag, false),
                new DetailMenu("Blütenstand", "Blütenstand", cookieTag, false),
                new DetailMenu("Blattfarbe", "Blattfarbe", leafColorsTag, true),

                new DetailMenu("Blattrand", "Blattrand", cookieTag, false),
                new DetailMenu("Blattstellung", "Blattstellung", cookieTag, false),
                new DetailMenu("Blattform", "Blattform", cookieTag, false),
                new DetailMenu("Laubrhythmus", "Laubrhythmus", cookieTag, false),
                new DetailMenu("Boden", "Boden", cookieTag, true),
                new DetailMenu("Licht", "Licht", cookieTag, true),
                new DetailMenu("Düngung", "Düngung", cookieTag, true),
                new DetailMenu("Schnitt", "Schnitt", cookieTag, true),
                new DetailMenu("Vermehrung", "Vermehrung", cookieTag, true),
                new DetailMenu("Wasserbedarf", "Wasserbedarf", cookieTag, false)
        );
        public static List<String> menuListOrder = Arrays.asList(
                "Licht", "Blühdauer", "Wuchshöhe", "Blütenfarben",
                "Verwendung", "Ausschlusskriterien", "Besonderheiten", "Herbstfärbung",

                "Wuchs", "Nutzpflanzen", "Dekoaspekte", "Blüten", "Blütenform", "Blütengröße", "Fruchtfarbe", "Früchte", "Blütenstand", "Blattfarbe",

                "Blattrand", "Blattstellung", "Blattform", "Laubrhythmus", "Boden", "Licht", "Düngung", "Schnitt", "Vermehrung", "Wasserbedarf"
        );
        String category;
        String name;
        boolean isMulti;
        FilterType filterType;

        public DetailMenu() {
        }

        public DetailMenu(String category, String name, FilterType filterType, boolean isMulti) {
            this.category = category;
            this.name = name;
            this.filterType = filterType;
            this.isMulti = isMulti;
        }
    }

    public void gotoRecyclerViewPosition() {
        if (plantSearchPersistDataViewModel.getNeedToScrollLiveData().getValue() != null &&
                plantSearchPersistDataViewModel.getNeedToScrollLiveData().getValue() &&
                plantSearchPersistDataViewModel.getRecyclerViewPositionLiveData().getValue() != null) {
            plantsRecycleView.scrollToPosition(plantSearchPersistDataViewModel.getRecyclerViewPositionLiveData().getValue());
            plantSearchPersistDataViewModel.setNeedToScrollLiveData(false);
        }
    }

    @Override
    public void onResume() {
        super.onResume();
        //initializeChipGroups();
        if (plantSearchPersistDataViewModel.getIsFilterAppliedLiveData().getValue() != null && plantSearchPersistDataViewModel.getIsFilterAppliedLiveData().getValue())
            //Log.d(TAG, "onResume: " + chipList.get(0));
            updateChipGroupForAppliedFilter();
    }

    @Override
    public void onActivityCreated(@Nullable Bundle savedInstanceState) {
        super.onActivityCreated(savedInstanceState);
        plantSearchPersistDataViewModel = ViewModelProviders.of(getActivity()).get(PlantSearchPersistDataViewModel.class);
        plantSearchPersistDataViewModel.getChipListLiveData().observe(getViewLifecycleOwner(), _chipList -> {
            chipList = _chipList;
        });
    }
}