package com.gardify.android.ui.home;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Context;
import android.graphics.drawable.ColorDrawable;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.PopupWindow;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.GridLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.bumptech.glide.Glide;
import com.gardify.android.R;
import com.gardify.android.data.misc.RequestData;
import com.gardify.android.data.news.News;
import com.gardify.android.data.video.Video;
import com.gardify.android.ui.MainActivity;
import com.gardify.android.ui.generic.materialShowCase.MaterialShowCaseView;
import com.gardify.android.ui.generic.materialShowCase.shape.Focus;
import com.gardify.android.ui.generic.materialShowCase.shape.FocusGravity;
import com.gardify.android.ui.generic.materialShowCase.shape.ShapeType;
import com.gardify.android.ui.generic.recyclerItem.GenericFragmentButton;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.PreferencesUtility;
import com.gardify.android.utils.RequestQueueSingleton;
import com.gardify.android.utils.RequestType;
import com.xwray.groupie.GroupAdapter;
import com.xwray.groupie.Section;

import org.jetbrains.annotations.NotNull;

import java.text.DateFormat;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

import static com.gardify.android.utils.UiUtils.disableHardWareAccelerationForView;
import static com.gardify.android.utils.UiUtils.displayAlertDialog;
import static com.gardify.android.utils.UiUtils.navigateToFragment;
import static com.gardify.android.utils.UiUtils.setupToolbar;


public class HomeFragment extends Fragment {


    private static final String ACTION_ARGUMENT = "action_argument";
    private static final String TAG = "HomeFragment";
    public static final int RECYCLE_VIEW_DEFAULT_SPAN_COUNT = 12;

    //Groupie adapter
    private GroupAdapter groupAdapter;
    private GridLayoutManager layoutManager;
    private RecyclerView recyclerView;


    public View onCreateView(@NonNull LayoutInflater inflater,
                             ViewGroup container, Bundle savedInstanceState) {

        View root = inflater.inflate(R.layout.fragment_home, container, false);

        init(root);
        checkBundleArgument(root);

        setupGroupAdapter();

        getVideoCount();


        return root;
    }

    private void checkBundleArgument(View root) {
        if (getArguments() != null) {
            int actionStringId = getArguments().getInt(ACTION_ARGUMENT);

            // perform action based on arguments
            switch (actionStringId) {
                case R.string.all_guidedTour:
                    popupGuidedTourStart(root);
                    break;
            }
        }
    }

    public void init(View root) {
        /* finding views block */
        recyclerView = root.findViewById(R.id.recycler_view_fragment_home);

        disableHardWareAccelerationForView(recyclerView);
    }


    private void setupGroupAdapter() {
        groupAdapter = new GroupAdapter();
        groupAdapter.setSpanCount(RECYCLE_VIEW_DEFAULT_SPAN_COUNT);
        layoutManager = new GridLayoutManager(getContext(), groupAdapter.getSpanCount());
        layoutManager.setSpanSizeLookup(groupAdapter.getSpanSizeLookup());
        recyclerView.setLayoutManager(layoutManager);
        populateAdapter();
        recyclerView.setAdapter(groupAdapter);
    }

    GenericFragmentButton myGardenButton, gardifyNewsButton, gardifyVideoButton, todoCalendarButton,
            gardifyPlusButton, gardifyShopButton, suggestPlantButton, gardenGlossaryButton, ecoScanButton,
            weatherButton, plantDocButton, plantScanButton, plantSearchButton, guidedTourButton;


