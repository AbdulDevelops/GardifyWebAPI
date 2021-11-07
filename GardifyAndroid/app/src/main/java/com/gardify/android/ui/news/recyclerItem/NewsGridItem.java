package com.gardify.android.ui.news.recyclerItem;

import android.content.Context;
import android.view.View;
import android.widget.MediaController;

import androidx.annotation.NonNull;

import com.gardify.android.data.news.InstaNews;
import com.gardify.android.data.news.News;
import com.gardify.android.R;
import com.gardify.android.databinding.ItemGenericGridImageBinding;
import com.gardify.android.ui.myGarden.MyGardenFragment;
import com.gardify.android.utils.APP_URL;
import com.xwray.groupie.databinding.BindableItem;

import static com.gardify.android.utils.UiUtils.loadImageUsingGlide;

public class NewsGridItem extends BindableItem<ItemGenericGridImageBinding> {

    public static final String FAVORITE = "FAVORITE";

    private boolean checked = false;
    private News.ListEntry news;
    private InstaNews.Datum instaNews;
    private Context context;

    public NewsGridItem(long id, Context context, News.ListEntry news, InstaNews.Datum instaNews) {
        super(id);
        this.news = news;
        this.context = context;
        this.instaNews = instaNews;

        getExtras().put(MyGardenFragment.INSET_TYPE_KEY, MyGardenFragment.INSET);
    }

    @Override
    public int getLayout() {
        return R.layout.item_generic_grid_image;
    }

    @Override
    public void bind(@NonNull final ItemGenericGridImageBinding binding, int position) {

        if (news != null) {
            binding.image.setVisibility(View.VISIBLE);
            binding.newsVideoPlayer.setVisibility(View.GONE);
            String imageUrl = APP_URL.BASE_ROUTE_INTERN + news.getEntryImages().get(0).getSrcAttr();
            loadImageUsingGlide(context, imageUrl, binding.image);

        } else {

            String contentUrl = instaNews.getMediaUrl();

            if (instaNews.getMediaType().equalsIgnoreCase("VIDEO")) {
                binding.image.setVisibility(View.GONE);
                binding.newsVideoPlayer.setVisibility(View.VISIBLE);
                binding.newsVideoPlayer.setVideoPath(contentUrl);
                binding.newsVideoPlayer.pause();
                binding.newsVideoPlayer.seekTo(1);

                //setting media controllers
                MediaController mediaController = new MediaController(context);
                mediaController.setAnchorView(binding.newsVideoPlayer);
                binding.newsVideoPlayer.setMediaController(mediaController);

            } else {
                binding.image.setVisibility(View.VISIBLE);
                binding.newsVideoPlayer.setVisibility(View.GONE);
                loadImageUsingGlide(context, contentUrl, binding.image);

            }
        }

    }

    @Override
    public int getSpanSize(int spanCount, int position) {

        // for larger screens set span count to 3

        boolean isTablet = context.getResources().getBoolean(R.bool.isTablet);
        if (isTablet) {
            return spanCount / 3;
        } else {
            return spanCount / 2;
        }

    }

}
