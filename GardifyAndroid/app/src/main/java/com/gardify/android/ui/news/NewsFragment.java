package com.gardify.android.ui.news;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ProgressBar;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.GridLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.gardify.android.R;
import com.gardify.android.data.misc.RequestData;
import com.gardify.android.data.news.InstaNews;
import com.gardify.android.data.news.News;
import com.gardify.android.ui.generic.HeaderItemDecoration;
import com.gardify.android.ui.generic.recyclerItem.GenericButton;
import com.gardify.android.ui.generic.recyclerItem.HeaderTwoIcons;
import com.gardify.android.ui.news.recyclerItem.NewsCardItem;
import com.gardify.android.ui.news.recyclerItem.NewsGridItem;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.PreferencesUtility;
import com.gardify.android.utils.RequestQueueSingleton;
import com.gardify.android.utils.RequestType;
import com.xwray.groupie.GroupAdapter;
import com.xwray.groupie.Section;

import java.util.ArrayList;
import java.util.List;

import static com.gardify.android.ui.home.HomeFragment.RECYCLE_VIEW_DEFAULT_SPAN_COUNT;
import static com.gardify.android.utils.APP_URL.isAndroid;
import static com.gardify.android.utils.UiUtils.setupToolbar;

public class NewsFragment extends Fragment implements View.OnClickListener {


    private RecyclerView recyclerViewNews;
    private ProgressBar progressBar;
    private GroupAdapter groupAdapter;
    private GridLayoutManager layoutManager;
    private List<News.ListEntry> newsArrayList;
    private List<InstaNews.Datum> instaNewsArrayList;
    private boolean isInstagram = false;
    private boolean isGrid = true;

    public View onCreateView(@NonNull LayoutInflater inflater,
                             ViewGroup container, Bundle savedInstanceState) {

        View root = inflater.inflate(R.layout.fragment_general_recycleview, container, false);

        //initialize views
        init(root);

        String newsApiUrl = APP_URL.INSTA_NEWS_API + isAndroid() + "&take=20";
        RequestQueueSingleton.getInstance(getContext()).typedRequest(newsApiUrl, this::onSuccessNewsInsta, null, InstaNews.class, new RequestData(RequestType.InstaNews));
        progressBar.setVisibility(View.VISIBLE);

        setupToolbar(getActivity(), "NEWS", R.drawable.gardify_app_icon_news, R.color.toolbar_gardifyNews_setupToolbar, true);

        return root;
    }

    public void init(View root) {
        /* finding views block */
        recyclerViewNews = root.findViewById(R.id.recycler_view_fragment_general);
        progressBar = root.findViewById(R.id.progressbar_fragment_general);
        newsArrayList = new ArrayList<>();
    }


    private void onSuccessNews(News news, RequestData data) {
        progressBar.setVisibility(View.GONE);

        newsArrayList = news.getListEntries();
        //adapter

        groupAdapter.remove(updateWithPayloadSection);
        // Update with payload
        updateWithPayloadSection = new Section();
        for (int i = 0; i < newsArrayList.size(); i++) {
            updateWithPayloadSection.add(new NewsCardItem(i, getContext(), newsArrayList.get(i), null, onNewsClickListener));
        }
        groupAdapter.add(updateWithPayloadSection);

    }

    private void onSuccessNewsInsta(InstaNews news, RequestData data) {
        progressBar.setVisibility(View.GONE);

        instaNewsArrayList = news.getData();
        //adapter

        if (updateWithPayloadSection == null) {
            groupAdapter = new GroupAdapter();
            groupAdapter.setSpanCount(RECYCLE_VIEW_DEFAULT_SPAN_COUNT);
            layoutManager = new GridLayoutManager(getContext(), groupAdapter.getSpanCount());
            layoutManager.setSpanSizeLookup(groupAdapter.getSpanSizeLookup());
            recyclerViewNews.setLayoutManager(layoutManager);
            recyclerViewNews.addItemDecoration(new HeaderItemDecoration(0, 0));
            recyclerViewNews.setAdapter(groupAdapter);

            populateAdapter();
            recyclerViewNews.setAdapter(groupAdapter);

            //get latest seen news date and store in sharedPref
            String date = instaNewsArrayList.get(0).getTimestamp();
            PreferencesUtility.setLatestSeenNewsDate(getContext(), date);

        } else {
            groupAdapter.remove(updateWithPayloadSection);
            // Update with payload
            updateWithPayloadSection = new Section();
            for (int i = 0; i < instaNewsArrayList.size(); i++) {
                updateWithPayloadSection.add(new NewsCardItem(i, getContext(), null, instaNewsArrayList.get(i), onNewsClickListener));
            }
            groupAdapter.add(updateWithPayloadSection);

        }

    }