    private void populateAdapter() {

        myGardenButton = new GenericFragmentButton.Builder(getContext())
                .setButtonIcon(R.drawable.gardify_app_icon_mein_garten_normal)
                .setTitle(R.string.home_my)
                .setSubTitle(R.string.home_garden)
                .setBackgroundColor(R.color.cardView_all_home)
                .setButtonClickListener(buttonClickListener(R.id.nav_controller_my_garden))
                .build();

        Section myGardenSection = new Section(myGardenButton);
        groupAdapter.add(myGardenSection);

        todoCalendarButton = new GenericFragmentButton.Builder(getContext())
                .setButtonIcon(R.drawable.gardify_app_icon_to_do_kalender)
                .setTitle(R.string.all_toDo)
                .setSubTitle(R.string.all_calendar)
                .setBackgroundColor(R.color.cardView_home_todoCalender)
                .setButtonClickListener(buttonClickListener(R.id.nav_controller_todo))
                .build();

        Section todoCalendarSection = new Section(todoCalendarButton);
        groupAdapter.add(todoCalendarSection);

        plantSearchButton = new GenericFragmentButton.Builder(getContext())
                .setButtonIcon(R.drawable.gardify_app_icon_pflanzensuche)
                .setTitle(R.string.all_plants)
                .setSubTitle(R.string.all_search)
                .setBackgroundColor(R.color.cardView_home_plantSearch)
                .setButtonClickListener(buttonClickListener(R.id.nav_controller_plant_search))
                .build();

        Section plantSearchsection = new Section(plantSearchButton);
        groupAdapter.add(plantSearchsection);

        plantScanButton = new GenericFragmentButton.Builder(getContext())
                .setButtonIcon(R.drawable.gardify_app_icon_pflanzen_erkennen)
                .setTitle(R.string.all_plants)
                .setSubTitle(R.string.home_scan)
                .setBackgroundColor(R.color.cardView_all_sea)
                .setButtonClickListener(buttonClickListener(R.id.nav_controller_plant_scan))
                .build();

        Section plantScanSection = new Section(plantScanButton);
        groupAdapter.add(plantScanSection);


        gardifyNewsButton = new GenericFragmentButton.Builder(getContext())
                .setButtonIcon(R.drawable.gardify_app_icon_news)
                .setTitle(R.string.all_gardify)
                .setSubTitle(R.string.all_news)
                .setBackgroundColor(R.color.cardView_home_gardifyNews)
                .enableNotificationCount(true)
                .setButtonClickListener(buttonClickListener(R.id.nav_controller_news))
                .build();

        Section gardifyNewsSection = new Section(gardifyNewsButton);
        groupAdapter.add(gardifyNewsSection);

        gardifyVideoButton = new GenericFragmentButton.Builder(getContext())
                .setButtonIcon(R.drawable.gardify_app_icon_video)
                .setTitle(R.string.home_garden)
                .setSubTitle(R.string.home_video)
                .setBackgroundColor(R.color.cardView_all_home)
                .enableNotificationCount(true)
                .setButtonClickListener(buttonClickListener(R.id.nav_controller_gardify_video))
                .build();

        Section gardifyVideoSection = new Section(gardifyVideoButton);
        groupAdapter.add(gardifyVideoSection);

        plantDocButton = new GenericFragmentButton.Builder(getContext())
                .setButtonIcon(R.drawable.gardify_app_icon_pflanzendoc)
                .setTitle(R.string.all_plants)
                .setSubTitle(R.string.home_doc)
                .setBackgroundColor(R.color.cardView_home_plantDoc)
                .setButtonClickListener(buttonClickListener(R.id.nav_controller_plant_doc))
                .build();

        Section plantDocSection = new Section(plantDocButton);
        groupAdapter.add(plantDocSection);

        weatherButton = new GenericFragmentButton.Builder(getContext())
                .setButtonIcon(R.drawable.gardify_app_icon_gartenwetter)
                .setTitle(R.string.home_garden)
                .setSubTitle(R.string.home_weather)
                .setBackgroundColor(R.color.cardView_home_gardenWeather)
                .setButtonClickListener(buttonClickListener(R.id.nav_controller_weather))
                .build();

        Section weatherSection = new Section(weatherButton);
        groupAdapter.add(weatherSection);

        gardifyShopButton = new GenericFragmentButton.Builder(getContext())
                .setButtonIcon(R.drawable.gardify_app_icon_shop)
                .setTitle(R.string.all_gardify)
                .setSubTitle(R.string.home_shop)
                .setBackgroundColor(R.color.cardView_home_gardifyShop)
                .setButtonClickListener(view -> displayAlertDialog(getContext(), "Demnächst verfügbar"))
                .build();

        Section gardifyShopSection = new Section(gardifyShopButton);
        groupAdapter.add(gardifyShopSection);

        ecoScanButton = new GenericFragmentButton.Builder(getContext())
                .setButtonIcon(R.drawable.gardify_app_icon_oekoscan)
                .setTitle(R.string.home_garden)
                .setSubTitle(R.string.all_ecoScan)
                .setBackgroundColor(R.color.cardView_home_gardenEcoScan)
                .setButtonClickListener(buttonClickListener(R.id.nav_controller_eco_scan))
                .build();

        Section ecoScanSection = new Section(ecoScanButton);
        groupAdapter.add(ecoScanSection);

        gardenGlossaryButton = new GenericFragmentButton.Builder(getContext())
                .setButtonIcon(R.drawable.gardify_app_icon_gartenwissen)
                .setTitle(R.string.home_garden)
                .setSubTitle(R.string.home_glossary)
                .setBackgroundColor(R.color.cardView_home_gardenGlossar)
                .setButtonClickListener(buttonClickListener(R.id.nav_controller_garden_glossary))
                .build();

        Section gardenGlossarySection = new Section(gardenGlossaryButton);
        groupAdapter.add(gardenGlossarySection);

        suggestPlantButton = new GenericFragmentButton.Builder(getContext())
                .setButtonIcon(R.drawable.gardify_app_icon_pflanze_ergaenzen)
                .setTitle(R.string.all_plants)
                .setSubTitle(R.string.home_suggest)
                .setBackgroundColor(R.color.cardView_all_suggestPlant)
                .setButtonClickListener(buttonClickListener(R.id.nav_controller_suggest_plants))
                .build();

        Section suggestPlantSection = new Section(suggestPlantButton);
        groupAdapter.add(suggestPlantSection);

        gardifyPlusButton = new GenericFragmentButton.Builder(getContext())
                .setButtonIcon(R.drawable.gardify_app_icon_gardify_plus)
                .setTitle(R.string.all_gardify)
                .setSubTitle(R.string.home_plus)
                .setBackgroundColor(R.color.cardView_home_gardifyPlus)
                .setButtonClickListener(view -> displayAlertDialog(getContext(), "Demnächst verfügbar"))
                .build();

        Section gardifyPlusSection = new Section(gardifyPlusButton);
        groupAdapter.add(gardifyPlusSection);

        guidedTourButton = new GenericFragmentButton.Builder(getContext())
                .setButtonIcon(R.drawable.gardify_app_icon_guided_tour)
                .setTitle(R.string.home_guided)
                .setSubTitle(R.string.home_tour)
                .setBackgroundColor(R.color.cardView_all_home)
                .setButtonClickListener(this::popupGuidedTourStart)
                .build();

        Section guidedTourSection = new Section(guidedTourButton);
        groupAdapter.add(guidedTourSection);


    }

