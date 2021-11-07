package com.gardify.android.ui.generic.recyclerItem;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.GridLayoutManager;

import com.gardify.android.R;
import com.gardify.android.databinding.RecyclerItemCardViewSectionBinding;
import com.xwray.groupie.databinding.BindableItem;

import static com.gardify.android.ui.myGarden.MyGardenFragment.INSET;
import static com.gardify.android.ui.myGarden.MyGardenFragment.INSET_TYPE_KEY;

public class CardViewTopBottomSection extends BindableItem<RecyclerItemCardViewSectionBinding> {

    boolean top;

    public CardViewTopBottomSection(boolean isTop) {
        top = isTop;

        getExtras().put(INSET_TYPE_KEY, INSET);
    }

    @Override
    public int getLayout() {
        return R.layout.recycler_item_card_view_section;
    }

    @Override
    public void bind(@NonNull RecyclerItemCardViewSectionBinding binding, int position) {
        binding.cardViewExpandableHeader.setElevation(0);
        if (top) {
            binding.cardViewExpandableHeader.setBackgroundResource(R.drawable.card_view_top_corner);
            ((GridLayoutManager.LayoutParams) binding.cardViewExpandableHeader.getLayoutParams()).setMargins(0, 5, 0, 0);
        } else {
            binding.cardViewExpandableHeader.setBackgroundResource(R.drawable.card_view_bottom_corner);
            ((GridLayoutManager.LayoutParams) binding.cardViewExpandableHeader.getLayoutParams()).setMargins(0, 0, 0, 10);
        }

    }
}
