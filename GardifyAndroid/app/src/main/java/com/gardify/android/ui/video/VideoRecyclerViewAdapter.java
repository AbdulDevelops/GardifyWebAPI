package com.gardify.android.ui.video;

import android.annotation.SuppressLint;
import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.Spinner;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.lifecycle.Lifecycle;
import androidx.recyclerview.widget.RecyclerView;

import com.gardify.android.R;
import com.gardify.android.data.video.Video;
import com.pierfrancescosoffritti.androidyoutubeplayer.core.player.YouTubePlayer;
import com.pierfrancescosoffritti.androidyoutubeplayer.core.player.listeners.AbstractYouTubePlayerListener;
import com.pierfrancescosoffritti.androidyoutubeplayer.core.player.views.YouTubePlayerView;

import java.util.Arrays;
import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import static android.view.View.GONE;
import static android.view.View.VISIBLE;

public class VideoRecyclerViewAdapter extends RecyclerView.Adapter<RecyclerView.ViewHolder> {

    private static final int TYPE_HEADER = 0;
    private static final int TYPE_ITEM = 1;
    public static final String SORT_BY_POPULARITY = "Beliebtheit";
    public static final String SORT_BY_DATE = "Datum";

    private List<Video> videosList;
    private Lifecycle lifecycle;
    private Context context;

    boolean expand = true;
    private boolean isVideoSortTouched = false;
    private boolean isTopicTagsTouched = false;
    private List<String> topicTagsList;
    private onVideoFilterClickListener onVideoFilterClickListener;
    private String selectedPopularity="";
    private String selectedTopic="";
    VideoRecyclerViewAdapter(Context context, List<Video> videosList, List<String> topicTagsList, Lifecycle lifecycle,
                             onVideoFilterClickListener onVideoFilterClickListener) {
        this.context = context;
        this.videosList = videosList;
        this.topicTagsList = topicTagsList;
        this.topicTagsList.add(0,"");
        this.lifecycle = lifecycle;
        this.onVideoFilterClickListener = onVideoFilterClickListener;
    }