    @NotNull
    private GenericFragmentButton.FragmentButtonClickListener buttonClickListener(int controller) {
        return view -> {
            navigateToFragment(controller, getActivity(), true, null);
        };
    }

    private void getVideoCount() {
        String videoApiURL = APP_URL.VIDEO_API;
        RequestQueueSingleton.getInstance(getContext()).typedRequest(videoApiURL, this::onSuccessVideo, null, Video[].class, new RequestData(RequestType.Video));
    }

    private void onSuccessVideo(Video[] videos, RequestData data) {
        if (getActivity() != null) {
            List<Video> videoArrayList;
            int newVideoCount = 0;

            DateFormat sdf = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");

            videoArrayList = Arrays.asList(videos);
            //adapter
            String lastWatchedVideoDate = PreferencesUtility.getLatestWatchedVideoDate(getContext());

            newVideoCount = CalculateNewVideoOrNewsCount(newVideoCount, videoArrayList, null, sdf, lastWatchedVideoDate);
            UpdateNewVideoCountLabel(newVideoCount, lastWatchedVideoDate, videoArrayList.size());

            //check Latest news Count
            String newsApiUrl = APP_URL.NEWS_API + "?take=10";
            RequestQueueSingleton.getInstance(getContext()).typedRequest(newsApiUrl, this::onSuccessNews, null, News.class, new RequestData(RequestType.News));

        }
    }

    private void UpdateNewVideoCountLabel(int newVideoCount, String lastWatchedVideoDate, int totalVideoCount) {
        if (lastWatchedVideoDate == "") {
            gardifyVideoButton.setNotificationCount(totalVideoCount);
            gardifyVideoButton.notifyChanged();
        } else if (newVideoCount > 0) {
            gardifyVideoButton.setNotificationCount(newVideoCount);
            gardifyVideoButton.notifyChanged();
        }
    }