    private Section updateWithPayloadSection;
    private GenericButton genericButton;
    private void populateAdapter() {

        Section headerGridList = new Section(new HeaderTwoIcons("", getContext(), isGrid, R.drawable.mein_garten_ansicht_kacheln_on, R.drawable.mein_garten_ansicht_karten_on, onIconClickListener));
        groupAdapter.add(headerGridList);

        genericButton = new GenericButton.Builder(getContext())
                .addNewButton(R.style.PrimaryButtonStyle, R.string.garden_news, R.dimen.textSize_body_xsmall, (GenericButton.HeaderButtonClickListener) (buttonString, view) -> {
                    headerButtonClickListener.onClick(buttonString, view);
                    genericButton.setSelectedButton(view);
                    genericButton.notifyChanged();
                })
                .addNewButton(R.style.PrimaryButtonStyle, R.string.gardify_news, R.dimen.textSize_body_xsmall, (GenericButton.HeaderButtonClickListener) (buttonString, view) -> {
                    headerButtonClickListener.onClick(buttonString, view);
                    genericButton.setSelectedButton(view);
                    genericButton.notifyChanged();
                })
                .setButtonColorState(R.color.button_background_color_state, R.color.button_text_color_state)
                .build();

        Section headerButtons = new Section(genericButton);
        groupAdapter.add(headerButtons);

        // Main recycleView Update with payload
        updateWithPayloadSection = new Section();
        for (int i = 0; i < instaNewsArrayList.size(); i++) {
            updateWithPayloadSection.add(new NewsCardItem(i, getContext(), null, instaNewsArrayList.get(i), onNewsClickListener));
        }

        groupAdapter.add(updateWithPayloadSection);
    }

    private NewsCardItem.OnNewsClickListener onNewsClickListener = (item, flag) -> {
        // Pretend to make a network request

        Toast.makeText(getContext(), "clicked " + flag, Toast.LENGTH_SHORT).show();
    };


    private HeaderTwoIcons.onIconClickListener onIconClickListener = (grid, view) -> {
        isGrid = grid;
        if (!isInstagram) {
            if (grid) {
                groupAdapter.remove(updateWithPayloadSection);
                // Update with payload
                updateWithPayloadSection = new Section();
                for (int i = 0; i < newsArrayList.size(); i++) {
                    updateWithPayloadSection.add(new NewsGridItem(i, getContext(), newsArrayList.get(i), null));

                }

            } else {
                // Update with payload
                groupAdapter.remove(updateWithPayloadSection);
                // Update with payload
                updateWithPayloadSection = new Section();
                for (int i = 0; i < newsArrayList.size(); i++) {
                    updateWithPayloadSection.add(new NewsCardItem(i, getContext(), newsArrayList.get(i), null, onNewsClickListener));
                }
            }
        } else {
            if (grid) {
                groupAdapter.remove(updateWithPayloadSection);
                // Update with payload
                updateWithPayloadSection = new Section();
                for (int i = 0; i < instaNewsArrayList.size(); i++) {
                    updateWithPayloadSection.add(new NewsGridItem(i, getContext(), null, instaNewsArrayList.get(i)));
                }

            } else {
                // Update with payload
                groupAdapter.remove(updateWithPayloadSection);
                // Update with payload
                updateWithPayloadSection = new Section();
                for (int i = 0; i < instaNewsArrayList.size(); i++) {
                    updateWithPayloadSection.add(new NewsCardItem(i, getContext(), null, instaNewsArrayList.get(i), onNewsClickListener));
                }
            }
        }
        groupAdapter.add(updateWithPayloadSection);
    };

    private GenericButton.HeaderButtonClickListener headerButtonClickListener = (stringId, view) -> {
        // Pretend to make a network request
        String newsApiUrl;
        switch (stringId) {
            case (R.string.gardify_news):
                newsApiUrl = APP_URL.NEWS_API + "?take=20";
                RequestQueueSingleton.getInstance(getContext()).typedRequest(newsApiUrl, this::onSuccessNews, null, News.class, new RequestData(RequestType.News));
                isInstagram = false;
                break;
            case (R.string.garden_news):
                newsApiUrl = APP_URL.INSTA_NEWS_API + "?take=20";
                RequestQueueSingleton.getInstance(getContext()).typedRequest(newsApiUrl, this::onSuccessNewsInsta, null, InstaNews.class, new RequestData(RequestType.News));
                isInstagram = true;
                break;
        }

        progressBar.setVisibility(View.VISIBLE);
    };

    @Override
    public void onClick(View view) {

        switch (view.getId()) {

        }
    }
}