    @NonNull
    @Override
    public RecyclerView.ViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
        if (viewType == TYPE_ITEM) {
            // Here Inflating your recyclerview item layout
            View itemView = LayoutInflater.from(parent.getContext()).inflate(R.layout.fragment_video_recycler_view_video_row, parent, false);
            YouTubePlayerView youTubePlayerView = itemView.findViewById(R.id.youtube_player_view);
            lifecycle.addObserver(youTubePlayerView);
            return new ItemViewHolder(itemView);

        } else if (viewType == TYPE_HEADER) {
            // Here Inflating your header view
            View itemView = LayoutInflater.from(parent.getContext()).inflate(R.layout.fragment_video_recycler_view_video_header, parent, false);
            return new HeaderViewHolder(itemView);
        } else return null;
    }

    @Override
    public void onBindViewHolder(final RecyclerView.ViewHolder holder, int position) {

        if (holder instanceof HeaderViewHolder) {
            HeaderViewHolder headerViewHolder = (HeaderViewHolder) holder;

            headerViewHolder.bindView();

        } else if (holder instanceof ItemViewHolder) {

            final ItemViewHolder itemViewHolder = (ItemViewHolder) holder;

            Video video = videosList.get(ItemPosition(position));
            String youtubeUrl = video.getYTLink();
            String videoId = youtubeUrlIdExtractor(youtubeUrl);
            itemViewHolder.cueVideo(videoId);
            itemViewHolder.bindView(video);

        }
    }

    private int ItemPosition(int position) {
        return position - 1;
    }

    public String youtubeUrlIdExtractor(String youtubeUrl) {
        String pattern = "(?<=watch\\?v=|/videos/|embed\\/|youtu.be\\/|\\/v\\/|\\/e\\/|watch\\?v%3D|watch\\?feature=player_embedded&v=|%2Fvideos%2F|embed%\u200C\u200B2F|youtu.be%2F|%2Fv%2F)[^#\\&\\?\\n]*";

        Pattern compiledPattern = Pattern.compile(pattern);
        Matcher matcher = compiledPattern.matcher(youtubeUrl); //url is youtube url for which you want to extract the id.
        if (matcher.find()) {
            return matcher.group();
        } else {
            return "";
        }
    }

    @Override
    public int getItemViewType(int position) {
        if (position == 0) {
            return TYPE_HEADER;
        }
        return TYPE_ITEM;
    }

    @Override
    public int getItemCount() {
        return videosList.size() + 1;
    }


    private class HeaderViewHolder extends RecyclerView.ViewHolder {
        TextView videoHeaderSearch;
        ImageView imageViewExpandButton;
        Spinner videoPopularityDateSortSpinner;
        Spinner videoTopicSortSpinner;
        LinearLayout spinnersLinearLayout;

        public HeaderViewHolder(View view) {
            super(view);
            videoHeaderSearch = view.findViewById(R.id.textView_video_header_search);
            imageViewExpandButton = view.findViewById(R.id.imageView_video_header_expand_button);
            videoPopularityDateSortSpinner = view.findViewById(R.id.spinner_video_popularity_date_sort);
            videoTopicSortSpinner = view.findViewById(R.id.spinner_video_topic_sort);
            spinnersLinearLayout = view.findViewById(R.id.linear_layout_video_sort_spinners);
        }

        public void bindView() {
            setupSortSpinner();
            setupTagsSpinner();

            videoHeaderSearch.setText("Suchen");
            imageViewExpandButton.setOnClickListener(v -> {
                if (expand) {
                    spinnersLinearLayout.setVisibility(VISIBLE);
                    imageViewExpandButton.setImageResource(R.drawable.collapse);
                    expand = false;
                } else {
                    spinnersLinearLayout.setVisibility(GONE);
                    imageViewExpandButton.setImageResource(R.drawable.expand);
                    expand = true;
                }
            });
        }

        @SuppressLint("ClickableViewAccessibility")
        private void setupSortSpinner() {
            List<String> popularityFilter = Arrays.asList("", SORT_BY_POPULARITY, SORT_BY_DATE);
            ArrayAdapter<String> sortArrayAdapter = new ArrayAdapter<>(context.getApplicationContext(), R.layout.custom_spinner_item, popularityFilter);
            videoPopularityDateSortSpinner.setAdapter(sortArrayAdapter);
            videoPopularityDateSortSpinner.setSelection(popularityFilter.indexOf(selectedPopularity));
            videoPopularityDateSortSpinner.setOnTouchListener((v, event) -> {
                isVideoSortTouched = true;
                return false;
            });
            videoPopularityDateSortSpinner.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
                @Override
                public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
                    if (isVideoSortTouched) {
                        isVideoSortTouched = false;
                        selectedPopularity = popularityFilter.get(position);
                        onVideoFilterClickListener.onClick(selectedPopularity, null);
                    }
                }

                @Override
                public void onNothingSelected(AdapterView<?> parent) {
                }
            });
        }

        @SuppressLint("ClickableViewAccessibility")
        private void setupTagsSpinner() {
            ArrayAdapter<String> topicArrayAdapter = new ArrayAdapter<>(context.getApplicationContext(), R.layout.custom_spinner_item, topicTagsList);
            videoTopicSortSpinner.setAdapter(topicArrayAdapter);
            videoTopicSortSpinner.setSelection(topicTagsList.indexOf(selectedTopic));
            videoTopicSortSpinner.setOnTouchListener((v, event) -> {
                isTopicTagsTouched = true;
                return false;
            });
            videoTopicSortSpinner.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
                @Override
                public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
                    if (isTopicTagsTouched) {
                        isTopicTagsTouched = false;
                        selectedTopic = topicTagsList.get(position);
                        onVideoFilterClickListener.onClick(null, selectedTopic);
                    }
                }

                @Override
                public void onNothingSelected(AdapterView<?> parent) {
                }
            });
        }
    }

    public class ItemViewHolder extends RecyclerView.ViewHolder {

        private YouTubePlayerView youTubePlayerView;
        private YouTubePlayer youTubePlayer;
        private String currentVideoId;
        private TextView videoTitleTextView;

        ItemViewHolder(View view) {
            super(view);
            youTubePlayerView = view.findViewById(R.id.youtube_player_view);
            videoTitleTextView = view.findViewById(R.id.textView_video_name);

            youTubePlayerView.addYouTubePlayerListener(new AbstractYouTubePlayerListener() {
                @Override
                public void onReady(@NonNull YouTubePlayer initializedYouTubePlayer) {
                    youTubePlayer = initializedYouTubePlayer;
                    youTubePlayer.cueVideo(currentVideoId, 0);
                }
            });
        }

        void cueVideo(String videoId) {
            currentVideoId = videoId;

            if (youTubePlayer == null)
                return;

            youTubePlayer.cueVideo(videoId, 0);
        }

        public void bindView(Video video) {
            videoTitleTextView.setText(video.getTitle());
        }
    }

    public void updateVideosList(List<Video> videosList) {
        this.videosList = videosList;
        notifyDataSetChanged();
    }

    public interface onVideoFilterClickListener {
        void onClick(String sortString, String topicString);
    }
}