    private void onSuccessNews(News news, RequestData data) {
        if (getActivity() != null) {
            int newNewsCount = 0;
            List<News.ListEntry> newsList = news.getListEntries();

            DateFormat sdf = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");

            //adapter
            String lastSeenNewsDate = PreferencesUtility.getLatestSeenNewsDate(getContext());

            newNewsCount = CalculateNewVideoOrNewsCount(newNewsCount, null, newsList, sdf, lastSeenNewsDate);
            UpdateNewNewsCountLabel(newNewsCount, lastSeenNewsDate, newsList.size());
        }
    }


    private void UpdateNewNewsCountLabel(int newNewsCount, String lastSeenNewsDate, int totalVideoCount) {
        if (lastSeenNewsDate == "") {
            gardifyNewsButton.setNotificationCount(totalVideoCount);
        } else if (newNewsCount > 0) {
            gardifyNewsButton.setNotificationCount(totalVideoCount);
        }
    }

    private int CalculateNewVideoOrNewsCount(int newCount, List<Video> videoArrayList, List<News.ListEntry> newsList, DateFormat sdf, String lastItemSeenDate) {
        if (videoArrayList != null) {
            newCount = countNewVideos(newCount, videoArrayList, sdf, lastItemSeenDate);
        } else if (newsList != null) {
            newCount = countNewNews(newCount, newsList, sdf, lastItemSeenDate);
        }

        return newCount;
    }

    private int countNewVideos(int newCount, List<Video> videoArrayList, DateFormat sdf, String lastItemSeenDate) {
        for (Video video : videoArrayList) {
            try {
                if (sdf.parse(video.getDate()).after(sdf.parse(lastItemSeenDate))) {
                    newCount++;
                }
            } catch (ParseException e) {
                // Log.d(TAG, "date not parsable");
            }
        }
        return newCount;
    }

    private int countNewNews(int newCount, List<News.ListEntry> newsList, DateFormat sdf, String lastItemSeenDate) {
        for (News.ListEntry news : newsList) {
            try {
                if (sdf.parse(news.getDate()).after(sdf.parse(lastItemSeenDate))) {
                    newCount++;
                }
            } catch (ParseException e) {
                //        Log.d(TAG, "date not parsable");
            }
        }
        return newCount;
    }

    @SuppressLint("ClickableViewAccessibility")
    public void popupGuidedTourStart(View v) {

        LayoutInflater layoutInflater = (LayoutInflater) getContext().getSystemService(Context.LAYOUT_INFLATER_SERVICE);
        final View popupView = layoutInflater.inflate(R.layout.showcase_guided_tour_start, null);
        PopupWindow popupWindow = new PopupWindow(
                popupView,
                ViewGroup.LayoutParams.WRAP_CONTENT,
                ViewGroup.LayoutParams.WRAP_CONTENT);
        ImageView imageViewStart = popupView.findViewById(R.id.imageView_popup_guided_tour_start);
        Button btnStart = popupView.findViewById(R.id.button_popup_guided_tour_start);
        ImageView imageButtonClose = popupView.findViewById(R.id.imageButton_popup_guided_tour_start_close_button);

        Glide.with(this).load(R.drawable.guided_tour_grafik).into(imageViewStart);

        imageButtonClose.setOnClickListener(v1 -> popupWindow.dismiss());

        btnStart.setOnClickListener(v2 -> {
            initializeShowCaseList();
            popupWindow.dismiss();

        });

        popupWindow.setBackgroundDrawable(new ColorDrawable(android.graphics.Color.TRANSPARENT));
        popupWindow.setElevation(20);
        popupWindow.setOutsideTouchable(false);
        popupWindow.showAtLocation(v, Gravity.CENTER, 0, 0);

    }

