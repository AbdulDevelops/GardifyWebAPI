package com.gardify.android.ui.news.recyclerItem;

import android.content.Context;
import android.view.View;
import android.widget.MediaController;

import androidx.annotation.NonNull;

import com.gardify.android.data.news.InstaNews;
import com.gardify.android.data.news.News;
import com.gardify.android.R;
import com.gardify.android.ui.myGarden.MyGardenFragment;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.databinding.ItemNewsCardBinding;
import com.xwray.groupie.databinding.BindableItem;

import static com.gardify.android.utils.StringUtils.formatHtmlKTags;
import static com.gardify.android.utils.UiUtils.loadImageUsingGlide;


public class NewsCardItem extends BindableItem<ItemNewsCardBinding> {


    private OnNewsClickListener onNewsClickListener;
    private News.ListEntry news;
    private InstaNews.Datum instaNews;
    private Context context;
    private boolean expand = false;

    public NewsCardItem(long id, Context context, News.ListEntry news, InstaNews.Datum instaNews, OnNewsClickListener onNewsClickListener) {
        super(id);
        this.onNewsClickListener = onNewsClickListener;
        this.news = news;
        this.instaNews = instaNews;
        this.context = context;
        getExtras().put(MyGardenFragment.INSET_TYPE_KEY, MyGardenFragment.INSET);
    }

    @Override
    public int getLayout() {
        return R.layout.item_news_card;
    }

    @Override
    public void bind(@NonNull final ItemNewsCardBinding binding, int position) {

        binding.expandIconNews.setOnClickListener(v -> {
            if(!expand){
                binding.expandIconNews.setImageResource(R.drawable.community_pfeil_grau_zu);
                binding.textViewEntryDescription.setMaxLines(50);
                expand = true;
            }else {
                binding.expandIconNews.setImageResource(R.drawable.gardify_app_icon_pfeil_nach_unten);
                binding.textViewEntryDescription.setMaxLines(4);
                expand = false;
            }

        });

        if (news != null) {
            binding.imageViewEntryImage.setVisibility(View.VISIBLE);
            binding.newsVideoPlayer.setVisibility(View.GONE);

            binding.textViewEntryTitle.setText(formatHtmlKTags(news.getTitle()));
            binding.textViewEntryDescription.setText(formatHtmlKTags(news.getText()));
            String imageUrl = APP_URL.BASE_ROUTE_INTERN + news.getEntryImages().get(0).getSrcAttr();
            loadImageUsingGlide(context, imageUrl, binding.imageViewEntryImage);

        } else {
            binding.textViewEntryTitle.setVisibility(View.GONE);
            binding.textViewEntryDescription.setText(instaNews.getCaption());

            String contentUrl = instaNews.getMediaUrl();

            if(instaNews.getMediaType().equalsIgnoreCase("VIDEO")){
                binding.imageViewEntryImage.setVisibility(View.GONE);
                binding.newsVideoPlayer.setVisibility(View.VISIBLE);
                binding.newsVideoPlayer.setVideoPath(contentUrl);
                binding.newsVideoPlayer.pause();
                binding.newsVideoPlayer.seekTo(1);

                //setting media controllers
                MediaController mediaController = new MediaController(context);
                mediaController.setAnchorView(binding.newsVideoPlayer);
                binding.newsVideoPlayer.setMediaController(mediaController);
            }else {
                binding.imageViewEntryImage.setVisibility(View.VISIBLE);
                binding.newsVideoPlayer.setVisibility(View.GONE);
                loadImageUsingGlide(context, contentUrl, binding.imageViewEntryImage);

            }
        }
    }

    public interface OnNewsClickListener {
        void onClick(NewsCardItem item, boolean flag);
    }
}
