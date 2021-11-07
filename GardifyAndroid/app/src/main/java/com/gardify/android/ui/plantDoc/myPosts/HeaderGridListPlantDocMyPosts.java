package com.gardify.android.ui.plantDoc.myPosts;

import androidx.annotation.NonNull;

import com.gardify.android.R;
import com.gardify.android.databinding.ItemHeaderGridListPlantDocMyPostsBinding;
import com.xwray.groupie.databinding.BindableItem;

import static com.gardify.android.ui.myGarden.MyGardenFragment.INSET;
import static com.gardify.android.ui.myGarden.MyGardenFragment.INSET_TYPE_KEY;

public class HeaderGridListPlantDocMyPosts extends BindableItem<ItemHeaderGridListPlantDocMyPostsBinding> {

    private int header;
    private HeaderGridListPlantDocMyPosts.onGridViewClickListener onGridViewClickListener;

    public HeaderGridListPlantDocMyPosts(int header, HeaderGridListPlantDocMyPosts.onGridViewClickListener onGridViewClickListener) {
        this.header = header;
        this.onGridViewClickListener = onGridViewClickListener;
        getExtras().put(INSET_TYPE_KEY, INSET);
    }

    @Override
    public int getLayout() {
        return R.layout.item_header_grid_list_plant_doc_my_posts;
    }

    @Override
    public void bind(@NonNull ItemHeaderGridListPlantDocMyPostsBinding viewBinding, int position) {
        //viewBinding.getRoot().setBackgroundColor(colorRes);
        viewBinding.textPlantDocMyPosts.setText(header);
        viewBinding.imageViewGridPlantDocMyPosts.setOnClickListener(v -> {
            onGridViewClickListener.onClick(true);
        });
        viewBinding.imageViewListPlantDocMyPosts.setOnClickListener(v -> {
            onGridViewClickListener.onClick(false);
        });
    }

    public void setText(int header) {
        this.header = header;
    }

    public int getText() {
        return header;
    }

    public interface onGridViewClickListener {
        void onClick(boolean grid);
    }
}