    private void initializeShowCaseList() {
        List<FragmentButtonModel> fragmentButtonModelList = new ArrayList<>();

        fragmentButtonModelList.add(new FragmentButtonModel(myGardenButton, R.string.menuActivityMainDrawer_myGarden, R.string.home_guidedTourPopupMyGarden));
        fragmentButtonModelList.add(new FragmentButtonModel(todoCalendarButton, R.string.menuActivityMainDrawer_toDo, R.string.home_guidedTourPopupTodoCalendar));
        fragmentButtonModelList.add(new FragmentButtonModel(plantSearchButton, R.string.menuActivityMainDrawer_plantSearch, R.string.home_guidedTourPopupSearchPlant));
        fragmentButtonModelList.add(new FragmentButtonModel(plantDocButton, R.string.menuActivityMainDrawer_plantDoc, R.string.home_guidedTourPopupPlantDoc));
        fragmentButtonModelList.add(new FragmentButtonModel(plantScanButton, R.string.menuActivityMainDrawer_plantScan, R.string.home_guidedTourPopupPlantScan));
        fragmentButtonModelList.add(new FragmentButtonModel(ecoScanButton, R.string.menuActivityMainDrawer_ecoScan, R.string.home_guidedTourPopupEcoScan));
        fragmentButtonModelList.add(new FragmentButtonModel(suggestPlantButton, R.string.menuActivityMainDrawer_suggestPlant, R.string.home_guidedTourPopupSuggestPlants));
        fragmentButtonModelList.add(new FragmentButtonModel(weatherButton, R.string.menuActivityMainDrawer_gardenWeather, R.string.home_guidedTourPopupGardenWeather));
        fragmentButtonModelList.add(new FragmentButtonModel(gardifyShopButton, R.string.menuActivityMainDrawer_gardifyShop, R.string.home_guidedTourPopupGardifyShop));


        //bottom nav settings icon
        View settingsBottomNavIcon = getActivity().findViewById(R.id.bottom_navigation_settings);

        fragmentButtonModelList.add(new FragmentButtonModel(settingsBottomNavIcon, R.string.menuActivityMainDrawerFooter_settings, R.string.home_guidedTourPopupSettings));

        displayShowCase(fragmentButtonModelList, 0); // initial position 0
    }

    private void displayShowCase(List<FragmentButtonModel> fragmentButtonsList, int position) {
        Object object = fragmentButtonsList.get(position).getObject();
        if (object instanceof GenericFragmentButton) {
            GenericFragmentButton fragmentButton = (GenericFragmentButton) object;
            int adapterPosition = groupAdapter.getAdapterPosition(fragmentButton);
            recyclerView.scrollToPosition(adapterPosition);
        }

        final Handler handler = new Handler(Looper.getMainLooper());
        handler.postDelayed(() -> buildShowCase(position, fragmentButtonsList, fragmentButtonsList.get(position).getObject()), 200);

    }


    private <T> void buildShowCase(int position, List<FragmentButtonModel> fragmentButtonsList, T object) {
        View view;

        if (object instanceof GenericFragmentButton) {
            GenericFragmentButton fragmentBtn = (GenericFragmentButton) object;
            view = fragmentBtn.getView();

        } else { // other view types
            view = (View) object;
        }

        new MaterialShowCaseView.Builder((Activity) getContext())
                .setFocusGravity(FocusGravity.CENTER)
                .setFocusType(Focus.NORMAL)
                .setOnClickListener((isNext, isPrevious) -> {
                    if (isNext && position < fragmentButtonsList.size() - 1) {
                        int pos = position + 1;
                        displayShowCase(fragmentButtonsList, pos);
                    }
                    if (isPrevious && position > 0) {
                        int pos = position - 1;
                        displayShowCase(fragmentButtonsList, pos);
                    }
                })
                .setTotalShowcaseCount(showCountStartingFromOne(fragmentButtonsList, position))
                .setTitleText(getContext().getResources().getString(fragmentButtonsList.get(position).title))
                .setInfoText(getContext().getResources().getString(fragmentButtonsList.get(position).body))
                .setShape(ShapeType.RECTANGLE)
                .setTarget(view)
                .show();

    }

    @NotNull
    private String showCountStartingFromOne(List<FragmentButtonModel> fragmentButtonsList, int position) {
        int currentPos = position + 1;
        int lastPos = fragmentButtonsList.size();
        return currentPos + "/" + lastPos;
    }


    @Override
    public void onResume() {
        setupToolbar(getActivity(), "", 0, R.color.colorPrimary, false);
        ((MainActivity) getActivity()).updateActionBarCounters();

        super.onResume();
    }


    public class FragmentButtonModel<T> {
        private T object;
        private int title, body;

        FragmentButtonModel(T object, int title, int body) {
            this.object = object;
            this.title = title;
            this.body = body;
        }


        public T getObject() {
            return object;
        }

        public int getTitle() {
            return title;
        }

        public int getBody() {
            return body;
        }

    }

}