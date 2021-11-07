package com.gardify.android.ui.video;

import android.content.res.Resources;
import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ProgressBar;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.gardify.android.R;
import com.gardify.android.data.misc.RequestData;
import com.gardify.android.data.video.Video;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.PreferencesUtility;
import com.gardify.android.utils.RequestQueueSingleton;
import com.gardify.android.utils.RequestType;

import org.jetbrains.annotations.NotNull;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;
import java.util.stream.Collectors;

import static com.gardify.android.ui.video.VideoRecyclerViewAdapter.SORT_BY_DATE;
import static com.gardify.android.ui.video.VideoRecyclerViewAdapter.SORT_BY_POPULARITY;
import static com.gardify.android.utils.APP_URL.isAndroid;
import static com.gardify.android.utils.CollectionUtils.removeDuplicates;
import static com.gardify.android.utils.UiUtils.setupToolbar;

public class VideoFragment extends Fragment {

    private RecyclerView recyclerViewVideo;
    private ProgressBar progressBar;
    private List<Video> videoArrayList;
    private List<String> topicTagsList = new ArrayList<>();
    private VideoRecyclerViewAdapter recyclerViewAdapter;

    public View onCreateView(@NonNull LayoutInflater inflater,
                             ViewGroup container, Bundle savedInstanceState) {

        View root = inflater.inflate(R.layout.fragment_general_recycleview, container, false);

        setupToolbar(getActivity(), "GARTEN VIDEOS", R.drawable.gardify_app_icon_video, R.color.toolbar_all_greyishTurquoise, true);

        initializeViews(root);

        String videoApiURL = APP_URL.VIDEO_API + isAndroid();
        RequestQueueSingleton.getInstance(getContext()).typedRequest(videoApiURL, this::onSuccessVideo, null, Video[].class, new RequestData(RequestType.Video));
        progressBar.setVisibility(View.VISIBLE);
        return root;
    }

    public void initializeViews(View root) {
        /* finding views block */
        recyclerViewVideo = root.findViewById(R.id.recycler_view_fragment_general);
        progressBar = root.findViewById(R.id.progressbar_fragment_general);

        RecyclerView.LayoutManager mLayoutManager = new LinearLayoutManager(getContext());
        recyclerViewVideo.setLayoutManager(mLayoutManager);
    }


    private void onSuccessVideo(Video[] videos, RequestData data) {
        progressBar.setVisibility(View.GONE);
        videoArrayList = new ArrayList<>();
        videoArrayList = Arrays.asList(videos);

        topicTagsList = getTopicTagsToList(videoArrayList);

        populateAdapter(videoArrayList, topicTagsList);

        updateSeenVideoDate_SharedPreferences();
    }

    public void onError(Exception ex, RequestData data) {
        if (getActivity() != null) {
            Resources res = getActivity().getResources();
            String network = res.getString(R.string.all_network);
            String anErrorOccurred = res.getString(R.string.all_anErrorOccurred);
            Log.e(network, anErrorOccurred + ex.toString());
        }
    }

    private List<String> getTopicTagsToList(List<Video> _videoArrayList) {
        for (Video video : _videoArrayList) {
            topicTagsList.addAll(video.getTags());
        }
        // return distinct only
        topicTagsList = removeDuplicates(topicTagsList);
        // sort alphabetically
        Collections.sort(topicTagsList);
        return topicTagsList;
    }

    private void populateAdapter(List<Video> _videoArrayList, List<String> topicTagsList) {
        recyclerViewAdapter = new VideoRecyclerViewAdapter(getContext(), _videoArrayList, topicTagsList, this.getLifecycle(), onVideoFilterClickListener);
        recyclerViewVideo.setAdapter(recyclerViewAdapter);
    }

    private void updateSeenVideoDate_SharedPreferences() {
        //get latestVideo date and store in latest uploaded video
        String date = videoArrayList.get(0).getDate();
        PreferencesUtility.setLatestWatchedVideoDate(getContext(), date);
    }

    private VideoRecyclerViewAdapter.onVideoFilterClickListener onVideoFilterClickListener = (sortString, topicString) -> {
        // Pretend to make a network request
        List<Video> videoSortedList = videoArrayList;

        videoSortedList = filterVideoBySelection(sortString, topicString, videoSortedList);

        recyclerViewAdapter.updateVideosList(videoSortedList);
    };

    private List<Video> filterVideoBySelection(String sortString, String topicString, List<Video> videoSortedList) {
        sortString = sortString != null ? sortString : "";

        if (sortString.equalsIgnoreCase(SORT_BY_POPULARITY)) {
            videoSortedList = sortVideosByPopularity();

        } else if (sortString.equalsIgnoreCase(SORT_BY_DATE)) {
            sortVideosByDate(videoSortedList);
        }

        if (topicString != null && topicString!="") {
            videoSortedList = sortVideosByTopic(topicString);
        }
        return videoSortedList;
    }

    private void sortVideosByDate(List<Video> videoSortedList) {
        videoSortedList.sort(Comparator.comparing(o -> o.getDate()));
    }

    @NotNull
    private List<Video> sortVideosByPopularity() {
        List<Video> videoSortedList;
        videoSortedList = videoArrayList.stream()
                .sorted(Comparator.comparingInt(Video::getViewCount))
                .collect(Collectors.toList());
        return videoSortedList;
    }

    @NotNull
    private List<Video> sortVideosByTopic(String topicString) {
        List<Video> videoSortedList;
        videoSortedList =
                videoArrayList.stream()
                        .filter(d -> d.getTags().stream().anyMatch(po -> po.equalsIgnoreCase(topicString)))
                        .collect(Collectors.toList());
        return videoSortedList;
    }
}